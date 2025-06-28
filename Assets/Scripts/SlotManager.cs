using UnityEngine;

namespace EmergencyRoulette
{
    public class SlotManager : MonoBehaviour
    {
        private SymbolLibrary _symbolLibrary;
        private SymbolPicker _symbolPicker;
        private PlayerResource _playerResource;
        private SlotBoard _slotBoard;

        public SlotColumnScroller Slot0;

        public void Start()
        {
            _symbolLibrary = new SymbolLibrary();
            _symbolPicker = new SymbolPicker(_symbolLibrary);
            _playerResource = new PlayerResource();
            _slotBoard = new SlotBoard(_symbolPicker);
            
            PrintBoard();
        }
        
        public void StartSpin()
        {
            _slotBoard.Spin();
            StartCoroutine(Slot0.StartSpin(_slotBoard.Get(0,0)));
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