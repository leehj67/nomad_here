using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput;
    public TMP_InputField passwordInput;
    public Button createRoomButton;
    public Transform scrollViewContent;
    public GameObject roomPrefab;
    public Button startGameButton; // 호스트의 게임 시작 버튼

    private void Start()
    {
        createRoomButton.onClick.AddListener(CreateRoom);
        startGameButton.gameObject.SetActive(false); // 초기에는 비활성화
    }

    void CreateRoom()
    {
        string roomName = roomNameInput.text;
        string password = passwordInput.text;
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "password", password } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };
        PhotonNetwork.CreateRoom(roomName, roomOptions);

        // 방 생성 버튼 비활성화
        createRoomButton.interactable = false;
    }

    public override void OnJoinedRoom()
    {
        // 방에 성공적으로 참가하면 호스트에게 게임 시작 버튼을 표시
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            GameObject newRoom = Instantiate(roomPrefab, scrollViewContent);
            RoomItem roomItem = newRoom.GetComponent<RoomItem>();
            roomItem.roomNameText.text = room.Name;
            roomItem.playerCountText.text = $"{room.PlayerCount}/4";
            roomItem.joinButton.onClick.AddListener(() => OnJoinRoomClicked(room));
        }
    }

    void OnJoinRoomClicked(RoomInfo roomInfo)
    {
        PasswordPanel.Instance.OpenPanel(roomInfo);
    }
}
