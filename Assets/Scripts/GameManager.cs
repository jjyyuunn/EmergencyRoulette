using UnityEngine;
using System.Collections.Generic;
using System;

namespace EmergencyRoulette
{
    [DefaultExecutionOrder(-10)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        private ExcelManager _excelManager;
        public static ExcelManager ExcelManager
        {
            get { return Instance._excelManager; }
            set { Instance._excelManager = value;  Debug.Log("excelManager Loaded");}
        }

        // example
        public Dictionary<int, TestItem> TestItemDict => ExcelManager.TestItemDict;
        public Dictionary<string, ModuleDataItem> ModuleDict => ExcelManager.ModuleDict;
        public PlayerState PlayerState = new PlayerState();

        public bool CanPlayerInteract { get; private set; }

        void Awake()
        {
            Init();
        }
        
        private static void Init()
        {
            if (Instance == null)
            {
                GameObject go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject { name = "@Managers" };
                    go.AddComponent<GameManager>();
                }
                Instance = go.GetComponent<GameManager>();
                DontDestroyOnLoad(Instance.gameObject);
            }
        }

        public void OnTurnEnd()
        {
            CanPlayerInteract = true; // 이 시점에서만 사용 허용
        }

        public void OnUseActiveModule()
        {
            CanPlayerInteract = false; // 사용 시 비활성화
        }
    }
}