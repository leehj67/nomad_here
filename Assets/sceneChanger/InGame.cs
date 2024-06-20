using UnityEngine;
using System.Collections;

namespace MyGameNamespace
{
    public class InGame : MonoBehaviour
    {
        public float gameDuration = 5.0f; // 인게임 시간

        void Start()
        {
            StartCoroutine(GameDuration());
        }

        IEnumerator GameDuration()
        {
            yield return new WaitForSeconds(gameDuration);
            GameSceneManager.Instance.StartResult();
        }
    }
}
