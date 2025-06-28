using System;

namespace EmergencyRoulette
{
    public enum ModuleUseType
    {
        Passive,    // �� �� �ڵ� �ߵ�
        Active,     // ������ �������� �Һ��� ���
        Combo       // Ư�� �ɺ� ���� �� �ߵ�
    }

    [Serializable]
    public class ModuleDataItem
    {
        public string moduleName;             // �̸�
        public string description;            // ����

        public int purchaseCost;              // ������ ��� ���� ���
        public ModuleUseType useType;         // ��� ���

        public int energyCost;                // Active Ÿ���� ��� ������ �Һ� (Passive/Combo�� ��� ����)

        public string effectKey;              // ȿ�� �ĺ��� (���� ���� ����� Ű���� �Ǵ� enum �̸�)
    }
}
