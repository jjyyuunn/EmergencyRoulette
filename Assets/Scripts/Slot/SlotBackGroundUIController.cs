using System.Collections.Generic;
using UnityEngine;

namespace EmergencyRoulette
{
    public class SlotBackGroundUIController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> rowIndicators; // 0~4번 row에 해당하는 UI 오브젝트들
        private SlotBoard slotBoard;

        public void Init(SlotBoard board)
        {
            slotBoard = board;
            UpdateRowUI();
        }

        public void UpdateRowUI()
        {
            if (slotBoard == null) return;

            for (int y = 0; y < slotBoard.RowCount; y++)
            {
                bool isActive = slotBoard.Rows[y];

                // 예: 잠긴 row는 비활성화된 이미지 표시
                rowIndicators[y].SetActive(isActive);
            }
        }
    }
}
