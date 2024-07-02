using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class HostRoomManager : MonoBehaviourPunCallbacks
{
    public Button startGameButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
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
