using System.Collections.Generic;

namespace EmergencyRoulette
{
    public class SymbolLibrary
    {
        private Dictionary<SymbolType, SymbolInfo> _symbolLibrary = new();

        public SymbolLibrary()
        {
            // 확률 수정
            _symbolLibrary[SymbolType.Energy] = new SymbolInfo(SymbolType.Energy, SymbolCategory.Resource, 0.5f);
            _symbolLibrary[SymbolType.Medical] = new SymbolInfo(SymbolType.Medical, SymbolCategory.Resource, 0.5f);
            _symbolLibrary[SymbolType.Food] = new SymbolInfo(SymbolType.Food, SymbolCategory.Resource, 0.5f);
            _symbolLibrary[SymbolType.Data] = new SymbolInfo(SymbolType.Data, SymbolCategory.Resource, 0.5f);
            _symbolLibrary[SymbolType.Warning] = new SymbolInfo(SymbolType.Warning, SymbolCategory.Danger, 0.5f);
            _symbolLibrary[SymbolType.Discharge] = new SymbolInfo(SymbolType.Discharge, SymbolCategory.Danger, 0.5f);
            _symbolLibrary[SymbolType.Outdated] = new SymbolInfo(SymbolType.Outdated, SymbolCategory.Danger, 0.5f);
        }
        
        public Dictionary<SymbolType, SymbolInfo> GetAll()
        {
            return _symbolLibrary;
        }

        public void SetProbability(SymbolType type, float prob)
        {
            _symbolLibrary[type].probability = prob;
        }
    }
}