using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace EmergencyRoulette
{
    public class ModuleShopItemUI : MonoBehaviour
    {
        public Image background;  // ���� ����� (���� ����)

        [SerializeField] private int moduleKey;
        [SerializeField] private GameObject module_highlight;
        private ModuleDataItem currentData;
        private System.Action<int, ModuleShopItemUI> onClickCallback;

        [Header("BG Sprites")]
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
        [SerializeField] private Sprite defaultBg;



        public int GetModuleKey() => moduleKey;

        public void Setup(int key, ModuleDataItem data, System.Action<int, ModuleShopItemUI> callback)
        {
            moduleKey = key;
            currentData = data;
            onClickCallback = callback;

            switch (data.moduleName)
            {
                case "������ �����":
                    background.sprite = bg_AddDataPerTurn;
                    break;
                case "�ķ� ����":
                    background.sprite = bg_AddFoodPerTurn;
                    break;
                case "������":
                    background.sprite = bg_AddTechPerTurn;
                    break;
                case "������":
                    background.sprite = bg_AddEnergyPerTurn;
                    break;
                case "��� ���ܱ�":
                    background.sprite = bg_IgnoreWarningWithEnergy;
                    break;
                case "�Ҿ��� ������":
                    background.sprite = bg_GainDataIfMultipleDanger;
                    break;
                case "���� ���� �κ�":
                    background.sprite = bg_RepairBrokenSlot;
                    break;
                case "�η� ����":
                    background.sprite = bg_ConvertFoodToEnergy;
                    break;
                case "��� ����Ŭ��":
                    background.sprite = bg_DoubleNextActiveModule;
                    break;
                case "������ ���� ���":
                    background.sprite = bg_ReduceOverloadByTech;
                    break;
                case "�극�� �����":
                    background.sprite = bg_IncreaseDataByFood;
                    break;
                case "���� ��ȯ ��������":
                    background.sprite = bg_AddCombo_WarningDischargeOutdated;
                    break;
                case "���� ���� ����":
                    background.sprite = bg_AddCombo_EnergyDischargeDischarge;
                    break;
                case "���д� ������ ��Ӵ�":
                    background.sprite = bg_AddCombo_TechDecayDecay;
                    break;
                default:
                    background.sprite = defaultBg;
                    break;
            }



            Highlight(false);

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickCallback?.Invoke(moduleKey, this);
            });
        }

        public void Highlight(bool on)
        {
            if (module_highlight != null)
                module_highlight.SetActive(on);
        }


        public void PlayRemoveAnimation(System.Action onComplete)
        {
            CanvasGroup cg = GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = gameObject.AddComponent<CanvasGroup>();
            }

            Sequence seq = DOTween.Sequence();
            seq.Join(cg.DOFade(0, 0.4f))
               .Join(transform.DOLocalMoveX(transform.localPosition.x - 50f, 0.4f))
               .OnComplete(() =>
               {
                   cg.alpha = 1f;
                   onComplete?.Invoke();
               });
        }

        public void SetModuleKey(int newKey)
        {
            this.moduleKey = newKey;
        }


    }
}
