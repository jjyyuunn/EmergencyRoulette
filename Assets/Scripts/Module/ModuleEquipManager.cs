using UnityEngine;
using UnityEngine.UI;

namespace EmergencyRoulette
{
    public class ModuleEquipManager : MonoBehaviour
    {
        public static ModuleEquipManager Instance { get; private set; }

        public int row = 3;

        [Header("UI ������ & �θ�")]
        public GameObject equipButtonPrefab;
        public Transform equipButtonContainer;

        public Button cancelModuleEquipBtn;

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

        private void GenerateEquipButtons()
        {
            for (int i = 0; i < row; i++)
            {
                GameObject btn = Instantiate(equipButtonPrefab, equipButtonContainer);
                var ui = btn.GetComponent<ModuleEquipPositionUI>();

                ui.Setup(i, OnEquipButtonClicked); // axis ���ŵ� ����
                ui.label.text = $"Slot {i}";

                RectTransform rt = btn.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, -(i - (row - 1) / 2f) * 200f); // �߾� ����
            }
        }

        private void OnEquipButtonClicked(int index)
        {
            if (ModuleShopManager.Instance.selectedModuleKey == -1)
                return;

            ModuleManager.Instance.TryPurchaseAndEquip(ModuleShopManager.Instance.selectedModuleKey, index);
            ModuleShopManager.Instance.selectedModuleKey = -1;

            // UI ������Ʈ
            foreach (Transform child in equipButtonContainer)
            {
                var ui = child.GetComponent<ModuleEquipPositionUI>();
                if (ui.GetIndex() == index)
                {
                    var equipped = ModuleManager.Instance.GetEquippedModule(index);
                    ui.UpdateLabelWithModuleName(equipped);
                    break;
                }
            }
        }
    }
}
