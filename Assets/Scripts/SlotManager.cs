using UnityEngine;

namespace EmergencyRoulette
{
    public class SlotManager : MonoBehaviour
    {
        private SymbolLibrary _symbolLibrary;
        private SymbolPicker _symbolPicker;
        private SlotBoard _slotBoard;
        
        public SlotColumnScroller Slot0;

        public void Start()
        {
            _symbolLibrary = new SymbolLibrary();
            _symbolPicker = new SymbolPicker(_symbolLibrary);
            _slotBoard = new SlotBoard(_symbolPicker);
            PrintBoard();
        }
        
        public void StartSpin()
        {
            _slotBoard.Spin();
            StartCoroutine(Slot0.StartSpin(_slotBoard.Get(0,0)));
            
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