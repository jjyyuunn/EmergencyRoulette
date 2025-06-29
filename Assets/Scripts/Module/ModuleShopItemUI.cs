using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace EmergencyRoulette
{
    public class ModuleShopItemUI : MonoBehaviour
    {
        [Header("UI 요소")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public Image iconImage;
        public TextMeshProUGUI priceText;
        public Image background;  // 배경색 변경용 (직접 연결)

        [SerializeField] private int moduleKey;
        private ModuleDataItem currentData;
        private System.Action<int, ModuleShopItemUI> onClickCallback;

        public int GetModuleKey() => moduleKey;

        public void Setup(int key, ModuleDataItem data, System.Action<int, ModuleShopItemUI> callback)
        {
            moduleKey = key;
            currentData = data;
            onClickCallback = callback;

            titleText.text = data.moduleName;
            descriptionText.text = data.description;
            priceText.text = data.purchaseCost.ToString();

            // 타입에 따라 아이콘 색상 변경
            switch (data.useType)
            {
                case ModuleUseType.Active:
                    iconImage.color = Color.red;
                    break;
                case ModuleUseType.Passive:
                    iconImage.color = Color.blue;
                    break;
                case ModuleUseType.Combo:
                    iconImage.color = new Color(0.6f, 0f, 1f); // 보라색 (RGB 153,0,255)
                    break;
                default:
                    iconImage.color = Color.white;
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
            if (background == null) return;

            background.color = on ? Color.green : Color.white;
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
