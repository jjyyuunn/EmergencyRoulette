using System.Collections.Generic;
using UnityEngine;

namespace EmergencyRoulette
{
    public class ModuleManager : MonoBehaviour
    {
        public static ModuleManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        // �̹� ������ ������ ����
        public Dictionary<int, ModuleDataItem> shopModules = new();

        // ���Ըӽſ� ������ ����
        public List<ModuleEquipSlot> equippedModules = new();

        [SerializeField] public ModuleShopManager shopManager;



        /// <summary>
        /// ������ ������ ��� N���� �������� ����
        /// </summary>
        public void SetupShop(int count)
        {
            shopModules.Clear();

            // ��ųʸ� ������ ����Ʈ�� ��ȯ (��� ��ü ���)
            List<ModuleDataItem> shuffled = new(GameManager.Instance.ModuleDict.Values);
            Shuffle(shuffled);

            // N���� ������ ����
            for (int i = 0; i < Mathf.Min(count, shuffled.Count); i++)
            {
                shopModules.Add(i, shuffled[i]);
            }

            shopManager.RefreshShopUI();
        }



        /// <summary>
        /// ����� �����ϰ� ���� ��ġ(Row/Column + index)�� ������
        /// </summary>
        public bool TryPurchaseAndEquip(int moduleKey, EquipAxis axis, int index)
        {
            // ������ �ش� Ű�� ������ ����
            if (!shopModules.TryGetValue(moduleKey, out var module))
                return false;

            // �ߺ� ���� ����
            foreach (var equip in equippedModules)
            {
                if (equip.axis == axis && equip.index == index)
                    return false;
            }

            equippedModules.Add(new ModuleEquipSlot(module, axis, index));
            shopModules.Remove(moduleKey);

            return true;
        }


        /// <summary>
        /// Ư�� ��ġ�� ������ ��� ����
        /// </summary>
        public void UnequipModule(EquipAxis axis, int index)
        {
            equippedModules.RemoveAll(m => m.axis == axis && m.index == index);
        }

        /// <summary>
        /// Ư�� ��ġ(Row/Col + index)�� ������ ��� ��ȯ
        /// </summary>
        public ModuleDataItem GetEquippedModule(EquipAxis axis, int index)
        {
            foreach (var equip in equippedModules)
            {
                if (equip.axis == axis && equip.index == index)
                    return equip.module;
            }
            return null;
        }

        /// <summary>
        /// ����Ʈ ������ ����
        /// </summary>
        private void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randIndex = Random.Range(i, list.Count);
                (list[i], list[randIndex]) = (list[randIndex], list[i]);
            }
        }
    }

    /// <summary>
    /// ���� ������ ��� ����: � �����, ���(Row/Col), �� ��° ���Կ� �پ��°�
    /// </summary>
    [System.Serializable]
    public class ModuleEquipSlot
    {
        public ModuleDataItem module;
        public EquipAxis axis;
        public int index;

        public ModuleEquipSlot(ModuleDataItem module, EquipAxis axis, int index)
        {
            this.module = module;
            this.axis = axis;
            this.index = index;
        }
    }

    /// <summary>
    /// ���� ��ġ ��: Row �Ǵ� Column
    /// </summary>
    public enum EquipAxis
    {
        Row,
        Column
    }
}
