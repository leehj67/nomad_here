using UnityEngine;
using TMPro;
using System.Collections;

namespace MyGameNamespace
{
    public class DayTransition : MonoBehaviour
    {
        public TMP_Text dayText;
        public float displayDuration = 2.0f; // 텍스트 표시 시간

        void Start()
        {
            if (dayText == null)
            {
                dayText = GameObject.Find("DayText").GetComponent<TMP_Text>();
            }

            if (dayText != null)
            {
                dayText.text = GameSceneManager.Instance.currentDay + " 일차...";
                StartCoroutine(DisplayTextAndChangeScene());
            }
            else
            {
                Debug.LogError("DayTransition: TMP Text object is not assigned and could not be found.");
            }
        }

        IEnumerator DisplayTextAndChangeScene()
        {
            yield return new WaitForSeconds(displayDuration);
            GameSceneManager.Instance.StartGame();
        }
    }
}
