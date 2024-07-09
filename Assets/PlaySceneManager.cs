using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviourPunCallbacks
{
    public float playDuration = 60f;
    private float timer;

    private void Start()
    {
        timer = playDuration;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ReturnToGameScene();
        }
    }

    private void ReturnToGameScene()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.AdvanceDay();
        }
        else
        {
            Debug.LogError("PlaySceneManager: GameStateManager instance not found");
        }

        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PlaySceneManager: Successfully joined the room");
    }
}
