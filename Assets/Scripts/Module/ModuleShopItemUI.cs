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
        public Image background;  // ���� ����� (���� ����)

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
            priceText.text = "����"; // ���� �ý��� ���� ����

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
