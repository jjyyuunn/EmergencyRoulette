using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace EmergencyRoulette
{
    public class ModuleEquipPositionUI : MonoBehaviour
    {
        public TextMeshProUGUI label;

        private EquipAxis axis;
        private int index;
        private System.Action<EquipAxis, int> onClick;

        public void Setup(EquipAxis axis, int index, System.Action<EquipAxis, int> onClick)
        {
            this.axis = axis;
            this.index = index;
            this.onClick = onClick;

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() => onClick?.Invoke(axis, index));

            // 기본 라벨 텍스트 설정
            label.text = $"{axis} {index}";
        }

        public void UpdateLabelWithModuleName(ModuleDataItem module)
        {
            if (module != null)
                label.text = $"{axis} {index}\n{module.moduleName}";

            else
                label.text = $"{axis} {index}";
        }

        public EquipAxis GetAxis() => axis;
        public int GetIndex() => index;
    }
}
