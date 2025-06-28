using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmergencyRoulette
{
    public class SlotBoard
    {
        private Dictionary<(int x, int y), SymbolType> _grid = new();
        public Dictionary<(int x, int y), SymbolType> Grid => _grid;
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        
        private SymbolPicker _picker;

        public SlotBoard(SymbolPicker picker, int initialRows = 3, int columns = 3)
        {
            _picker = picker;
            RowCount = initialRows;
            ColumnCount = columns;

            for (int y = 0; y < RowCount; y++)
            {
                for (int x = 0; x < ColumnCount; x++)
                {
                    _grid[(x, y)] = _picker.Pick();
                }
            }
        }
        
        public void Spin()
        {
            var keys = _grid.Keys.ToList();
            foreach (var key in keys)
            {
                _grid[key] = _picker.Pick();
            }
        }
        
        // 수정해야할지도
        public void AddRow(int columnCount)
        {
            int newY = RowCount;
            for (int x = 0; x < ColumnCount; x++)
            {
                _grid[(x, newY)] = _picker.Pick();
            }
            RowCount++;
        }
        
        public SymbolType Get(int x, int y) => _grid[(x, y)];
        public void Set(int x, int y, SymbolType value) => _grid[(x, y)] = value;
    }
}