using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject roomItemPrefab;
    public Transform roomListParent;
    public TMP_InputField roomNameInput;
    public TMP_InputField playerNameInput; // 플레이어 이름 입력 필드 추가
    public Button createRoomButton;
    public Button startGameButton;
    public GameObject panel;
    private NetworkManager networkManager;
    private bool isConnectedToMaster = false;
    private List<RoomInfo> roomList = new List<RoomInfo>();

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();

        startGameButton.gameObject.SetActive(false);
        panel.SetActive(true); // 시작 시 패널을 활성화 상태로 설정합니다.
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master from RoomManager");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby(); // 로비에 접속합니다.
        }
        isConnectedToMaster = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby from RoomManager");
        UpdateRoomList(roomList); // 로비에 접속할 때 방 목록을 업데이트합니다.
    }

    public override void OnRoomListUpdate(List<RoomInfo> updatedRoomList)
    {
        Debug.Log("Room list updated with " + updatedRoomList.Count + " rooms");
        roomList = updatedRoomList;
        UpdateRoomList(roomList);
    }

    public void CreateRoom()
    {
        Debug.Log("CreateRoom method called");
        if (!string.IsNullOrEmpty(roomNameInput.text) && isConnectedToMaster)
        {
            if (!string.IsNullOrEmpty(playerNameInput.text))
            {
                PhotonNetwork.NickName = playerNameInput.text;
                string roomName = roomNameInput.text;
                Debug.Log("Trying to create room: " + roomName);
                try
                {
                    networkManager.CreateRoom(roomName);
                    Debug.Log("CreateRoom called successfully");
                    // 방 생성 후 로비에 다시 참여
                    PhotonNetwork.JoinLobby();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Failed to create room: " + ex.Message);
                }
            }
            else
            {
                Debug.LogWarning("Player name is empty! Please enter a player name.");
            }
        }
        else
        {
            Debug.LogWarning("Room name is empty or not connected to master!");
        }
    }

    public void JoinRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(playerNameInput.text))
        {
            PhotonNetwork.NickName = playerNameInput.text;
            Debug.Log("Trying to join room: " + roomName);
            networkManager.JoinRoom(roomName);
        }
        else
        {
            Debug.LogWarning("Player name is empty! Please enter a player name.");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom called in RoomManager");
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
        }
        UpdateRoomPlayerCount();
    }

    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            if (room.IsVisible && room.IsOpen) // 방이 공개되고 열려 있는지 확인
            {
                Debug.Log("Creating room item for: " + room.Name);
                GameObject roomItemObject = Instantiate(roomItemPrefab, roomListParent);
                if (roomItemObject != null)
                {
                    RoomItemPrefab roomItem = roomItemObject.GetComponent<RoomItemPrefab>();
                    if (roomItem != null)
                    {
                        roomItem.roomNameText.text = room.Name;
                        roomItem.playersText.text = $"{room.PlayerCount}/{room.MaxPlayers}";
                        roomItem.joinButton.onClick.RemoveAllListeners(); // 기존의 모든 리스너 제거
                        roomItem.joinButton.onClick.AddListener(() => JoinRoom(room.Name)); // 새로운 리스너 추가
                        Debug.Log("Room item created for: " + room.Name);
                    }
                    else
                    {
                        Debug.LogError("RoomItemPrefab component missing in roomItemObject");
                    }
                }
                else
                {
                    Debug.LogError("Failed to instantiate room item prefab");
                }
            }
        }
    }

    private void UpdateRoomPlayerCount()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            Debug.Log("Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 플레이어 수와 상관없이 게임 시작
            Debug.Log("MasterClient starting the game");
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
}
