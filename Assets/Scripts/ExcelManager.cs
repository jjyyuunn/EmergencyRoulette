using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace EmergencyRoulette
{
    [DefaultExecutionOrder(-1)]
    public class ExcelManager : MonoBehaviour
    {
        public static ExcelManager Instance;
        public static Dictionary<int, TestItem> TestItemDict;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                GameManager.ExcelManager = this;
                TestItemDict = new();
                LoadDatabase();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // 여기에 로드할 거 추가
        private void LoadDatabase()
        {
            ExcelTest db = Resources.Load<ExcelTest>("ScriptableObjects/ExcelTest");
            if (db == null)
            {
                Debug.LogError("ExcelTest를 찾을 수 없습니다!");
                return;
            }
            TestItemDict = db.Sheet1.ToDictionary(item => item.id);
            Debug.Log("아이템 딕셔너리 초기화 완료");
        }

        // test 예시
        public static TestItem GetItemById(int id)
        {
            if (TestItemDict != null && TestItemDict.TryGetValue(id, out var item))
            {
                return item;
            }
            Debug.LogWarning($"ID {id}에 해당하는 아이템이 없습니다.");
            return null;
        }
    }
}