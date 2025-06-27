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
            priceText.text = "����"; // ���� �ý��� ���� ����

            // �������� ���߿�
            // iconImage.sprite = ...

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickCallback?.Invoke(moduleKey);
            });
        }
    }
}
