using System.Collections.Generic;
using UnityEngine;

namespace EmergencyRoulette
{
    public class ModuleShopManager : MonoBehaviour
    {
        [Header("UI ����")]
        public Transform shopItemContainer;                  // ��� �����۵��� �� �θ� ������Ʈ (��: VerticalLayoutGroup)

        /// <summary>
        /// ���� UI �ʱ�ȭ �� ����
        /// </summary>
        public void RefreshShopUI()
        {
            // ���� UI ���� �� ��Ȱ��ȭ �� Ǯ�� ��ȯ
            List<Transform> children = new();
            foreach (Transform child in shopItemContainer)
                children.Add(child);

            foreach (var child in children)
                ModuleShopPrefabPooler.Instance.Return(child.gameObject);

            // ��� �Ŵ������� ���� ��� �������� (��ųʸ� ����)
            Dictionary<int, ModuleDataItem> shopModules = ModuleManager.Instance.shopModules;

            // Ǯ���� �ϳ��� �����ͼ� ����
            foreach (var kvp in shopModules)
            {
                int moduleKey = kvp.Key;
                ModuleDataItem module = kvp.Value;

                GameObject itemGO = ModuleShopPrefabPooler.Instance.Get();
                itemGO.transform.SetParent(shopItemContainer, false);

                // ��ġ ����
                RectTransform rt = itemGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.pivot = new Vector2(0.5f, 0.5f);

                rt.offsetMin = new Vector2(30, rt.offsetMin.y);
                rt.offsetMax = new Vector2(-30, rt.offsetMax.y);

                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 180);

                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -150 - 200 * moduleKey);

                var itemUI = itemGO.GetComponent<ModuleShopItemUI>();
                itemUI.Setup(moduleKey, module, OnModuleBuyClicked);
            }

        }

        public void OnModuleBuyClicked(int moduleKey)
        {
            var moduleDict = ModuleManager.Instance.shopModules;

            if (!moduleDict.TryGetValue(moduleKey, out var module))
                return;

            // ���⼭ ���� �õ�
            bool success = ModuleManager.Instance.TryPurchaseAndEquip(moduleKey, EquipAxis.Row, 0); // ��: Row 0�� ����

            if (success)
            {
                Debug.Log($"��� ���� �� ���� ����: {module.moduleName}");
                RefreshShopUI(); // �ٽ� UI ����
            }
            else
            {
                Debug.Log("���� ����: �̹� ������ ��ġ�ų� ����");
            }
        }

    }
}
