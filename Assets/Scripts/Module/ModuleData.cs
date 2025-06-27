using UnityEngine;

public enum ModuleTarget
{
    Row,              // �ش� ��
    Column,           // �ش� ��
    Global,           // ��ü ����
    //NotRow,           // �ش� �� ����
    //NotColumn,        // �ش� �� ����
    //AdjacentRow,      // ���� ���� ��
    //AdjacentColumn    // �¿� ���� ��
}

public enum ModuleEffectType
{
    IncreaseChance,      // Ư�� �ڿ� Ȯ�� ����
    DecreaseChance,      // Ư�� �ڿ� Ȯ�� ����
    IgnoreResult,        // Ư�� �ڿ� ȿ�� ����
    BonusOnResult,       // Ư�� �ڿ� ���� �� �߰� ����
    RerollOnResult,      // Ư�� �ڿ��� �߸� �籼��
    PredictProbability   // �ڿ� ���� Ȯ�� �̸� ����
}

[CreateAssetMenu(fileName = "NewModuleData", menuName = "SpinSurvival/Module")]
public class ModuleData : ScriptableObject
{
    [Header("�⺻ ����")]
    public string moduleName;
    [TextArea]
    public string description;

    [Header("���� ��� ����")]
    public ModuleTarget targetType;    // Row / Column / Global ��
    public int targetIndex;            // ��/�� ��ȣ (0���� ����), Global�̸� -1

    [Header("ȿ�� ����")]
    public ModuleEffectType effectType;
    public string targetResource;      // "������", "�糭" �� �ڿ� �̸� (�ؽ�Ʈ ���)
    public float effectValue;          // Ȯ�� ��ȭ ��ġ (ex. 0.1 = +10%), ���ʽ� ���� ��
}
