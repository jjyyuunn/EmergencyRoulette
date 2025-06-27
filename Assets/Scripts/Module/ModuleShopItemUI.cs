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

        [SerializeField] private int moduleKey;
        private ModuleDataItem currentData;
        private System.Action<int> onClickCallback;

        public void Setup(int key, ModuleDataItem data, System.Action<int> callback)
        {
            moduleKey = key;
            currentData = data;
            onClickCallback = callback;

            titleText.text = data.moduleName;
            descriptionText.text = data.description;
            priceText.text = "무료"; // 가격 시스템 연동 가능

            // 아이콘은 나중에
            // iconImage.sprite = ...

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickCallback?.Invoke(moduleKey);
            });
        }
    }
}
