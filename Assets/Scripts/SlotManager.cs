using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System;
using DG.Tweening;

namespace EmergencyRoulette
{
    public class SlotManager : MonoBehaviour
    {
        public static SlotManager Instance;
        
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform slotParent;

        [SerializeField] private Transform energyAttractor;
        [SerializeField] private Transform foodAttractor;
        [SerializeField] private Transform technologyAttractor;
        [SerializeField] private Transform dataAttractor;

        [SerializeField] private GameObject btnHandle;
        [SerializeField] private GameObject btnBar;

        private bool isBtnExpanded = false;


        private SlotColumnScroller[,] slotInstances;

        private SymbolLibrary _symbolLibrary;
        private SymbolPicker _symbolPicker;
        private SlotBoard _slotBoard;
        public bool HasSpunThisTurn;
        
        public Action OnComplete;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                GameManager.SlotManager = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Start()
        {
            _symbolLibrary = GameManager.Instance.SymbolLibrary;
            _symbolPicker = new SymbolPicker(_symbolLibrary);
            _slotBoard = new SlotBoard(_symbolPicker);

            CreateSlotVisuals(_slotBoard.RowCount, _slotBoard.ColumnCount);
            PrintBoard();
        }

        public void CreateSlotVisuals(int row, int column)
        {
            slotInstances = new SlotColumnScroller[column, row];

            for (int x = 0; x < column; x++)
            {
                for (int y = 0; y < row; y++)
                {
                    GameObject slotObj = Instantiate(slotPrefab, slotParent);
                    var scroller = slotObj.GetComponent<SlotColumnScroller>();
                    slotInstances[x, y] = scroller;

                    var effect = slotObj.GetComponent<SlotParticleEffect>();
                    if (effect != null)
                    {
                        effect.SetupAttractors(
                            energyAttractor,
                            foodAttractor,
                            technologyAttractor,
                            dataAttractor
                        );
                    }

                    RectTransform rt = slotObj.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(x * 250f, -y * 135f);
                    
                    if (!_slotBoard.Rows[y]) slotObj.SetActive(false);
                }
            }
        }

        public void StartSpin()
        {
            if (GameManager.Instance.CurrentState != GameState.Spin)
                return;

            if (HasSpunThisTurn)
                return;

            SoundManager.Instance.PlaySlotButtonClickSound();
            _slotBoard.ResetGainedSymbols(); // 심볼 리셋
            ModuleManager.Instance.ClearUsedActiveModulesThisTurn();

            _slotBoard.Spin();
            HasSpunThisTurn = true;

            ToggleBtnAnimate();
            
            int pendingCount = 0;

            for (int y = 0; y < _slotBoard.RowCount; y++)
            {
                if (_slotBoard.Rows[y])
                {
                    for (int x = 0; x < _slotBoard.ColumnCount; x++)
                    {
                        SymbolType symbol = _slotBoard.Get(x, y);
                        var instance = slotInstances[x, y];
                        pendingCount++;
                        
                        instance.StartCoroutine(instance.StartSpin(symbol, () =>
                        {
                            pendingCount--;
                            if (pendingCount == 0)
                            {
                                OnAllSpinComplete();
                            }
                        }));
                    }
                }
            }

            PrintBoard();
            AddResource();
        }
        
        private void OnAllSpinComplete()
        {
            Debug.Log("All spin animations completed");
            GameManager.Instance.SetState(GameState.Resolving);
        }

        // 플레이어 리소스 더하기
        private void AddResource()
        {
            var playerState = GameManager.Instance.PlayerState;

            playerState.SetInternalData(_slotBoard);

            // 콤보 모듈 → 심볼 조합 처리
            playerState.SetResourceCombo();
            playerState.SetSpecialCombo();
            playerState.SetPenaltyCombos();

            // 패시브 모듈 적용
            ModuleEffectExecutor.ApplyAllPassiveModules(playerState);
            // 로그
            Debug.Log("=== After PassiveModule ===");
            playerState.PrintResources();

            // 기본 심볼 생산
            playerState.SetNormalState();
        }

        public void PlayAllSlotPendingEffects()
        {
            SoundManager.Instance.PlaySlotResourceGainSound();
            foreach (var slot in slotInstances)
            {
                slot.GetComponent<SlotParticleEffect>()?.PlayPending();
            }
        }

        public void LockRow(int y)
        {
            _slotBoard.LockRow(y);
            for (int x = 0; x < _slotBoard.ColumnCount; x++)
            {
                slotInstances[x, y].gameObject.SetActive(false);
            }
        }
        
        public void UnlockRow(int y)
        {
            _slotBoard.UnlockRow(y);
            for (int x = 0; x < _slotBoard.ColumnCount; x++)
            {
                slotInstances[x, y].gameObject.SetActive(true);
            }
        }

        public void ToggleBtnAnimate()
        {
            isBtnExpanded = !isBtnExpanded;

            // btnBar 위치 이동
            Vector3 targetBarPos = isBtnExpanded ? new Vector3(0f, 84f, 0f) : new Vector3(0f, -56f, 0f);
            btnBar.transform.DOLocalMove(targetBarPos, 0.5f).SetEase(Ease.OutCubic);

            // btnHandle 크기 변경
            Vector3 targetHandleScale = isBtnExpanded ? Vector3.one * 0.5f : Vector3.one;
            btnHandle.transform.DOScale(targetHandleScale, 0.5f).SetEase(Ease.OutCubic);
        }

        private void PrintBoard()
        {
            Debug.Log("=== Slot Board ===");
            for (int y = 0; y < _slotBoard.RowCount; y++)
            {
                if (_slotBoard.Rows[y])
                {
                    string rowStr = "";
                    for (int x = 0; x < _slotBoard.ColumnCount; x++)
                    {
                        var symbol = _slotBoard.Get(x, y);
                        rowStr += symbol+ " ";
                    }
                    Debug.Log(rowStr);
                }
            }
        }
    }
}