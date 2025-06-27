namespace EmergencyRoulette
{
    public enum SymbolType
    {
        Energy,
        Medical,
        Food,
        Data,
        Warning,
        Discharge,
        Outdated
    }
    
    public enum SymbolCategory
    {
        Resource,
        Danger
    }
    
    public class SymbolInfo
    {
        public SymbolType type;
        public SymbolCategory category;
        public float probability;

        public SymbolInfo(SymbolType type, SymbolCategory category, float probability)
        {
            this.type = type;
            this.category = category;
            this.probability = probability;
        }
    }
}