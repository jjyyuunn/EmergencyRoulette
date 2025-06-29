using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace EmergencyRoulette
{
    public class SlotBackGroundUIController : MonoBehaviour
    {
        [SerializeField] private List<Image> rowIndicators; // 0~4���� Image ������Ʈ

        private void Start()
        {
            // �ڵ� �Ҵ�
            if (rowIndicators == null || rowIndicators.Count == 0)
            {
                rowIndicators = GetComponentsInChildren<RectTransform>(true)
                    .Where(rt => rt != this.transform) // �ڱ� �ڽ� ����
                    .Select(rt => rt.GetComponent<Image>())
                    .Where(img => img != null)
                    .OrderByDescending(img => img.rectTransform.anchoredPosition.y)
                    .Take(5)
                    .ToList();

                Debug.Log($"[SlotBackGroundUIController] rowIndicators auto-assigned: {rowIndicators.Count}");
            }
        }

        public void UpdateRowUI()
        {
            var slotBoard = GameManager.Instance.PlayerState.SlotBoard;
            if (slotBoard == null || rowIndicators == null || rowIndicators.Count < 5) return;

            for (int y = 0; y < slotBoard.RowCount; y++)
            {
                bool isUnlocked = slotBoard.Rows[y];
                var img = rowIndicators[y];

                if (img != null)
                {
                    img.color = isUnlocked ? Color.white : new Color32(0xFF, 0x5A, 0x5A, 0xFF);
                }
            }
        }
    }
}
