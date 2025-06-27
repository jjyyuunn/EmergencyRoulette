using UnityEngine;

public enum ModuleTarget
{
    Row,              // 해당 행
    Column,           // 해당 열
    Global,           // 전체 슬롯
    //NotRow,           // 해당 행 제외
    //NotColumn,        // 해당 열 제외
    //AdjacentRow,      // 상하 인접 행
    //AdjacentColumn    // 좌우 인접 열
}

public enum ModuleEffectType
{
    IncreaseChance,      // 특정 자원 확률 증가
    DecreaseChance,      // 특정 자원 확률 감소
    IgnoreResult,        // 특정 자원 효과 무시
    BonusOnResult,       // 특정 자원 등장 시 추가 보상
    RerollOnResult,      // 특정 자원이 뜨면 재굴림
    PredictProbability   // 자원 등장 확률 미리 보기
}

[CreateAssetMenu(fileName = "NewModuleData", menuName = "SpinSurvival/Module")]
public class ModuleData : ScriptableObject
{
    [Header("기본 정보")]
    public string moduleName;
    [TextArea]
    public string description;

    [Header("적용 대상 설정")]
    public ModuleTarget targetType;    // Row / Column / Global 등
    public int targetIndex;            // 행/열 번호 (0부터 시작), Global이면 -1

    [Header("효과 설정")]
    public ModuleEffectType effectType;
    public string targetResource;      // "에너지", "재난" 등 자원 이름 (텍스트 기반)
    public float effectValue;          // 확률 변화 수치 (ex. 0.1 = +10%), 보너스 수량 등
}
