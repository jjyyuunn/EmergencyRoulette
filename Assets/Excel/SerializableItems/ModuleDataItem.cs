using System;

namespace EmergencyRoulette
{
    // 모듈이 가지는 효과 타입
    public enum ModuleEffectType
    {
        IncreaseChance,      // 특정 자원 확률 증가
        DecreaseChance,      // 특정 자원 확률 감소
        IgnoreResult,        // 특정 자원 효과 무시
        BonusOnResult,       // 특정 자원 등장 시 추가 보상
        RerollOnResult,      // 특정 자원이 나올 경우 재굴림
        PredictProbability   // 등장 확률 미리보기 (UI 연출용)
    }

    // 모듈 한 개의 정보 (엑셀로부터 파싱됨)
    [System.Serializable]
    public class ModuleDataItem
    {
        public string moduleName;         // 모듈 이름
        public string description;        // 모듈 설명

        public ModuleEffectType effectType; // 효과 종류
        public string targetResource;       // 영향을 주는 자원 이름
        public float effectValue;           // 수치 (확률 변화량, 보너스 값 등)
    }
}
