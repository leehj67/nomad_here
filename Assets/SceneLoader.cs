using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject gameStateManagerPrefab;
    public GameObject eventManagerPrefab; // EventManager 프리팹을 추가

    private void Start()
    {
        if (GameStateManager.Instance == null)
        {
            GameObject manager = Instantiate(gameStateManagerPrefab);
            DontDestroyOnLoad(manager);
        }

        if (EventManager.Instance == null) // EventManager가 없을 경우 인스턴스 생성
        {
            GameObject manager = Instantiate(eventManagerPrefab);
            DontDestroyOnLoad(manager);
        }
    }

    public void LoadBScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
