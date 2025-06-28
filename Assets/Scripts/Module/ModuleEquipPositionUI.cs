using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace EmergencyRoulette
{
    public class ModuleEquipPositionUI : MonoBehaviour
    {
        public TextMeshProUGUI label;

        private int index;
        private System.Action<int> onClick;

        public void Setup(int index, System.Action<int> onClick)
        {
            this.index = index;
            this.onClick = onClick;

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() => onClick?.Invoke(index));

            // 기본 라벨 텍스트 설정
            label.text = $"{index}";
        }

        public void UpdateLabelWithModuleName(ModuleDataItem module)
        {
            if (module != null)
                label.text = $"{index}\n{module.moduleName}";

            else
                label.text = $"{index}";
        }

        public int GetIndex() => index;
    }
}
