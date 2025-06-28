using System.Collections.Generic;
using UnityEngine;

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

        private Dictionary<SymbolType, int> _gainedSymbols;

        public PlayerState()
        {
            Energy = 0;
            Food = 0;
            Technology = 0;
            Data = 0;
            OverloadGauge = 0f;
            EmergencyLevel = EmergencyLevel.Safe;
        }

        public void SetPlayerState(Dictionary<SymbolType, int> gainedSymbols)
        {
            _gainedSymbols = gainedSymbols;
            
            SetNormalResource();
        }

        // 기본 심볼 생산
        private void SetNormalResource()
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