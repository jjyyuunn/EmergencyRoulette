using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        public void Setup(int key, ModuleDataItem data, System.Action<int, ModuleShopItemUI> callback)
        {
            moduleKey = key;
            currentData = data;
            onClickCallback = callback;

            titleText.text = data.moduleName;
            descriptionText.text = data.description;
            priceText.text = "무료"; // 가격 시스템 연동 가능

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
    }
}
