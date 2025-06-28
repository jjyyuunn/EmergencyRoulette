using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmergencyRoulette
{
    public class SlotBoard
    {
        private Dictionary<(int x, int y), SymbolType> _grid = new();
        public Dictionary<(int x, int y), SymbolType> Grid => _grid;
        public List<bool> Rows;
        
        public readonly int RowCount = 5;
        public readonly int ColumnCount = 3;
        public Dictionary<SymbolType, int> GainedSymbols { get; private set; }
        
        private SymbolPicker _picker;

        public SlotBoard(SymbolPicker picker)
        {
            _picker = picker;
            GainedSymbols = new Dictionary<SymbolType, int>()
            {
                { SymbolType.Energy, 0 },
                { SymbolType.Technology, 0 },
                { SymbolType.Food, 0 },
                { SymbolType.Data, 0 },
                { SymbolType.Warning, 0 },
                { SymbolType.Discharge, 0 },
                { SymbolType.Outdated, 0 },
            };
            Rows = new(RowCount);

            for (int y = 0; y < RowCount; y++)
            {
                for (int x = 0; x < ColumnCount; x++)
                {
                    _grid[(x, y)] = _picker.Pick();
                }

                if (y < 3)
                {
                    Rows.Add(true);
                }
                else
                {
                    Rows.Add(false);
                }
            }
        }
        
        public void Spin()
        {
            var keys = _grid.Keys.ToList();
            foreach (var key in keys)
            {
                if (Rows[key.y])
                {
                    _grid[key] = _picker.Pick();
                    GainedSymbols[_grid[key]]++;
                }
            }
        }
        
        public void ResetGainedSymbols()
        {
            var keys = GainedSymbols.Keys.ToList();
            foreach (var key in keys)
            {
                GainedSymbols[key] = 0;
            }
        }

        public void LockRow(int y)
        {
            Rows[y] = false;
        }

        public void UnlockRow(int y)
        {
            Rows[y] = true;
        }
        
        public SymbolType Get(int x, int y) => _grid[(x, y)];
        public void Set(int x, int y, SymbolType value) => _grid[(x, y)] = value;
        
        public List<(int y, SymbolType symbol)> CheckCombos()
        {
            List<(int y, SymbolType symbol)> combos = new();

            for (int y = 0; y < RowCount; y++)
            {
                if (Rows[y])
                {
                    var rowSymbols = new List<SymbolType>();
                    for (int x = 0; x < ColumnCount; x++)
                    {
                        rowSymbols.Add(_grid[(x, y)]);
                    }

                    // 모든 SymbolType이 같다면 콤보
                    if (rowSymbols.All(s => s == rowSymbols[0]))
                    {
                        combos.Add((y, rowSymbols[0]));
                    }
                }
            }

            return combos;
        }
        
        public List<(int y, SymbolType symbol)> GetPenaltyCombos()
        {
            var penaltyCombos = new List<(int y, SymbolType symbol)>();
            for (int y = 0; y < RowCount; y++)
            {
                if (Rows[y])
                {
                    var rowSymbols = new List<SymbolType>();
                    for (int x = 0; x < ColumnCount; x++)
                    {
                        rowSymbols.Add(Get(x, y));
                    }

                    // 위험 심볼만
                    var dangerSymbols = rowSymbols
                        .Where(s => s == SymbolType.Warning || s == SymbolType.Discharge || s == SymbolType.Outdated)
                        .ToList();

                    if (dangerSymbols.Count >= 3 && dangerSymbols.Distinct().Count() == 1) // 정확히 같은 심볼로 3개
                    {
                        penaltyCombos.Add((y, dangerSymbols.First()));
                    }
                }
            }

            return penaltyCombos;
        }

    }
}