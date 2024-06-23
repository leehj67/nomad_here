using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class RoomListUI : MonoBehaviourPunCallbacks
{
    public GameObject roomListItemPrefab;
    public Transform roomListContent;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }
        UpdateRoomListUI();
    }

    void UpdateRoomListUI()
    {
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo roomInfo in cachedRoomList.Values)
        {
            GameObject roomListItem = Instantiate(roomListItemPrefab, roomListContent);
            roomListItem.GetComponentInChildren<Text>().text = roomInfo.Name;
            roomListItem.GetComponent<Button>().onClick.AddListener(() => JoinRoom(roomInfo.Name));
        }
    }

    void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Room Failed: {message} (Code: {returnCode})");
    }
}
