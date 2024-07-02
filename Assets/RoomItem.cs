using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomNameText;
    public TMP_Text playerCountText;
    public Button joinButton;

    private string roomName;
    private LobbyManager lobbyManager;

    public void SetRoomInfo(string roomName, int playerCount, LobbyManager manager)
    {
        this.roomName = roomName;
        roomNameText.text = roomName;
        playerCountText.text = $"{playerCount}/4";
        lobbyManager = manager;
        joinButton.onClick.AddListener(OnJoinRoomClicked);
    }

    void OnJoinRoomClicked()
    {
        if (lobbyManager != null)
        {
            lobbyManager.JoinRoom(roomName);
        }
    }
}
