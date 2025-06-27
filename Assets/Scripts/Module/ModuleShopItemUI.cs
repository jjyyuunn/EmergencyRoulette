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
        public Image iconImage;  // 아이콘은 아직 없으면 생략 가능
        public TextMeshProUGUI priceText;

        private ModuleDataItem currentData;
        private System.Action<ModuleDataItem> onClickCallback;

        public void Setup(ModuleDataItem data, System.Action<ModuleDataItem> callback)
        {
            currentData = data;
            onClickCallback = callback;

            titleText.text = data.moduleName;
            descriptionText.text = data.description;
            priceText.text = "무료"; // 가격 시스템 있으면 연동 가능

            // 아이콘은 나중에 연동
            // iconImage.sprite = ...;

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickCallback?.Invoke(currentData);
            });
        }
    }
}
