using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System;

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
            _symbolLibrary = new SymbolLibrary();
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
                }
            }
        }

        public void StartSpin()
        {
            if (GameManager.Instance.CurrentState != GameManager.GameState.Spin)
                return;

            if (HasSpunThisTurn)
                return;
            
            _slotBoard.ResetGainedSymbols(); // 심볼 리셋
            _slotBoard.Spin();
            HasSpunThisTurn = true;
            
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
            GameManager.Instance.SetState(GameManager.GameState.Resolving);
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
            foreach (var slot in slotInstances)
            {
                slot.GetComponent<SlotParticleEffect>()?.PlayPending();
            }
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