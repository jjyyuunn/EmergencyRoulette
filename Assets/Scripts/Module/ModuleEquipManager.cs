using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EmergencyRoulette
{
    public class ModuleEquipManager : MonoBehaviour
    {
        public static ModuleEquipManager Instance { get; private set; }

        public int row = 5;

        [Header("UI 프리팹 & 부모")]
        public GameObject equipButtonPrefab;
        public GameObject equipBrokenImagePrefab;
        public Transform equipButtonContainer;

        public Button cancelModuleEquipBtn;

        private List<GameObject> brokenImages = new();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            GenerateEquipButtons();

            cancelModuleEquipBtn.onClick.AddListener(() =>
            {
                if (ModuleShopManager.Instance.selectedModuleKey != -1)
                {
                    ModuleShopManager.Instance.ClearSelection();
                }
            });
        }

        public List<GameObject> GetBrokenImages()
        {
            return brokenImages;
        }


        private void GenerateEquipButtons()
        {
            brokenImages.Clear();

            for (int i = 0; i < row; i++)
            {
                GameObject btn = Instantiate(equipButtonPrefab, equipButtonContainer);
                GameObject img = Instantiate(equipBrokenImagePrefab, equipButtonContainer);
                var ui = btn.GetComponent<ModuleEquipPositionUI>();

                ui.Setup(
                    i,
                    onShopClick: OnEquipButtonClicked,
                    onOtherClick: OnEquipSlotClickedOutsideShop
                );

                //ui.label.text = $"Slot {i}";

                RectTransform rt = btn.GetComponent<RectTransform>();
                RectTransform rt2 = img.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(79f, 222f - i * 143f);
                rt2.anchoredPosition = new Vector2(79f, 222f - i * 143f);

                img.name = "Img_" + i.ToString();
                brokenImages.Add(img);

            }

            ModuleManager.Instance.RefreshBrokenImages();
        }

        private void OnEquipButtonClicked(int index)
        {
            if (ModuleShopManager.Instance.selectedModuleKey == -1)
                return;

            ModuleManager.Instance.TryPurchaseAndEquip(ModuleShopManager.Instance.selectedModuleKey, index);
            ModuleShopManager.Instance.selectedModuleKey = -1;

            foreach (Transform child in equipButtonContainer)
            {
                if (!child.TryGetComponent(out ModuleEquipPositionUI ui))
                    continue;

                if (ui.GetIndex() == index)
                {
                    var equipped = ModuleManager.Instance.GetEquippedModule(index);
                    ui.UpdateLabelWithModuleName(equipped);
                    break;
                }
            }

        }

        private void OnEquipSlotClickedOutsideShop(int index)
        {
            // 1. 해당 index에 장착된 모듈 찾기
            var slot = ModuleManager.Instance.equippedModules.Find(m => m.index == index);
            if (slot == null || slot.module == null)
                return;

            // 2. 모듈이 Active 타입이 아니면 리턴
            if (slot.module.useType != ModuleUseType.Active)
                return;

            // 3. 여기서 Active 모듈의 효과 발동 처리
            ModuleEffectExecutor.ApplyActiveModuleAt(index);
        }

    }
}
