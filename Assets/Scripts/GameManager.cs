using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

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
        public PlayerState PlayerState = new PlayerState();

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
            yield return new WaitForSeconds(1f); // 애니메이션 등을 고려한 대기
            
            // 3. 재난 이벤트
            if (CurrentTurn % 4 == 0)
            {
                SetState(GameState.Disaster);
                Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
                // yield return StartCoroutine(HandleDisasterEvent()); // 여기 안에서 자원 소모로 넘겨줘야 함.
            }
            else
            {
                SetState(GameState.ResourceConsuming);
                Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
            }
            
            yield return new WaitForSeconds(2f);
            
            
            // // 4. 자원 소모 (식량 등)
            // SetState(GameState.ResourceConsuming);
            // yield return StartCoroutine(HandleResourceConsuming());
            //
            // // 5. 재난 예보
            // SetState(GameState.Forecasting);
            // ShowForecast();
            // yield return new WaitForSeconds(1f);

            // 다음 턴으로
            StartTurn();
        }

    }
}