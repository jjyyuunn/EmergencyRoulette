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

        private HashSet<int> brokenModuleIndices = new();

        // shopModules ������
        [SerializeField] private List<ModuleDataItem> debugShopModuleList = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateDebugShopList();
        }
#endif

        private void UpdateDebugShopList()
        {
            debugShopModuleList.Clear();
            foreach (var kvp in shopModules)
            {
                debugShopModuleList.Add(kvp.Value);
            }
        }
        // shopModules ������


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

            ModuleShopManager.Instance.RefreshShopUI();
            UpdateDebugShopList();
        }

        /// <summary>
        /// ������ ���� ��� �� ������ �ϳ��� �߰��մϴ�.
        /// </summary>
        public void AddRandomModuleToShop()
        {
            // 1. ��ü ��� ��� ��������
            List<ModuleDataItem> allModules = new(GameManager.Instance.ModuleDict.Values);

            // 2. ���� ������ �̹� �ִ� ��� ����
            foreach (var existing in shopModules.Values)
            {
                allModules.Remove(existing);
            }

            // 3. ���� �� ���ٸ� ����
            if (allModules.Count == 0)
            {
                Debug.LogWarning("�� �̻� �߰��� ����� �����ϴ�.");
                return;
            }

            // 4. ���� Shuffle �޼���� ����
            Shuffle(allModules);

            // 5. ù ��° ��� ���� �� �߰�
            var selectedModule = allModules[0];
            int newIndex = shopModules.Count;
            shopModules[newIndex] = selectedModule;

            ModuleShopManager.Instance.AddNewModuleToShop(newIndex, selectedModule);

            UpdateDebugShopList();
        }




        /// <summary>
        /// ����� �����ϰ� ���� ��ġ(Row/Column + index)�� ������
        /// </summary>
        public void TryPurchaseAndEquip(int moduleKey, int index)
        {
            if (!shopModules.TryGetValue(moduleKey, out var module))
                return;

            // ���� ����� �ִٸ� ���� (�����)
            equippedModules.RemoveAll(m => m.index == index);

            equippedModules.Add(new ModuleEquipSlot(module, index));
            shopModules.Remove(moduleKey);

            ModuleShopManager.Instance.ClearSelection();
            ModuleShopManager.Instance.RemoveModuleUI(moduleKey);

        }

        /// <summary>
        /// Ư�� ��ġ(Row/Col + index)�� ������ ��� ��ȯ
        /// </summary>
        public ModuleDataItem GetEquippedModule(int index)
        {
            foreach (var equip in equippedModules)
            {
                if (equip.index == index)
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

        public void SetModuleBroken(int index)
        {
            brokenModuleIndices.Add(index);
        }

        public void RepairModule(int index)
        {
            brokenModuleIndices.Remove(index);
        }

        public bool IsModuleBroken(int index)
        {
            return brokenModuleIndices.Contains(index);
        }

    }

    /// <summary>
    /// ���� ������ ��� ����: � �����, ���(Row/Col), �� ��° ���Կ� �پ��°�
    /// </summary>
    [System.Serializable]
    public class ModuleEquipSlot
    {
        public ModuleDataItem module;
        public int index;

        public ModuleEquipSlot(ModuleDataItem module, int index)
        {
            this.module = module;
            this.index = index;
        }
    }

}
