using System.Collections.Generic;
using UnityEngine;

namespace EmergencyRoulette
{
    public class SlotManager : MonoBehaviour
    {
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform slotParent;

        private SlotColumnScroller[,] slotInstances;

        private SymbolLibrary _symbolLibrary;
        private SymbolPicker _symbolPicker;
        private SlotBoard _slotBoard;
        
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

                    RectTransform rt = slotObj.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(x * 120f, -y * 120f);
                }
            }
        }

        public void StartSpin()
        {
            _slotBoard.Spin();

            for (int x = 0; x < _slotBoard.ColumnCount; x++)
            {
                for (int y = 0; y < _slotBoard.RowCount; y++)
                {
                    SymbolType symbol = _slotBoard.Get(x, y);
                    slotInstances[x, y].StartCoroutine(slotInstances[x, y].StartSpin(symbol));
                }
            }

            PrintBoard();
            AddResource();
        }


        // 플레이어 리소스 더하기
        private void AddResource()
        {
            GameManager.Instance.PlayerResource.SetPlayerResource(_slotBoard.GainedSymbols);
            _slotBoard.ResetGainedSymbols(); // 다 적용 후 리셋
        }
        
        private void PrintBoard()
        {
            Debug.Log("=== Slot Board ===");
            for (int y = 0; y < _slotBoard.RowCount; y++)
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