using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace EmergencyRoulette
{
    public class ModuleShopItemUI : MonoBehaviour
    {
        public Image background;  // 배경색 변경용 (직접 연결)

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
                case "데이터 생산기":
                    background.sprite = bg_AddDataPerTurn;
                    break;
                case "식량 공장":
                    background.sprite = bg_AddFoodPerTurn;
                    break;
                case "연구소":
                    background.sprite = bg_AddTechPerTurn;
                    break;
                case "발전소":
                    background.sprite = bg_AddEnergyPerTurn;
                    break;
                case "경고 차단기":
                    background.sprite = bg_IgnoreWarningWithEnergy;
                    break;
                case "불안정 수집기":
                    background.sprite = bg_GainDataIfMultipleDanger;
                    break;
                case "피해 복구 로봇":
                    background.sprite = bg_RepairBrokenSlot;
                    break;
                case "인력 발전":
                    background.sprite = bg_ConvertFoodToEnergy;
                    break;
                case "모듈 오버클럭":
                    background.sprite = bg_DoubleNextActiveModule;
                    break;
                case "과부하 통제 기술":
                    background.sprite = bg_ReduceOverloadByTech;
                    break;
                case "브레인 스토밍":
                    background.sprite = bg_IncreaseDataByFood;
                    break;
                case "위기 전환 프로토콜":
                    background.sprite = bg_AddCombo_WarningDischargeOutdated;
                    break;
                case "응급 충전 유닛":
                    background.sprite = bg_AddCombo_EnergyDischargeDischarge;
                    break;
                case "실패는 성공의 어머니":
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
