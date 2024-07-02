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
    public PasswordPanel passwordPanel; // 패스워드 패널

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    private void Start()
    {
        createRoomButton.onClick.AddListener(CreateRoom);
        startGameButton.onClick.AddListener(StartGame); // 게임 시작 버튼 리스너 추가
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
        else
        {
            startGameButton.gameObject.SetActive(false);
        }

        // 방 목록을 갱신
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
        Debug.Log($"OnRoomListUpdate called with {roomList.Count} rooms.");
        UpdateRoomList();
    }

    void UpdateRoomList()
    {
        // 현재 참가한 방 정보를 추가
        if (PhotonNetwork.InRoom)
        {
            foreach (Transform child in scrollViewContent)
            {
                Destroy(child.gameObject);
            }

            GameObject newRoom = Instantiate(roomPrefab, scrollViewContent);
            RectTransform roomRect = newRoom.GetComponent<RectTransform>();
            roomRect.anchoredPosition = Vector2.zero; // 위치를 초기화

            RoomItem roomItem = newRoom.GetComponent<RoomItem>();
            roomItem.SetRoomInfo(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom.PlayerCount, this, PhotonNetwork.IsMasterClient);

            // 디버그 로그 추가
            Debug.Log($"Current room: {PhotonNetwork.CurrentRoom.Name}, Players: {PhotonNetwork.CurrentRoom.PlayerCount}");
        }
        else
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
                roomItem.SetRoomInfo(room.Name, room.PlayerCount, this, PhotonNetwork.IsMasterClient);

                // 디버그 로그 추가
                Debug.Log($"Room added: {room.Name}, Players: {room.PlayerCount}");
            }
        }
    }

    public void JoinRoom(string roomName)
    {
        if (cachedRoomList == null)
        {
            Debug.LogError("cachedRoomList is null");
            return;
        }

        // 디버그 로그 추가
        Debug.Log($"Attempting to join room: {roomName}");

        foreach (var room in cachedRoomList)
        {
            Debug.Log($"Room in cachedRoomList: {room.Name}");
        }

        RoomInfo roomInfo = cachedRoomList.Find(r => r.Name == roomName);
        if (roomInfo != null)
        {
            if (passwordPanel != null)
            {
                passwordPanel.OpenPanel(roomInfo);
            }
            else
            {
                Debug.LogError("passwordPanel is null");
            }
        }
        else
        {
            Debug.LogError($"RoomInfo is null or room not found for roomName: {roomName}");
        }
    }

    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene"); // 여기서 "GameScene"은 실제 게임 씬의 이름이어야 합니다.
        }
    }
}
