using UnityEngine;
using TMPro;
using System.Collections;

namespace MyGameNamespace
{
    public class Result : MonoBehaviour
    {
        public TMP_Text resultText;
        public float displayDuration = 3.0f; // 정산 화면 표시 시간

        void Start()
        {
            resultText.text = "점수: " + GameSceneManager.Instance.score;
            StartCoroutine(DisplayResultAndNextDay());
        }

        IEnumerator DisplayResultAndNextDay()
        {
            yield return new WaitForSeconds(displayDuration);
            GameSceneManager.Instance.NextDay();
        }
    }
}
