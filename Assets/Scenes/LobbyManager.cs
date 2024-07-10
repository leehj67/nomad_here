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
    private new PhotonView photonView; // PhotonView 객체 선언

    private void Start()
    {
        photonView = GetComponent<PhotonView>(); // PhotonView 컴포넌트 가져오기

        if (photonView == null)
        {
            Debug.LogError("PhotonView is not attached to the LobbyManager object.");
            return;
        }

        createRoomButton.onClick.AddListener(CreateRoom);
        startGameButton.onClick.AddListener(StartGame); // 게임 시작 버튼 리스너 추가
        startGameButton.gameObject.SetActive(false); // 초기에는 비활성화

        // 로비에 참가
        PhotonNetwork.JoinLobby();
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
        // 플레이어가 방에 들어올 때 방 목록 갱신
        UpdateRoomList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 플레이어가 방을 나갈 때 방 목록 갱신
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
        // 중복된 방 목록 제거
        cachedRoomList.RemoveAll(room => !room.IsVisible || room.RemovedFromList);
        
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // 현재 참가한 방도 추가
        if (PhotonNetwork.InRoom)
        {
            GameObject currentRoom = Instantiate(roomPrefab, scrollViewContent);
            RectTransform roomRect = currentRoom.GetComponent<RectTransform>();
            roomRect.anchoredPosition = new Vector2(0, -roomRect.sizeDelta.y * 0); // 위치를 정렬하여 설정

            RoomItem roomItem = currentRoom.GetComponent<RoomItem>();
            roomItem.SetRoomInfo(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom.PlayerCount, this, PhotonNetwork.IsMasterClient);

            // 디버그 로그 추가
            Debug.Log($"Current room: {PhotonNetwork.CurrentRoom.Name}, Players: {PhotonNetwork.CurrentRoom.PlayerCount}");
        }

        for (int i = 0; i < cachedRoomList.Count; i++)
        {
            RoomInfo room = cachedRoomList[i];
            // 현재 참가한 방과 중복되지 않도록 확인
            if (PhotonNetwork.InRoom && room.Name == PhotonNetwork.CurrentRoom.Name)
            {
                continue;
            }

            GameObject newRoom = Instantiate(roomPrefab, scrollViewContent);
            RectTransform roomRect = newRoom.GetComponent<RectTransform>();
            roomRect.anchoredPosition = new Vector2(0, -roomRect.sizeDelta.y * (i + 1)); // 위치를 정렬하여 설정

            RoomItem roomItem = newRoom.GetComponent<RoomItem>();
            roomItem.SetRoomInfo(room.Name, room.PlayerCount, this, PhotonNetwork.IsMasterClient);

            // 디버그 로그 추가
            Debug.Log($"Room added: {room.Name}, Players: {room.PlayerCount}");
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

    public void StartGame() // public으로 변경
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("LoadGameScene", RpcTarget.All); // 모든 클라이언트에 게임 씬 로드를 요청
        }
    }

    [PunRPC]
    void LoadGameScene()
    {
        PhotonNetwork.LoadLevel("GameScene"); // 여기서 "GameScene"은 실제 게임 씬의 이름이어야 합니다.
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        PhotonNetwork.GetCustomRoomList(TypedLobby.Default, ""); // 방 목록 갱신 요청
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left Lobby");
    }
}
