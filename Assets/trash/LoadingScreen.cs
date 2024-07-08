using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider progressBar;
    public Text progressText;

    void Start()
    {
        // 로딩 화면을 비활성화합니다.
        loadingScreen.SetActive(false);
    }

    public void LoadScene(int sceneIndex)
    {
        // 로딩 화면을 활성화하고 씬 로드를 시작합니다.
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        // 로딩 화면을 활성화합니다.
        loadingScreen.SetActive(true);

        // 씬을 비동기적으로 로드합니다.
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        // 씬이 로드되는 동안 진행 상황을 업데이트합니다.
        while (!operation.isDone)
        {
            // 로딩 진행도를 0에서 1 사이의 값으로 계산합니다.
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // 진행도를 프로그레스 바와 텍스트에 반영합니다.
            progressBar.value = progress;
            progressText.text = (progress * 100f).ToString("F2") + "%";

            // 다음 프레임을 대기합니다.
            yield return null;
        }

        // 씬이 로드되면 로딩 화면을 비활성화합니다.
        loadingScreen.SetActive(false);
    }
}
