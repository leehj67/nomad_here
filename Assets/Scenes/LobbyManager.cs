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

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

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
        UpdateRoomList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoomList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        cachedRoomList = new List<RoomInfo>(roomList); // 이 부분에서 새로운 리스트를 할당하여 문제를 방지
        UpdateRoomList();
    }

    void UpdateRoomList()
    {
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in cachedRoomList)
        {
            GameObject newRoom = Instantiate(roomPrefab, scrollViewContent);
            RectTransform roomRect = newRoom.GetComponent<RectTransform>();
            roomRect.anchoredPosition = Vector2.zero; // 위치를 초기화

            RoomItem roomItem = newRoom.GetComponent<RoomItem>();
            roomItem.SetRoomInfo(room.Name, room.PlayerCount, this);
        }
    }

    public void JoinRoom(string roomName)
    {
        RoomInfo roomInfo = cachedRoomList.Find(r => r.Name == roomName);
        if (roomInfo != null)
        {
            PasswordPanel.Instance.OpenPanel(roomInfo);
        }
        else
        {
            Debug.LogError("RoomInfo is null or room not found");
        }
    }
}
