using UnityEngine;

namespace EmergencyRoulette
{
    [DefaultExecutionOrder(-10)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

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
    }
}