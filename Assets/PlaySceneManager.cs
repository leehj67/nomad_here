using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviourPunCallbacks
{
    public float playDuration = 60f;
    private float timer;
    private bool isReturningToGameScene = false;

    private void Start()
    {
        timer = playDuration;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && !isReturningToGameScene)
        {
            isReturningToGameScene = true;
            ReturnToGameScene();
        }
    }

   private void ReturnToGameScene()
{
    Debug.Log("Returning to GameScene...");
    SceneManager.LoadScene("GameScene");
    SceneManager.sceneLoaded += OnGameSceneLoaded;
}

private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
{
    if (scene.name == "GameScene")
    {
        Debug.Log("GameScene loaded, advancing day...");
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.AdvanceDay();
        }
        else
        {
            Debug.LogError("GameStateManager instance is not found!");
        }

        SceneManager.sceneLoaded -= OnGameSceneLoaded;
    }
}

}
