using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance;
    public bool isInLobby = false;
    private string pendingRoomName;
    private bool isTryingToCreateRoom = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Already connected to Photon.");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        isInLobby = true;

        if (isTryingToCreateRoom && !string.IsNullOrEmpty(pendingRoomName))
        {
            Debug.Log($"Creating pending room: {pendingRoomName}");
            CreateRoom(pendingRoomName);
            isTryingToCreateRoom = false;
            pendingRoomName = null;
        }
    }

    public void CreateRoom(string roomName)
    {
        if (isInLobby)
        {
            Debug.Log($"Creating room: {roomName}");
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4 });
        }
        else
        {
            Debug.LogWarning("Not in lobby yet. Can't create room.");
            pendingRoomName = roomName;
            isTryingToCreateRoom = true;
            Debug.Log("Leaving current room to join the lobby.");
            PhotonNetwork.LeaveRoom();
        }
    }

    public void JoinRoom(string roomName)
    {
        if (isInLobby)
        {
            Debug.Log($"Joining room: {roomName}");
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.LogWarning("Not in lobby yet. Can't join room.");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
        // Load the game scene when a room is successfully joined
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Create Room Failed: {message} (Code: {returnCode})");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Room Failed: {message} (Code: {returnCode})");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room. Joining Lobby...");
        PhotonNetwork.JoinLobby();
    }
}
