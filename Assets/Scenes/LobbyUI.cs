using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyUI : MonoBehaviour
{
    public TMP_InputField roomNameInput;
    public Button createRoomButton;
    public Button joinRoomButton;
    public Button startGameButton;

    void Start()
    {
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
        startGameButton.onClick.AddListener(StartGame);
    }

    void CreateRoom()
    {
        string roomName = roomNameInput.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            Debug.Log($"Request to create room: {roomName}");
            PhotonManager.Instance.CreateRoom(roomName);
        }
        else
        {
            Debug.LogWarning("Room name is empty, cannot create room.");
        }
    }

    void JoinRoom()
    {
        string roomName = roomNameInput.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            Debug.Log($"Request to join room: {roomName}");
            PhotonManager.Instance.JoinRoom(roomName);
        }
        else
        {
            Debug.LogWarning("Room name is empty, cannot join room.");
        }
    }

    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting the game and loading GameScene");
            PhotonNetwork.LoadLevel("GameScene");
        }
        else
        {
            Debug.LogWarning("Only the Master Client can start the game.");
        }
    }
}
