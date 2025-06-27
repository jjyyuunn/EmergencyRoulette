using System;

namespace EmergencyRoulette
{
    // ����� ����Ǵ� ��� (��, ��, ��ü)
    public enum ModuleTarget
    {
        Row,
        Column,
        Global
        //NotRow,           // �ش� �� ����
        //NotColumn,        // �ش� �� ����
        //AdjacentRow,      // ���� ���� ��
        //AdjacentColumn    // �¿� ���� ��
    }

    // ����� ������ ȿ�� Ÿ��
    public enum ModuleEffectType
    {
        IncreaseChance,      // Ư�� �ڿ� Ȯ�� ����
        DecreaseChance,      // Ư�� �ڿ� Ȯ�� ����
        IgnoreResult,        // Ư�� �ڿ� ȿ�� ����
        BonusOnResult,       // Ư�� �ڿ� ���� �� �߰� ����
        RerollOnResult,      // Ư�� �ڿ��� ���� ��� �籼��
        PredictProbability   // ���� Ȯ�� �̸����� (UI �����)
    }

    // ��� �� ���� ���� (�����κ��� �Ľ̵�)
    [System.Serializable]
    public class ModuleDataItem
    {
        public string moduleName;         // ��� �̸�
        public string description;        // ��� ����

        public ModuleTarget targetType;   // ��� Ÿ�� (��, ��, ��ü)
        public int targetIndex;           // ��� �ε��� (0���� ����)

        public ModuleEffectType effectType; // ȿ�� ����
        public string targetResource;       // ������ �ִ� �ڿ� �̸�
        public float effectValue;           // ��ġ (Ȯ�� ��ȭ��, ���ʽ� �� ��)
    }
}
