using System.Collections.Generic;

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
    
    public class PlayerResource
    {
        public int Energy;
        public int Food;
        public int Medical;
        public int Data;

        public float OverloadGauge; // 0 ~ 100%
        public EmergencyLevel EmergencyLevel;

        public PlayerResource()
        {
            Energy = 0;
            Food = 0;
            Medical = 0;
            Data = 0;
            OverloadGauge = 0f;
            EmergencyLevel = EmergencyLevel.Safe;
        }

        public void SetPlayerResource(Dictionary<SymbolType, int> gainedSymbols)
        {
            Energy = gainedSymbols[SymbolType.Energy];
            Medical = gainedSymbols[SymbolType.Medical];
            Food = gainedSymbols[SymbolType.Food];
            Data = gainedSymbols[SymbolType.Data];
            
            CheckWarning();

            EmergencyLevel = GetEmergencyLevel();
        }

        private void CheckWarning()
        {
            OverloadGauge += 5f;

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