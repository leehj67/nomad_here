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
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom(string roomName)
    {
        Debug.Log("Creating room: " + roomName);
        try
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4 });
        }
        catch (System.Exception ex)
        {
            Debug.LogError("CreateRoom exception: " + ex.Message);
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
    }
}
