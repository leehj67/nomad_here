using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGameNamespace
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        public int currentDay = 0;
        public int score = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartDay()
        {
            SceneManager.LoadScene("DayTransitionScene");
        }

        public void StartGame()
        {
            SceneManager.LoadScene("InGameScene");
        }

        public void StartResult()
        {
            SceneManager.LoadScene("ResultScene");
        }

        public void NextDay()
        {
            currentDay++;
            StartDay();
        }
    }
}
