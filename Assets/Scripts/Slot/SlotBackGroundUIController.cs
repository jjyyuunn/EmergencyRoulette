using System.Collections.Generic;
using UnityEngine;

namespace EmergencyRoulette
{
    public class SlotBackGroundUIController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> rowIndicators; // 0~4�� row�� �ش��ϴ� UI ������Ʈ��
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

                // ��: ��� row�� ��Ȱ��ȭ�� �̹��� ǥ��
                rowIndicators[y].SetActive(isActive);
            }
        }
    }
}
