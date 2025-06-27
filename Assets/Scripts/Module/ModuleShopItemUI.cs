using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmergencyRoulette
{
    public class ModuleShopItemUI : MonoBehaviour
    {
        [Header("UI ���")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public Image iconImage;  // �������� ���� ������ ���� ����
        public TextMeshProUGUI priceText;

        private ModuleDataItem currentData;
        private System.Action<ModuleDataItem> onClickCallback;

        public void Setup(ModuleDataItem data, System.Action<ModuleDataItem> callback)
        {
            currentData = data;
            onClickCallback = callback;

            titleText.text = data.moduleName;
            descriptionText.text = data.description;
            priceText.text = "����"; // ���� �ý��� ������ ���� ����

            // �������� ���߿� ����
            // iconImage.sprite = ...;

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickCallback?.Invoke(currentData);
            });
        }
    }
}
