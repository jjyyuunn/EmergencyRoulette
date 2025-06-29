using UnityEngine;
using UnityEngine.SceneManagement;

namespace EmergencyRoulette
{
    public class LoadSceneManager : MonoBehaviour
    {
        public void LoadGame()
        {
            SceneManager.LoadScene("02_MainGame");
        }

        public void LoadLobby()
        {
            SceneManager.LoadScene("01_Lobby");
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}