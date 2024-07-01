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
    public TMP_InputField playerNameInput;
    public Button createRoomButton;
    public Button startGameButton;
    public GameObject panel;
    private NetworkManager networkManager;
    private bool isConnectedToMaster = false;

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();

        startGameButton.gameObject.SetActive(false);
        panel.SetActive(true); // 시작 시 패널을 활성화 상태로 설정합니다.
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master from RoomManager");
        PhotonNetwork.JoinLobby();
        isConnectedToMaster = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby from RoomManager");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated with " + roomList.Count + " rooms");
        UpdateRoomList(roomList);
    }

    public void CreateRoom()
    {
        Debug.Log("CreateRoom method called");
        if (!string.IsNullOrEmpty(playerNameInput.text) && isConnectedToMaster)
        {
            PhotonNetwork.NickName = playerNameInput.text;
            string roomName = "Room_" + playerNameInput.text;
            Debug.Log("Trying to create room: " + roomName);
            try
            {
                networkManager.CreateRoom(roomName);
                Debug.Log("CreateRoom called successfully");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to create room: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("Player name is empty or not connected to master!");
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
            Debug.LogWarning("Player name is empty!");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom called in RoomManager");
        panel.SetActive(true); // 방에 들어간 후에도 패널을 활성화 상태로 유지합니다.
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
        }
    }

    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in roomList)
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
                    roomItem.joinButton.onClick.AddListener(() => JoinRoom(room.Name));
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

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("MasterClient starting the game");
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
}
