using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
//using UnityEditor;
using UnityEngine.Analytics;

namespace EmergencyRoulette
{
    public enum GameState
    {
        None,
        StartTurn,
        Spin,
        Resolving,
        Disaster,
        ResourceConsuming,
        Forecasting,
        Ended
    }

    [DefaultExecutionOrder(-10)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; } = GameState.None;
        public int CurrentTurn { get; private set; } = 0;
        public DisasterEventItem CurrentDisaster { get; private set; } = null;
        public SymbolLibrary SymbolLibrary = new SymbolLibrary();

        /*
        public void SetState(GameState newState)
        {
            CurrentState = newState;
            Debug.Log($"[GameState] ���� ��ȯ: {newState}");
        }*/

        public void SetState(GameState newState)
        {
            Debug.Log($"GameManager: 상태 전환 {CurrentState} → {newState}");
            CurrentState = newState;

            switch (newState)
            {
                case GameState.StartTurn: StartCoroutine(EnterStartTurn()); break;
                case GameState.Spin: EnterSpin(); break;
                case GameState.Resolving: StartCoroutine(EnterResolving()); break;
                case GameState.Disaster: StartCoroutine(EnterDisaster()); break;
                case GameState.ResourceConsuming: StartCoroutine(EnterResourceConsuming()); break;
                case GameState.Forecasting: EnterForecasting(); break;
                case GameState.Ended: EnterEnded(); break;
            }
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

        public bool IsShopActive = false;
        public GameObject UI_Shop;
        
        public TextMeshProUGUI dayText;
        public TextMeshProUGUI announcementText;
        public Image announcementSymbol;
        public Image notice;
        public TextMeshProUGUI noticeText;
        
        public GameObject gameOver;

        //public SlotBackGroundUIController slotBackGroundUIController;

        public bool CanPlayerInteract { get; set; } // 기본 자원 사용
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }


        private void Start()
        {
            gameOver.SetActive(false);
            notice.gameObject.SetActive(false);
            SetState(GameState.StartTurn);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                //EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
            }

        }    

        private IEnumerator EnterStartTurn()
        {
            if (++CurrentTurn > 16)
            {
                SetState(GameState.Ended);
                yield break;
            }
            dayText.text = $"Day {CurrentTurn}";
            Debug.Log($"[Turn] 턴 {CurrentTurn} 시작");

            // 0. 재난 예보
            if (CurrentTurn % 4 == 1)
            {
                SetDisaster();
                Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
                Debug.Log($"Disaster: {CurrentDisaster.disaster}, Info: {CurrentDisaster.information}, {4 - (CurrentTurn % 4)} days left!");
                yield return new WaitForSeconds(3f); // 메시지 보여줄 시간
            }

            if (CurrentTurn % 4 == 0)
            {
                announcementText.text = "오늘 재난이 발생합니다. 대비하세요.";
            }
            else
            {
                announcementText.text = $"{4 - (CurrentTurn % 4)}일 후 재난 발생 예정입니다.";
            }

            SlotManager.Instance.ToggleBtnAnimate();

            SetState(GameState.Spin);
        }

        private void EnterSpin()
        {
            Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
        }

        private IEnumerator EnterResolving()
        {
            _slotManager.HasSpunThisTurn = false;

            // 2. 결과 처리 단계
            // 콤보, 패널티, 기본 심볼 이펙트 처리
            Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
            yield return new WaitForSeconds(4f);
            SlotManager?.PlayAllSlotPendingEffects();
            playerStateUI.RefreshUI();
            yield return new WaitForSeconds(1f); // 애니메이션 등을 고려한 대기

            SetState(GameState.Disaster);
        }

        private IEnumerator EnterDisaster()
        {
            if (CurrentTurn % 4 == 0)
            {
                Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");

                notice.gameObject.SetActive(true);
                noticeText.text = $"{CurrentDisaster.disaster}: {CurrentDisaster.information}";

                yield return new WaitForSeconds(2.5f);
                yield return StartCoroutine(HandleDisasterEvent());
                notice.gameObject.SetActive(false);   
            }
            SetState(GameState.ResourceConsuming);
        }

        private IEnumerator EnterResourceConsuming()
        {
            Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
            ModuleManager.Instance.SetupShop();
            //slotBackGroundUIController.UpdateRowUI();

            yield return new WaitUntil(() => CurrentState == GameState.StartTurn);
        }

        private void EnterForecasting()
        {

        }

        private void EnterEnded()
        {

        }

        /*
        // Turn 관련 코드
        private void StartTurn()
        {
            if (++CurrentTurn > 16)
            {
                SetState(GameState.Ended);
                return;
            }
            dayText.text = $"Day {CurrentTurn}";
            
            Debug.Log($"[Turn] 턴 {CurrentTurn} 시작");
            StartCoroutine(RunTurnSequence());
        }
        
        private IEnumerator RunTurnSequence()
        {
            // 0. 재난 예보
            if (CurrentTurn % 4 == 1)
            {
                SetDisaster();
                Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
                Debug.Log($"Disaster: {CurrentDisaster.disaster}, Info: {CurrentDisaster.information}, {4-(CurrentTurn % 4)} days left!");
                yield return new WaitForSeconds(3f); // 메시지 보여줄 시간
            }
            announcementText.text = $"{4-(CurrentTurn % 4)}일 후 재난 발생 예정입니다.";
            
            // 1. 스핀 단계
            SetState(GameState.Spin);
            Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
            yield return new WaitUntil(() => CurrentState == GameState.Resolving);
            _slotManager.HasSpunThisTurn = false;
            
            // 2. 결과 처리 단계
            // 콤보, 패널티, 기본 심볼 이펙트 처리
            Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
            yield return new WaitForSeconds(4f);
            SlotManager?.PlayAllSlotPendingEffects();
            playerStateUI.RefreshUI();
            yield return new WaitForSeconds(1f); // 애니메이션 등을 고려한 대기
            
            // 3. 재난 이벤트
            if (CurrentTurn % 4 == 0)
            {
                SetState(GameState.Disaster);
                Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
                
                notice.gameObject.SetActive(true);
                noticeText.text = $"{CurrentDisaster.disaster}: {CurrentDisaster.information}";

                yield return new WaitForSeconds(2.5f);
                yield return StartCoroutine(HandleDisasterEvent());
                notice.gameObject.SetActive(false);
            }

            // 4. 자원 소모 (식량 등)
            SetState(GameState.ResourceConsuming);
            Debug.Log($"Curren GameState: {CurrentState}, Current Turn: {CurrentTurn}");
            ModuleManager.Instance.SetupShop();
            slotBackGroundUIController.UpdateRowUI();

            yield return new WaitUntil(() => CurrentState == GameState.None);

            // 다음 턴으로
            StartTurn();
        }*/

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

        private void SetDisasterSymbol(DisasterEventItem disaster)
        {
            switch (disaster.disaster)
            {
                // 고치기
                case "발전소 불안정":
                    // announcementSymbol.sprite = Resources.Load<>();
                    break;
            }
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
                    var slotBoard = GameManager.Instance.PlayerState.SlotBoard;

                    List<int> intactRows = new();
                    for (int y = 0; y < slotBoard.RowCount; y++)
                    {
                        if (!ModuleManager.Instance.IsModuleBroken(y))
                            intactRows.Add(y);
                    }

                    if (intactRows.Count == 0)
                    {
                        Debug.Log("[DisasterPenalty] 부술 수 있는 모듈 없음");
                        return;
                    }

                    int randRow = UnityEngine.Random.Range(0, intactRows.Count);
                    ModuleManager.Instance.SetModuleBroken(intactRows[randRow]);
                    Debug.Log($"패널티 1: Row {intactRows[randRow]} 모듈 파괴");
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

        public void CompleteResourceConsuming()
        {
            if (CurrentState != GameState.ResourceConsuming) return;

            // 모듈 상점 닫기
            if(IsShopActive)
                ToggleShopUI();

            // Food 자원 처리
            var remainFood = PlayerState.Food - PlayerState.UseFood - PlayerState.UseFoodBonus;
            if (remainFood > 0)
                PlayerState.Food = remainFood;
            else
                PlayerState.OverloadGauge += 10f;

            // 다음턴 기술 보너스
            PlayerState.Technology += PlayerState.NextTurnTechBonus;
            PlayerState.NextTurnTechBonus = 0;

            SetState(GameState.StartTurn);
        }

        public void ToggleShopUI()
        {
            if (CurrentState != GameState.ResourceConsuming)
                return;

            RectTransform rt = UI_Shop.GetComponent<RectTransform>();

            rt.DOKill(); // 중복 방지

            if (IsShopActive)
            {
                // 닫기
                rt.DOAnchorPos(rt.anchoredPosition + new Vector2(500f, 0), 0.4f).SetEase(Ease.OutCubic);
            }
            else
            {
                rt.DOAnchorPos(rt.anchoredPosition - new Vector2(500f, 0), 0.4f).SetEase(Ease.OutCubic);
            }

            IsShopActive = !IsShopActive;
        }
        
        public void TriggerGameOver()
        {
            if (CurrentState != GameState.Ended)
            {
                SetState(GameState.Ended);
                gameOver.SetActive(true);
                Debug.Log("게임 오버! 과부하 게이지가 100%에 도달했습니다.");
                // TODO: 게임오버 UI, 리셋 버튼, 씬 전환 등 필요한 연출 넣기
            }
        }


        public void UseTechToReduceOverload()
        {
            var player = PlayerState;

            if (player.Technology <= 0)
            {
                Debug.Log("[GameManager] 기술 자원이 부족합니다.");
                return;
            }

            player.Technology -= 1;
            player.OverloadGauge -= 5f;

            Debug.Log("[GameManager] 기술 1 소모 → 과부하 게이지 5% 감소");

            playerStateUI.RefreshUI();
        }

    }
}