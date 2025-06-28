using System;

namespace EmergencyRoulette
{
    public enum ModuleUseType
    {
        Passive,    // 매 턴 자동 발동
        Active,     // 유저가 에너지를 소비해 사용
        Combo       // 특정 심볼 조합 시 발동
    }

    [Serializable]
    public class ModuleDataItem
    {
        public string moduleName;             // 이름
        public string description;            // 설명

        public int purchaseCost;              // 데이터 기반 구매 비용
        public ModuleUseType useType;         // 사용 방식

        public int energyCost;                // Active 타입일 경우 에너지 소비량 (Passive/Combo일 경우 무시)

        public string effectKey;              // 효과 식별자 (내부 로직 연결용 키워드 또는 enum 이름)
    }
}
