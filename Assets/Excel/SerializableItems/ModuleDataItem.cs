using System;

namespace EmergencyRoulette
{
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

        public ModuleEffectType effectType; // ȿ�� ����
        public string targetResource;       // ������ �ִ� �ڿ� �̸�
        public float effectValue;           // ��ġ (Ȯ�� ��ȭ��, ���ʽ� �� ��)
    }
}
