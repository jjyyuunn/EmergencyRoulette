using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static EmergencyRoulette.GameManager;

namespace EmergencyRoulette
{
    public class ModuleEquipPositionUI : MonoBehaviour
    {
        public TextMeshProUGUI label;

        private int index;
        private System.Action<int> onShopClick;
        private System.Action<int> onOtherClick;

        public void Setup(int index, System.Action<int> onShopClick, System.Action<int> onOtherClick)
        {
            this.index = index;
            this.onShopClick = onShopClick;
            this.onOtherClick = onOtherClick;

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(OnClick);

            // 기본 라벨 텍스트 설정
            //label.text = $"{index}";
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
            if (module != null)
                label.text = $"{module.moduleName}";
            else
                label.text = $"{index}";
        }

        public int GetIndex() => index;
    }
}
