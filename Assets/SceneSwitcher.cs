using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    // 대기할 시간을 설정합니다. (예: 5초)
    public float waitTime = 5f;

    void Start()
    {
        // 장면 전환을 위한 코루틴을 시작합니다.
        StartCoroutine(SwitchSceneAfterTime());
    }

    IEnumerator SwitchSceneAfterTime()
    {
        // 지정된 시간만큼 대기합니다.
        yield return new WaitForSeconds(waitTime);

        // 날짜 업데이트 로직 추가
        UpdateDate();

        // "GameScene"이라는 이름의 장면으로 전환합니다.
        SceneManager.LoadScene("GameScene");
    }

    void UpdateDate()
    {
        // 현재 날짜를 증가시키고 저장
        int currentDay = PlayerPrefs.GetInt("Day", 0); // 초기값을 0으로 설정
        currentDay++;
        PlayerPrefs.SetInt("Day", currentDay);
        PlayerPrefs.Save();
    }
}
