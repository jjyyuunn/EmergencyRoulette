using UnityEngine;
using UnityEngine.UI;

namespace EmergencyRoulette
{
    public class ModuleEquipManager : MonoBehaviour
    {
        public static ModuleEquipManager Instance { get; private set; }

        [Header("행/열 수")]
        public int row = 3;
        public int column = 3;

        [Header("UI 프리팹 & 부모")]
        public GameObject equipButtonPrefab;
        public Transform rowButtonContainer;
        public Transform columnButtonContainer;

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
                GameObject btn = Instantiate(equipButtonPrefab, rowButtonContainer);
                var ui = btn.GetComponent<ModuleEquipPositionUI>();
                ui.Setup(EquipAxis.Row, i, OnEquipButtonClicked);
                ui.label.text = $"Row {i}";

                RectTransform rt = btn.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2((i - 1) * 200, 0);
            }

            for (int i = 0; i < column; i++)
            {
                GameObject btn = Instantiate(equipButtonPrefab, columnButtonContainer);
                var ui = btn.GetComponent<ModuleEquipPositionUI>();
                ui.Setup(EquipAxis.Column, i, OnEquipButtonClicked);
                ui.label.text = $"Column {i}";

                RectTransform rt = btn.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, (i - 1) * 200);
            }
        }

        private void OnEquipButtonClicked(EquipAxis axis, int index)
        {
            if (ModuleShopManager.Instance.selectedModuleKey == -1)
                return;

            ModuleManager.Instance.TryPurchaseAndEquip(ModuleShopManager.Instance.selectedModuleKey, axis, index);

            ModuleShopManager.Instance.selectedModuleKey = -1;

            foreach (Transform child in (axis == EquipAxis.Row ? rowButtonContainer : columnButtonContainer))
            {
                var ui = child.GetComponent<ModuleEquipPositionUI>();
                if (ui.GetAxis() == axis && ui.GetIndex() == index)
                {
                    var equipped = ModuleManager.Instance.GetEquippedModule(axis, index);
                    ui.UpdateLabelWithModuleName(equipped);
                    break;
                }
            }

        }
    }
}
