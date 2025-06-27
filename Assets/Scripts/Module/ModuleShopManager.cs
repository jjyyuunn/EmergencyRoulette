using System.Collections.Generic;
using UnityEngine;

namespace EmergencyRoulette
{
    public class ModuleShopManager : MonoBehaviour
    {
        [Header("UI 연결")]
        public Transform shopItemContainer;                  // 모듈 아이템들이 들어갈 부모 오브젝트 (예: VerticalLayoutGroup)

        /// <summary>
        /// 상점 UI 초기화 및 갱신
        /// </summary>
        public void RefreshShopUI()
        {
            // 기존 UI 제거 → 비활성화 후 풀에 반환
            List<Transform> children = new();
            foreach (Transform child in shopItemContainer)
                children.Add(child);

            foreach (var child in children)
                ModuleShopPrefabPooler.Instance.Return(child.gameObject);

            // 모듈 매니저에서 상점 모듈 가져오기 (딕셔너리 기준)
            Dictionary<int, ModuleDataItem> shopModules = ModuleManager.Instance.shopModules;

            // 풀에서 하나씩 가져와서 세팅
            foreach (var kvp in shopModules)
            {
                int moduleKey = kvp.Key;
                ModuleDataItem module = kvp.Value;

                GameObject itemGO = ModuleShopPrefabPooler.Instance.Get();
                itemGO.transform.SetParent(shopItemContainer, false);

                // 위치 조정
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

            // 여기서 장착 시도
            bool success = ModuleManager.Instance.TryPurchaseAndEquip(moduleKey, EquipAxis.Row, 0); // 예: Row 0에 장착

            if (success)
            {
                Debug.Log($"모듈 구매 및 장착 성공: {module.moduleName}");
                RefreshShopUI(); // 다시 UI 갱신
            }
            else
            {
                Debug.Log("구매 실패: 이미 장착된 위치거나 오류");
            }
        }

    }
}
