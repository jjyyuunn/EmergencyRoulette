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

            // ��� �Ŵ������� ���� ��� ��������
            List<ModuleDataItem> shopModules = ModuleManager.Instance.shopModules;

            // Ǯ���� �ϳ��� �����ͼ� ����
            foreach (var module in shopModules)
            {
                GameObject itemGO = ModuleShopPrefabPooler.Instance.Get();
                itemGO.transform.SetParent(shopItemContainer, false);

                var itemUI = itemGO.GetComponent<ModuleShopItemUI>();
                itemUI.Setup(module, OnModuleBuyClicked);
            }
        }


        /// <summary>
        /// ��� ���� ��ư Ŭ�� �� ȣ���
        /// </summary>
        private void OnModuleBuyClicked(ModuleDataItem clickedModule)
        {
            // ����: ���� �� ������ ��ġ ���� �˾� ����
            Debug.Log($"���� �õ�: {clickedModule.moduleName}");

            // ���� ������ ���� ��ġ UI ����� ���� �̾ ó��
            // �Ǵ� ���⼭ �ٷ� �׽�Ʈ ������ ����
        }
    }
}
