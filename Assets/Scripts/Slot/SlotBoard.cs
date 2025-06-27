using System.Collections.Generic;
using UnityEngine;

namespace EmergencyRoulette.Slot
{
    public class SlotBoard
    {
        public List<List<SymbolType>> Grid { get; private set; } = new List<List<SymbolType>>();
        private SymbolPicker _picker;

        public SlotBoard(SymbolPicker picker, int initialRows = 3, int columns = 3)
        {
            _picker = picker;
            for (int i = 0; i < initialRows; i++)
            {
                Grid.Add(GenerateRow(columns));
            }
        }
        
        public void Spin()
        {
            for (int row = 0; row < Grid.Count; row++)
            {
                for (int col = 0; col < Grid[row].Count; col++)
                {
                    Grid[row][col] = _picker.Pick();
                }
            }
        }
        
        public void AddRow(int columnCount)
        {
            Grid.Add(GenerateRow(columnCount));
        }

        private List<SymbolType> GenerateRow(int columnCount)
        {
            var row = new List<SymbolType>();
            for (int i = 0; i < columnCount; i++)
            {
                row.Add(_picker.Pick());
            }
            return row;
        }

        public int RowCount => Grid.Count;
        public int ColumnCount => Grid[0].Count;
    }
}