using System.Collections.Generic;
using UnityEngine;

namespace EmergencyRoulette
{
    public class ModuleShopManager : MonoBehaviour
    {
        public static ModuleShopManager Instance { get; private set; }

        [Header("UI 연결")]
        public Transform shopItemContainer;

        public int selectedModuleKey = -1;
        private ModuleShopItemUI selectedItemUI = null;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void RefreshShopUI()
        {
            selectedModuleKey = -1;
            selectedItemUI = null;

            // 기존 UI 제거 → 비활성화 후 풀에 반환
            List<Transform> children = new();
            foreach (Transform child in shopItemContainer)
                children.Add(child);

            foreach (var child in children)
                ModuleShopPrefabPooler.Instance.Return(child.gameObject);

            Dictionary<int, ModuleDataItem> shopModules = ModuleManager.Instance.shopModules;

            foreach (var kvp in shopModules)
            {
                int moduleKey = kvp.Key;
                ModuleDataItem module = kvp.Value;

                GameObject itemGO = ModuleShopPrefabPooler.Instance.Get();
                itemGO.transform.SetParent(shopItemContainer, false);

                RectTransform rt = itemGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.offsetMin = new Vector2(30, rt.offsetMin.y);
                rt.offsetMax = new Vector2(-30, rt.offsetMax.y);
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 180);
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -150 - 200 * moduleKey);

                var itemUI = itemGO.GetComponent<ModuleShopItemUI>();
                itemUI.Setup(moduleKey, module, OnModuleSelected);
            }
        }

        public void OnModuleSelected(int moduleKey, ModuleShopItemUI ui)
        {
            // 이전 선택 해제
            if (selectedItemUI != null)
                selectedItemUI.Highlight(false);

            // 새로운 선택
            selectedModuleKey = moduleKey;
            selectedItemUI = ui;
            selectedItemUI.Highlight(true);

            // 장착 위치 UI 활성화
            //ModuleEquipManager.Instance.SelectModule(moduleKey);

            Debug.Log($"모듈 선택됨: {moduleKey}");
        }

        public void ClearSelection()
        {
            if (selectedItemUI != null)
                selectedItemUI.Highlight(false);

            selectedModuleKey = -1;
            selectedItemUI = null;
        }

        public int GetSelectedModuleKey() => selectedModuleKey;
    }
}
