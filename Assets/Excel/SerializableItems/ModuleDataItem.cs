using System;

namespace EmergencyRoulette
{
    // 모듈이 적용되는 대상 (행, 열, 전체)
    public enum ModuleTarget
    {
        Row,
        Column,
        Global
        //NotRow,           // 해당 행 제외
        //NotColumn,        // 해당 열 제외
        //AdjacentRow,      // 상하 인접 행
        //AdjacentColumn    // 좌우 인접 열
    }

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

        public ModuleTarget targetType;   // 대상 타입 (행, 열, 전체)
        public int targetIndex;           // 대상 인덱스 (0부터 시작)

        public ModuleEffectType effectType; // 효과 종류
        public string targetResource;       // 영향을 주는 자원 이름
        public float effectValue;           // 수치 (확률 변화량, 보너스 값 등)
    }
}
