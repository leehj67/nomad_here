using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby(); // 로비에 접속합니다.
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom(string roomName)
    {
        Debug.Log("Creating room: " + roomName);
        if (PhotonNetwork.InLobby)
        {
            try
            {
                PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4 });
            }
            catch (System.Exception ex)
            {
                Debug.LogError("CreateRoom exception: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("Not in Lobby, cannot create room.");
        }
    }

    public void JoinRoom(string roomName)
    {
        Debug.Log("Joining room: " + roomName);
        try
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("JoinRoom exception: " + ex.Message);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        UpdateRoomPlayerCount();
    }

    private void UpdateRoomPlayerCount()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            Debug.Log("Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }
}
