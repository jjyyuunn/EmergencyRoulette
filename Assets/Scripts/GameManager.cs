using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

namespace EmergencyRoulette
{
    [DefaultExecutionOrder(-10)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameState
        {
            None,
            Spin,
            Resolving,
            Disaster,
            ResourceConsuming,
            Forecasting,
            Ended
        }

        public GameState CurrentState { get; private set; } = GameState.None;
        public int CurrentTurn { get; private set; } = 0;
        public DisasterEventItem CurrentDisaster { get; private set; } = null;
        
        public SymbolLibrary SymbolLibrary = new SymbolLibrary();

        public void SetState(GameState newState)
        {
            CurrentState = newState;
            Debug.Log($"[GameState] ���� ��ȯ: {newState}");
        }

        private ExcelManager _excelManager;
        public static ExcelManager ExcelManager
        {
            get { return Instance._excelManager; }
            set { Instance._excelManager = value;  Debug.Log("excelManager Loaded");}
        }
        
        private SlotManager _slotManager;
        public static SlotManager SlotManager
        {
            get { return Instance._slotManager; }
            set { Instance._slotManager = value;  Debug.Log("slotManager Loaded");}
        }

        // example
        public Dictionary<int, TestItem> TestItemDict => ExcelManager.TestItemDict;
        public Dictionary<string, ModuleDataItem> ModuleDict => ExcelManager.ModuleDict;
        public Dictionary<string, DisasterEventItem> DisasterEventDict => ExcelManager.DisasterEventDict;
        public PlayerState PlayerState = new PlayerState();
        public PlayerStateUI playerStateUI;

        public bool CanPlayerInteract { get; set; } // 기본 자원 사용

        void Awake()
        {
            Init();
        }

        private void Start()
        {
            StartTurn();
        }

        private static void Init()
        {
            if (Instance == null)
            {
                GameObject go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject { name = "@Managers" };
                    go.AddComponent<GameManager>();
                }
                Instance = go.GetComponent<GameManager>();
                DontDestroyOnLoad(Instance.gameObject);
            }
        }
        
        // Turn 관련 코드
        private void StartTurn()
        {
            if (++CurrentTurn > 16)
            {
                SetState(GameState.Ended);
                return;
            }
            
            Debug.Log($"[Turn] 턴 {CurrentTurn} 시작");
            StartCoroutine(RunTurnSequence());
        }
        
        private IEnumerator RunTurnSequence()
        {
            // 1. 스핀 단계
            SetState(GameState.Spin);
            Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
            yield return new WaitUntil(() => CurrentState == GameState.Resolving);
            _slotManager.HasSpunThisTurn = false;
            
            // 2. 결과 처리 단계
            // 콤보, 패널티, 기본 심볼 이펙트 처리
            Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
            yield return new WaitForSeconds(7f);
            SlotManager?.PlayAllSlotPendingEffects();
            playerStateUI.RefreshUI();
            yield return new WaitForSeconds(1f); // 애니메이션 등을 고려한 대기
            
            // 3. 재난 이벤트
            if (CurrentTurn % 4 == 0)
            {
                SetState(GameState.Disaster);
                Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
                
                yield return StartCoroutine(HandleDisasterEvent());
            }
            
            // 4. 자원 소모 (식량 등)
            SetState(GameState.ResourceConsuming);
            Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
            // 완료버튼 누르면 gamestate 바뀌게. 다시 disable.
            
            SetState(GameState.Forecasting); //임시
            yield return new WaitUntil(() => CurrentState == GameState.Forecasting);
            
            // 5. 재난 예보
            if (CurrentTurn == 1 || CurrentTurn % 4 == 0) SetDisaster();
            
            // CurrentDisaster를 ui에 띄워줌. 일단은 디버그 로그로
            if (CurrentTurn != 16)
            {
                Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
                Debug.Log($"Disaster: {CurrentDisaster.disaster}, Info: {CurrentDisaster.information}, {4-(CurrentTurn % 4)} days left!");
            }

            yield return new WaitForSeconds(2f); // 메시지 보여줄 시간
            SetState(GameState.None);

            // 다음 턴으로
            yield return new  WaitUntil(() => CurrentState == GameState.None);
            StartTurn();
        }

        private void SetDisaster()
        {
            var disasters = DisasterEventDict.Values.ToList();
            if (disasters.Count == 0)
            {
                Debug.LogWarning("재난 데이터가 없습니다.");
                return;
            }
            int randomIndex = UnityEngine.Random.Range(0, disasters.Count);
            CurrentDisaster = disasters[randomIndex];
        }
        
        private IEnumerator HandleDisasterEvent()
        {
            var disaster = CurrentDisaster;
            Debug.Log($"[Disaster] 재난 발생! {disaster.disaster}");

            bool hasEnough = CheckAndConsumeResources(disaster);

            if (!hasEnough)
            {
                Debug.Log("[Disaster] 자원 부족! 패널티 적용");
                ApplyRandomDisasterPenalty();
            }

            // 재난 연출을 위한 잠깐 대기 (나중에 애니메이션 대체 가능)
            yield return new WaitForSeconds(2f);
        }
        
        private bool CheckAndConsumeResources(DisasterEventItem disaster)
        {
            var state = PlayerState;

            if (state.Food < disaster.food ||
                state.Energy < disaster.energy ||
                state.Data < disaster.data ||
                state.Technology < disaster.technology)
            {
                return false; // 자원 부족
            }

            // 자원 충분
            state.Food -= disaster.food;
            state.Energy -= disaster.energy;
            state.Data -= disaster.data;
            state.Technology -= disaster.technology;

            Debug.Log("[Disaster] 자원 소비 완료");
            return true;
        }
        
        private void ApplyRandomDisasterPenalty()
        {
            int option = UnityEngine.Random.Range(1, 6); // 1~5

            switch (option)
            {
                case 1:
                    // 추가 수정
                    Debug.Log("패널티 1: 모듈 하나 파괴");
                    break;
                case 2:
                    SymbolLibrary.MultiplyProbability(SymbolType.Discharge, 2);
                    SymbolLibrary.MultiplyProbability(SymbolType.Outdated, 2);
                    Debug.Log("패널티 2: 방전/낙후 심볼 가중치 2배");
                    break;
                case 3:
                    SymbolLibrary.MultiplyProbability(SymbolType.Warning, 2);
                    Debug.Log("패널티 3: 경고 심볼 가중치 2배");
                    break;
                case 4:
                    PlayerState.UseFoodBonus++;
                    Debug.Log("패널티 4: 식량 소비 +1");
                    break;
                case 5:
                    PlayerState.OverloadGauge += 20;
                    Debug.Log("패널티 5: 과부하 게이지 +20%");
                    break;
            }
        }


    }
}