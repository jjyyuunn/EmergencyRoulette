using System.Collections.Generic;

namespace EmergencyRoulette
{
    public class SymbolLibrary
    {
        private Dictionary<SymbolType, SymbolInfo> _symbolLibrary = new();

        public SymbolLibrary()
        {
            // 기본 확률 지정
            _symbolLibrary[SymbolType.Energy] = new SymbolInfo(SymbolType.Energy, SymbolCategory.Resource, 0.3f);
            _symbolLibrary[SymbolType.Technology] = new SymbolInfo(SymbolType.Technology, SymbolCategory.Resource, 0.15f);
            _symbolLibrary[SymbolType.Food] = new SymbolInfo(SymbolType.Food, SymbolCategory.Resource, 0.15f);
            _symbolLibrary[SymbolType.Data] = new SymbolInfo(SymbolType.Data, SymbolCategory.Resource, 0.15f);
            _symbolLibrary[SymbolType.Warning] = new SymbolInfo(SymbolType.Warning, SymbolCategory.Danger, 0.15f);
            _symbolLibrary[SymbolType.Discharge] = new SymbolInfo(SymbolType.Discharge, SymbolCategory.Danger, 0.05f);
            _symbolLibrary[SymbolType.Outdated] = new SymbolInfo(SymbolType.Outdated, SymbolCategory.Danger, 0.05f);
        }
        
        public Dictionary<SymbolType, SymbolInfo> GetAll()
        {
            return _symbolLibrary;
        }

        public void SetProbability(SymbolType type, int energy)
        {
            //소모한 에너지 개수 만큼 가중치 추가
            _symbolLibrary[type].probability += energy*0.01f ;
        }

        public void MultiplyProbability(SymbolType type, float weight)
        {
            _symbolLibrary[type].probability *= weight;
        }
    }
}