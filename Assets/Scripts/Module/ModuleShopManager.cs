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

            // 모듈 매니저에서 상점 모듈 가져오기
            List<ModuleDataItem> shopModules = ModuleManager.Instance.shopModules;

            // 풀에서 하나씩 가져와서 세팅
            foreach (var module in shopModules)
            {
                GameObject itemGO = ModuleShopPrefabPooler.Instance.Get();
                itemGO.transform.SetParent(shopItemContainer, false);

                var itemUI = itemGO.GetComponent<ModuleShopItemUI>();
                itemUI.Setup(module, OnModuleBuyClicked);
            }
        }


        /// <summary>
        /// 모듈 구매 버튼 클릭 시 호출됨
        /// </summary>
        private void OnModuleBuyClicked(ModuleDataItem clickedModule)
        {
            // 예시: 구매 후 장착할 위치 선택 팝업 띄우기
            Debug.Log($"구매 시도: {clickedModule.moduleName}");

            // 실제 장착은 장착 위치 UI 만들고 나서 이어서 처리
            // 또는 여기서 바로 테스트 장착도 가능
        }
    }
}
