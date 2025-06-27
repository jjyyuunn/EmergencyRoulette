using System;
using UnityEngine;

namespace EmergencyRoulette.Test
{
    public class ExcelLoadTest : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var item in GameManager.Instance.TestItemDict)
                {
                    Debug.Log($"id:{item.Key} name:{item.Value.name} ");
                }
            }
        }
    }
}