using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class HostRoomManager : MonoBehaviourPunCallbacks
{
    public Button startGameButton;
    public GameObject gameStateManagerPrefab;
    public GameObject eventManagerPrefab;

    private void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // PhotonNetwork를 통해 프리팹 인스턴스화
            if (GameStateManager.Instance == null)
            {
                PhotonNetwork.Instantiate(gameStateManagerPrefab.name, Vector3.zero, Quaternion.identity);
            }
            if (EventManager.Instance == null)
            {
                PhotonNetwork.Instantiate(eventManagerPrefab.name, Vector3.zero, Quaternion.identity);
            }

            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }

    public override void OnLeftRoom()
    {
        startGameButton.gameObject.SetActive(false);
    }
}
