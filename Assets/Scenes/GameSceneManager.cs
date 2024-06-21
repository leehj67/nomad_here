using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;  // TextMeshPro 네임스페이스 추가
using System.Collections;

namespace MyGameNamespace
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        public int currentDay = 0;
        public int score = 0;
        private TextMeshProUGUI dayText; // Text 대신 TextMeshProUGUI 사용

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
            SceneManager.LoadScene("DayTransitionScene", LoadSceneMode.Single);
            SceneManager.sceneLoaded += OnDayTransitionSceneLoaded;
        }

        private void OnDayTransitionSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "DayTransitionScene")
            {
                GameObject canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    Transform imageTransform = canvas.transform.Find("Image");
                    if (imageTransform != null)
                    {
                        dayText = imageTransform.Find("DayText")?.GetComponent<TextMeshProUGUI>();
                        if (dayText != null)
                        {
                            StartCoroutine(UpdateDayTextAndProceed());
                        }
                        else
                        {
                            Debug.LogError("DayText 오브젝트에 TextMeshProUGUI 컴포넌트가 없습니다.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Image 오브젝트를 찾을 수 없습니다.");
                    }
                }
                else
                {
                    Debug.LogError("Canvas 오브젝트를 찾을 수 없습니다.");
                }
                SceneManager.sceneLoaded -= OnDayTransitionSceneLoaded;
            }
        }

        IEnumerator UpdateDayTextAndProceed()
        {
            dayText.text = "Day: " + currentDay.ToString();
            yield return new WaitForSeconds(2); // 2초 후에 다음 씬 로드
            StartGame(); // 다음 게임 씬 시작
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
