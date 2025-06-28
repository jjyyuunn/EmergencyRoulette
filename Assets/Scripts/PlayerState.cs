using System.Collections.Generic;
using UnityEngine;
using System;

namespace EmergencyRoulette
{
    public enum EmergencyLevel
    {
        Safe = 0,           // 0% ~ 30%
        Warning = 30,        // 30% ~ 50%
        Danger = 50,         // 50% ~ 70%
        Severe = 70,         // 70% ~ 90%
        Crisis = 90         // 90% ~ 100%
    }
    
    public class PlayerState
    {
        public int Energy;
        public int Food;
        public int Technology;
        public int Data;

        public float OverloadGauge; // 0 ~ 100%
        public EmergencyLevel EmergencyLevel;

        private int _useFood => (int)Math.Ceiling(0.3f * _slotBoard.RowCount);
        private int _useFoodBonus = 0;

        private SlotBoard _slotBoard;
        private Dictionary<SymbolType, int> _gainedSymbols;
        private bool _dismissPenaltyCombo;
        public bool DataCombo; // 처리 후 다시 false로 바꿔주시와요..
        private int _nextTurnTechBonus;

        public PlayerState()
        {
            Energy = 0;
            Food = 0;
            Technology = 0;
            Data = 0;
            OverloadGauge = 0f;
            EmergencyLevel = EmergencyLevel.Safe;
        }

        public void SetPlayerState(SlotBoard slotBoard)
        {
            _slotBoard = slotBoard;
            _gainedSymbols = slotBoard.GainedSymbols;
            Technology += _nextTurnTechBonus; // 콤보 보너스 적용
            _nextTurnTechBonus = 0;
            
            // 순서대로
            SetResourceCombo();
            SetSpecialCombo();
            SetNormalState();
        }
        
        // 자원 콤보
        private void SetResourceCombo()
        {
            List<(int y, SymbolType symbol)> combos = _slotBoard.CheckCombos();
            for (int y = 0; y < combos.Count; y++)
            {
                switch (combos[y].symbol)
                {
                    case SymbolType.Food:
                        _useFoodBonus++;
                        break;
                    case SymbolType.Energy:
                        OverloadGauge -= 10f;
                        break;
                    case SymbolType.Technology:
                        _dismissPenaltyCombo = true;
                        break;
                    case SymbolType.Data:
                        DataCombo = true;
                        break;
                }
            }
        }

        // 특수 콤보
        private void SetSpecialCombo()
        {
            for (int y = 0; y < _slotBoard.RowCount; y++)
            {
                Dictionary<SymbolType, int> rowSymbols = new();
                for (int x = 0; x < _slotBoard.ColumnCount; x++)
                {
                    var symbol = _slotBoard.Get(x, y);
                    if (!rowSymbols.ContainsKey(symbol))
                        rowSymbols[symbol] = 0;
                    rowSymbols[symbol]++;
                }

                CheckCrisisProtocol(rowSymbols);
                CheckEmergencyChargeUnit(rowSymbols);
                CheckFailureToSuccess(rowSymbols);
            }
        }
        
        private void CheckCrisisProtocol(Dictionary<SymbolType, int> rowSymbols)
        {
            if (rowSymbols.TryGetValue(SymbolType.Warning, out int warn) && warn == 1 &&
                rowSymbols.TryGetValue(SymbolType.Discharge, out int discharge) && discharge == 1 &&
                rowSymbols.TryGetValue(SymbolType.Outdated, out int outdated) && outdated == 1)
            {
                OverloadGauge = Mathf.Max(0, OverloadGauge - 10f); // 얼만큼 감소?
                Food += 1;
                Technology += 1;
                Energy += 1;
                Data += 1;
                Debug.Log("[Combo] 위기 전환 프로토콜 발동!");
            }
        }
        
        private void CheckEmergencyChargeUnit(Dictionary<SymbolType, int> rowSymbols)
        {
            if (rowSymbols.TryGetValue(SymbolType.Energy, out int energy) && energy == 1 &&
                rowSymbols.TryGetValue(SymbolType.Discharge, out int discharge) && discharge == 2)
            {
                if (Energy == 0)
                    Energy += 3;
                else
                    Energy += 2;

                Debug.Log("[Combo] 응급 충전 유닛 발동!");
            }
        }
        
        private void CheckFailureToSuccess(Dictionary<SymbolType, int> rowSymbols)
        {
            if (rowSymbols.TryGetValue(SymbolType.Technology, out int tech) && tech == 1 &&
                rowSymbols.TryGetValue(SymbolType.Outdated, out int outdated) && outdated == 2)
            {
                _nextTurnTechBonus = 4;
                Debug.Log("[Combo] 실패는 성공의 어머니 발동! 다음 턴 기술 +4");
            }
        }



        // 기본 심볼 생산
        private void SetNormalState()
        {
            Energy += _gainedSymbols[SymbolType.Energy];
            Technology += _gainedSymbols[SymbolType.Technology];
            Food += _gainedSymbols[SymbolType.Food];
            Data += _gainedSymbols[SymbolType.Data];
            
            CheckWarning();
            CheckDischarge();
            CheckOutdated();

            EmergencyLevel = GetEmergencyLevel();
            
            Debug.Log($"[PlayerResource] Set values - Energy: {Energy}, Technology: {Technology}, Food: {Food}, Data: {Data}");
            Debug.Log($"[PlayerResource] OverloadGauge: {OverloadGauge}");
            Debug.Log($"[PlayerResource] EmergencyLevel: {EmergencyLevel}");
        }

        private void CheckWarning()
        {
            OverloadGauge += _gainedSymbols[SymbolType.Warning] * 5f;

            switch (EmergencyLevel)
            {
                case EmergencyLevel.Warning:
                    OverloadGauge += 2f;
                    break;
                case EmergencyLevel.Danger:
                    OverloadGauge += 3f;
                    break;
                case EmergencyLevel.Severe:
                    OverloadGauge += 5f;
                    break;
                case EmergencyLevel.Crisis:
                    OverloadGauge += 10f;
                    break;
                default:
                    break;
            }
        }

        private void CheckDischarge()
        {
            int decreasingEnergy = 1;
            
            switch (EmergencyLevel)
            {
                case EmergencyLevel.Danger:
                case EmergencyLevel.Warning:
                case EmergencyLevel.Crisis:
                    decreasingEnergy++;
                    break;
                default:
                    break;
            }
            decreasingEnergy *= _gainedSymbols[SymbolType.Discharge];
            
            if (Energy >= decreasingEnergy)
                Energy -= decreasingEnergy;
            else
            {
                OverloadGauge += _gainedSymbols[SymbolType.Discharge] * 10f;
                switch (EmergencyLevel)
                {
                    case EmergencyLevel.Warning:
                        OverloadGauge += 2f;
                        break;
                    case EmergencyLevel.Danger:
                        OverloadGauge += 3f;
                        break;
                    case EmergencyLevel.Severe:
                        OverloadGauge += 5f;
                        break;
                    case EmergencyLevel.Crisis:
                        OverloadGauge += 10f;
                        break;
                    default:
                        break;
                }
            }

        }
        
        private void CheckOutdated()
        {
            int decreasingTechnology = 1;
            
            switch (EmergencyLevel)
            {
                case EmergencyLevel.Danger:
                case EmergencyLevel.Warning:
                case EmergencyLevel.Crisis:
                    decreasingTechnology++;
                    break;
                default:
                    break;
            }
            decreasingTechnology *= _gainedSymbols[SymbolType.Outdated];
            
            if (Technology >= decreasingTechnology)
                Technology -= decreasingTechnology;
            else
            {
                OverloadGauge += _gainedSymbols[SymbolType.Outdated] * 10f;
                switch (EmergencyLevel)
                {
                    case EmergencyLevel.Warning:
                        OverloadGauge += 2f;
                        break;
                    case EmergencyLevel.Danger:
                        OverloadGauge += 3f;
                        break;
                    case EmergencyLevel.Severe:
                        OverloadGauge += 5f;
                        break;
                    case EmergencyLevel.Crisis:
                        OverloadGauge += 10f;
                        break;
                    default:
                        break;
                }
            }
        }
        
        private EmergencyLevel GetEmergencyLevel()
        {
            if (OverloadGauge < 30f) return EmergencyLevel.Safe;
            if (OverloadGauge < 50f) return EmergencyLevel.Warning;
            if (OverloadGauge < 70f) return EmergencyLevel.Danger;
            if (OverloadGauge < 90f) return EmergencyLevel.Severe;
            return EmergencyLevel.Crisis;
        }
    }
}