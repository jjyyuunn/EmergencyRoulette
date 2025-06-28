using System;
using UnityEngine;

namespace EmergencyRoulette.Test
{
    public class ExcelLoadTest : MonoBehaviour
    {
        public SlotBoard SlotBoard;
        public SymbolPicker SymbolPicker;
        public SymbolLibrary SymbolLibrary;

        private void Start()
        {
            SymbolLibrary = new SymbolLibrary();
            SymbolPicker = new SymbolPicker(SymbolLibrary);
            SlotBoard = new SlotBoard(SymbolPicker);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var item in GameManager.Instance.TestItemDict)
                {
                    Debug.Log($"id:{item.Key} name:{item.Value.name} ");
                }
            }

            // if (Input.GetKeyDown(KeyCode.DownArrow))
            // {
            //     foreach (var pair in SlotBoard.Grid)
            //     {
            //         Debug.Log($"Grid:{pair.Key}, Symbol:{pair.Value}");
            //     }
            // }
        }
    }
}