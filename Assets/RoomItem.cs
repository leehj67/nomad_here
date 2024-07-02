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
        joinButton.onClick.RemoveAllListeners(); // 이전에 할당된 모든 리스너 제거
        joinButton.onClick.AddListener(OnJoinRoomClicked); // 새로운 리스너 추가
    }

    void OnJoinRoomClicked()
    {
        if (lobbyManager != null)
        {
            lobbyManager.JoinRoom(roomName);
        }
    }
}
