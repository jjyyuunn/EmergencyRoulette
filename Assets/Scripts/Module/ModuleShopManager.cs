using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EmergencyRoulette
{
    public class ModuleShopManager : MonoBehaviour
    {
        public static ModuleShopManager Instance { get; private set; }

        [Header("UI 연결")]
        public Transform shopItemContainer;

        public int selectedModuleKey = -1;
        private ModuleShopItemUI selectedItemUI = null;

        [SerializeField] private GameObject cancelSelectedBtn;

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
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);

                rt.sizeDelta = new Vector2(382f, 154f);
                rt.anchoredPosition = new Vector2(0f, -200f - 180f * moduleKey);

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

            cancelSelectedBtn.SetActive(true);

        }

        public void ClearSelection()
        {
            if (selectedItemUI != null)
                selectedItemUI.Highlight(false);

            selectedModuleKey = -1;
            selectedItemUI = null;

            cancelSelectedBtn.SetActive(false);
        }


        public void RemoveModuleUI(int moduleKey)
        {
            foreach (Transform child in shopItemContainer)
            {
                var ui = child.GetComponent<ModuleShopItemUI>();
                if (ui != null && ui.GetModuleKey() == moduleKey)
                {
                    ui.PlayRemoveAnimation(() =>
                    {
                        // 1. UI를 풀로 반환
                        ModuleShopPrefabPooler.Instance.Return(ui.gameObject);

                        // 2. 딕셔너리 및 UI 재정렬
                        ReorderShopModulesAfterRemoval(moduleKey);
                    });
                    break;
                }
            }
        }


        private void ReorderShopModulesAfterRemoval(int removedKey)
        {
            // 1. 기존 딕셔너리를 모듈 순서대로 정렬 (key 기준)
            var sorted = ModuleManager.Instance.shopModules.OrderBy(kvp => kvp.Key).ToList();

            // 2. 새 딕셔너리 만들기
            Dictionary<int, ModuleDataItem> newDict = new();

            Sequence moveSequence = DOTween.Sequence();

            for (int newIndex = 0; newIndex < sorted.Count; newIndex++)
            {
                var module = sorted[newIndex].Value;
                int oldKey = sorted[newIndex].Key;
                newDict[newIndex] = module;

                // UI 위치도 함께 이동
                foreach (Transform child in shopItemContainer)
                {
                    var ui = child.GetComponent<ModuleShopItemUI>();
                    if (ui != null && ui.GetModuleKey() == oldKey)
                    {
                        ui.SetModuleKey(newIndex); // 내부 키 업데이트
                        RectTransform rt = ui.GetComponent<RectTransform>();

                        Tween moveTween = rt.DOAnchorPosY(-160 - 180 * newIndex, 0.3f).SetEase(Ease.OutQuad);
                        moveSequence.Join(moveTween);
                    }
                }
            }

            ModuleManager.Instance.shopModules = newDict;
            moveSequence.OnComplete(() =>
            {
                ModuleManager.Instance.AddRandomModuleToShop();
            });
        }

        public void AddNewModuleToShop(int index, ModuleDataItem newModule)
        {
            GameObject itemGO = ModuleShopPrefabPooler.Instance.Get();
            itemGO.transform.SetParent(shopItemContainer, false);

            RectTransform rt = itemGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            rt.sizeDelta = new Vector2(382f, 154f);
            rt.anchoredPosition = new Vector2(0f, -160f - 180f * index);


            var itemUI = itemGO.GetComponent<ModuleShopItemUI>();
            itemUI.Setup(index, newModule, OnModuleSelected);
        }



    }
}
