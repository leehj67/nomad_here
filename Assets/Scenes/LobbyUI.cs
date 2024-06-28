using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;  // This line is crucial to access RoomOptions and related functionalities

public class LobbyUI : MonoBehaviour
{
    public TMP_InputField roomNameInput;
    public Button createRoomButton;
    public Button joinRoomButton;

    void Start()
    {
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
    }

    void CreateRoom()
    {
        string roomName = roomNameInput.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            Debug.Log($"Request to create room: {roomName}");
            PhotonManager.Instance.CreateRoom(roomName);
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
    }
}
