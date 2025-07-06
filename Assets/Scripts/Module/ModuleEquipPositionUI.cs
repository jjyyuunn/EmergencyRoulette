using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static EmergencyRoulette.GameManager;

namespace EmergencyRoulette
{
    public class ModuleEquipPositionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject backgroundImageChildObject;
        [SerializeField] private Image backgroundImageChild;

        private int index;
        private System.Action<int> onShopClick;
        private System.Action<int> onOtherClick;

        [SerializeField] private Sprite bg_AddDataPerTurn;
        [SerializeField] private Sprite bg_AddFoodPerTurn;
        [SerializeField] private Sprite bg_AddTechPerTurn;
        [SerializeField] private Sprite bg_AddEnergyPerTurn;
        [SerializeField] private Sprite bg_IgnoreWarningWithEnergy;
        [SerializeField] private Sprite bg_GainDataIfMultipleDanger;
        [SerializeField] private Sprite bg_RepairBrokenSlot;
        [SerializeField] private Sprite bg_ConvertFoodToEnergy;
        [SerializeField] private Sprite bg_DoubleNextActiveModule;
        [SerializeField] private Sprite bg_ReduceOverloadByTech;
        [SerializeField] private Sprite bg_IncreaseDataByFood;
        [SerializeField] private Sprite bg_AddCombo_WarningDischargeOutdated;
        [SerializeField] private Sprite bg_AddCombo_EnergyDischargeDischarge;
        [SerializeField] private Sprite bg_AddCombo_TechDecayDecay;

        [SerializeField] private GameObject tooltipBox;
        [SerializeField] private TextMeshProUGUI tooltipText;

        private ModuleDataItem currentModule;

        public void Setup(int index, System.Action<int> onShopClick, System.Action<int> onOtherClick)
        {
            this.index = index;
            this.onShopClick = onShopClick;
            this.onOtherClick = onOtherClick;

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(OnClick);

        }

        private void OnClick()
        {
            if (GameManager.Instance.CurrentState != GameState.ResourceConsuming)
                return;

            if (GameManager.Instance.IsShopActive)
                onShopClick?.Invoke(index);
            else
                onOtherClick?.Invoke(index);
        }

        public void UpdateLabelWithModuleName(ModuleDataItem module)
        {
            currentModule = module;

            if (module != null)
            {
                backgroundImageChildObject.SetActive(true);

                switch (module.moduleName)
                {
                    case "������ �����":
                        backgroundImageChild.sprite = bg_AddDataPerTurn;
                        break;
                    case "�ķ� ����":
                        backgroundImageChild.sprite = bg_AddFoodPerTurn;
                        break;
                    case "������":
                        backgroundImageChild.sprite = bg_AddTechPerTurn;
                        break;
                    case "������":
                        backgroundImageChild.sprite = bg_AddEnergyPerTurn;
                        break;
                    case "��� ���ܱ�":
                        backgroundImageChild.sprite = bg_IgnoreWarningWithEnergy;
                        break;
                    case "�Ҿ��� ������":
                        backgroundImageChild.sprite = bg_GainDataIfMultipleDanger;
                        break;
                    case "���� ���� �κ�":
                        backgroundImageChild.sprite = bg_RepairBrokenSlot;
                        break;
                    case "�η� ����":
                        backgroundImageChild.sprite = bg_ConvertFoodToEnergy;
                        break;
                    case "��� ����Ŭ��":
                        backgroundImageChild.sprite = bg_DoubleNextActiveModule;
                        break;
                    case "������ ���� ���":
                        backgroundImageChild.sprite = bg_ReduceOverloadByTech;
                        break;
                    case "�극�� �����":
                        backgroundImageChild.sprite = bg_IncreaseDataByFood;
                        break;
                    case "���� ��ȯ ��������":
                        backgroundImageChild.sprite = bg_AddCombo_WarningDischargeOutdated;
                        break;
                    case "���� ���� ����":
                        backgroundImageChild.sprite = bg_AddCombo_EnergyDischargeDischarge;
                        break;
                    case "���д� ������ ��Ӵ�":
                        backgroundImageChild.sprite = bg_AddCombo_TechDecayDecay;
                        break;
                    default:
                        backgroundImageChild.sprite = null;
                        backgroundImageChildObject.SetActive(false);
                        break;
                }

                tooltipText.text = module.description;
                tooltipBox.SetActive(false);
            }
            else
            {
                backgroundImageChild.sprite = null;
                backgroundImageChildObject.SetActive(false);
                tooltipText.text = "";
                tooltipBox.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (currentModule != null)
                tooltipBox.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipBox.SetActive(false);
        }


        public int GetIndex() => index;
    }
}
