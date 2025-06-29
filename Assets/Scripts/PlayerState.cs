using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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

        private float _overloadGauge;
        public float OverloadGauge // 0 ~ 100%
        {
            get => _overloadGauge;
            set => _overloadGauge = Mathf.Clamp(value, 0f, 100f);
        }
        public EmergencyLevel EmergencyLevel;

        // 하루 소모 자원
        private int _useFood => (int)Math.Ceiling(0.3f * _slotBoard.RowCount);
        public int UseFoodBonus = 0;

        private SlotBoard _slotBoard;
        private Dictionary<SymbolType, int> _gainedSymbols;

        private bool _dismissPenaltyCombo;
        private int _nextTurnTechBonus; // 실패는 성공의 어머니
        private bool _noEnergyProduction; // 정전

        public bool DataCombo { get; set; }  // 처리 후 다시 false로 바꿔주시와요..
        public bool DoubleNextActive { get; set; } // 모듈 오버클럭

        public PlayerState()
        {
            Energy = 0;
            Food = 0;
            Technology = 0;
            Data = 0;
            OverloadGauge = 0f;
            EmergencyLevel = EmergencyLevel.Safe;
        }

        public void SetInternalData(SlotBoard slotBoard)
        {
            _slotBoard = slotBoard;
            _gainedSymbols = slotBoard.GainedSymbols;
        }

        // 자원 콤보
        public void SetResourceCombo()
        {
            List<(int y, SymbolType symbol)> combos = _slotBoard.CheckCombos();
            for (int y = 0; y < combos.Count; y++)
            {
                switch (combos[y].symbol)
                {
                    case SymbolType.Food:
                        UseFoodBonus--; // 여기 마이너스 or 플러스?
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
                Debug.Log("Resource combo activated.");
            }
        }

        // 특수 콤보
        public void SetSpecialCombo()
        {
            bool crisisOn = HasComboModule("AddCombo_WarningDischargeDecay");
            bool chargeOn = HasComboModule("AddCombo_EnergyDischargeDischarge");
            bool failOn = HasComboModule("AddCombo_TechDecayDecay");

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

                if (crisisOn)
                    CheckCrisisProtocol(rowSymbols);

                if (chargeOn)
                    CheckEmergencyChargeUnit(rowSymbols);

                if (failOn)
                    CheckFailureToSuccess(rowSymbols);
            }
        }


        private bool HasComboModule(string effectKey)
        {
            foreach (var equip in ModuleManager.Instance.equippedModules)
            {
                if (equip.module.useType != ModuleUseType.Combo)
                    continue;

                if (equip.module.effectKey != effectKey)
                    continue;

                int row = equip.index;

                if (ModuleManager.Instance.IsModuleBroken(row))
                {
                    Debug.Log($"[ComboCheck] Row {row} 고장 → 콤보 모듈 '{effectKey}' 무시됨");
                    continue;
                }

                return true;
            }

            return false;
        }


        private void CheckCrisisProtocol(Dictionary<SymbolType, int> rowSymbols)
        {
            if (rowSymbols.TryGetValue(SymbolType.Warning, out int warn) && warn == 1 &&
                rowSymbols.TryGetValue(SymbolType.Discharge, out int discharge) && discharge == 1 &&
                rowSymbols.TryGetValue(SymbolType.Outdated, out int outdated) && outdated == 1)
            {
                OverloadGauge -= 20f;
                
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
        
        // 패널티 콤보
        public void SetPenaltyCombos()
        {
            var penaltyCombos = _slotBoard.GetPenaltyCombos();

            foreach (var combo in penaltyCombos)
            {
                switch (combo.symbol)
                {
                    case SymbolType.Warning: // 경고 연쇄
                        OverloadGauge += 5f;
                        EmergencyLevel = GetEmergencyLevel();
                        Debug.Log($"[PenaltyCombo] Warning Chain on row {combo.y} → +5% OverloadGauge");
                        break;

                    case SymbolType.Discharge: // 정전
                        _noEnergyProduction = true;
                        Debug.Log("[PenaltyCombo] NO ENERGY! ON NEXT TURN.");
                        break;
                    
                    case SymbolType.Outdated: // 붕괴 별도 구현
                        GameManager.SlotManager.LockRow(combo.y); // 슬롯 잠금
                        Debug.Log($"[PenaltyCombo] Collapse on row {combo.y} → Row locked next turn");
                        break;
                }
            }
            CheckEnergyLeakage();
        }

        private void CheckEnergyLeakage()
        {
            List<int> leakRows = new();

            for (int y = 0; y < _slotBoard.RowCount; y++)
            {
                if (!_slotBoard.Rows[y]) continue; // 잠긴 행 제외

                int discharge = 0;
                int outdated = 0;

                for (int x = 0; x < _slotBoard.ColumnCount; x++)
                {
                    var symbol = _slotBoard.Get(x, y);
                    if (symbol == SymbolType.Discharge) discharge++;
                    if (symbol == SymbolType.Outdated) outdated++;
                }

                if (discharge >= 2 && outdated >= 1)
                    leakRows.Add(y);
            }

            if (leakRows.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, leakRows.Count);
                int selectedRow = leakRows[randomIndex];

                ModuleManager.Instance.SetModuleBroken(selectedRow);
                Debug.Log($"[SpecialPenaltyCombo] Energy Leak → Row {selectedRow} module broken");
            }
        }


        private int CountGainDataOnDangerModules()
        {
            int count = 0;

            foreach (var equip in ModuleManager.Instance.equippedModules)
            {
                if (equip.module.useType != ModuleUseType.Passive)
                    continue;

                if (equip.module.effectKey != "GainDataIfMultipleDanger")
                    continue;

                int row = equip.index;

                if (ModuleManager.Instance.IsModuleBroken(row))
                {
                    Debug.Log($"[PassiveEffect:GainDataIfMultipleDanger] Row {row} 고장 → 적용 안 됨");
                    continue;
                }

                count++;
            }

            return count;
        }



        // FIXME: 기본 심볼 생산
        public void SetNormalState()
        {
            Energy += _gainedSymbols[SymbolType.Energy];
            Technology += _gainedSymbols[SymbolType.Technology];
            Food += _gainedSymbols[SymbolType.Food];
            Data += _gainedSymbols[SymbolType.Data];

            int dangerCount =
                _gainedSymbols[SymbolType.Warning] +
                _gainedSymbols[SymbolType.Discharge] +
                _gainedSymbols[SymbolType.Outdated];

            if (dangerCount >= 2)
            {
                Data += 2 * CountGainDataOnDangerModules();
                Debug.Log("[PassiveEffect] 위험 심볼 2개 이상 → 데이터 +2 발동");
            }

            CheckWarning();
            CheckDischarge();
            CheckOutdated();

            EmergencyLevel = GetEmergencyLevel();
            
            Debug.Log($"[PlayerResource] Set values - Energy: {Energy}, Technology: {Technology}, Food: {Food}, Data: {Data}");
            Debug.Log($"[PlayerResource] OverloadGauge: {OverloadGauge}");
            Debug.Log($"[PlayerResource] EmergencyLevel: {EmergencyLevel}");
        }

        private int CountIgnoreWarningModules()
        {
            int count = 0;

            foreach (var equip in ModuleManager.Instance.equippedModules)
            {
                if (equip.module.useType != ModuleUseType.Passive)
                    continue;

                if (equip.module.effectKey != "IgnoreWarningWithEnergy")
                    continue;

                int row = equip.index;

                if (ModuleManager.Instance.IsModuleBroken(row))
                {
                    Debug.Log($"[PassiveEffect:IgnoreWarning] Row {row} 고장 → 무시됨");
                    continue;
                }

                count++;
            }

            return count;
        }




        private void CheckWarning()
        {
            int ignoreCount = CountIgnoreWarningModules();

            if (ignoreCount > 0)
            {
                if (ignoreCount <= 0 || Energy <= 0)
                    return;

                int usableCount = Mathf.Min(ignoreCount, Energy); // 에너지가 부족하면 그만큼만
                Energy -= usableCount;

                Debug.Log($"[WarningCheck] 경고 무효화 모듈 {ignoreCount}개 중 {usableCount}개 작동 → 에너지 {usableCount} 소모, 경고 {usableCount}개 무효화");
            }

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
            Debug.Log($"[PlayerResource] Discharge value: {decreasingEnergy}");
            
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
            Debug.Log($"[PlayerResource] Outdated value: {decreasingTechnology}");
            
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

        public SlotBoard SlotBoard => _slotBoard;


        public void PrintResources()
        {
            Debug.Log($"[Resource] Energy: {Energy}, Tech: {Technology}, Food: {Food}, Data: {Data}, Overload: {OverloadGauge}");
        }

    }
}