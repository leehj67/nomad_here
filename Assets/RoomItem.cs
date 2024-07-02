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

    public void SetRoomInfo(string roomName, int playerCount, LobbyManager manager, bool isHost)
    {
        this.roomName = roomName;
        roomNameText.text = roomName;
        playerCountText.text = $"{playerCount}/4";
        lobbyManager = manager;

        // 호스트인 경우 참여하기 버튼 비활성화
        joinButton.interactable = !isHost;
    }

    // 이 메서드는 유니티 에디터에서 joinButton의 OnClick 이벤트에 연결
    public void OnJoinRoomButtonClicked()
    {
        if (lobbyManager != null)
        {
            lobbyManager.JoinRoom(roomName);
        }
    }
}
