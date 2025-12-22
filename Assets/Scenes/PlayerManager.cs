using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public GameObject[] playerSprites;

    private void Start()
    {
        TryUpdate();
    }

    public override void OnJoinedRoom() => TryUpdate();
    public override void OnPlayerEnteredRoom(Player newPlayer) => TryUpdate();
    public override void OnPlayerLeftRoom(Player otherPlayer) => TryUpdate();
    public override void OnMasterClientSwitched(Player newMasterClient) => TryUpdate();

    void TryUpdate()
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        UpdatePlayerSprites();
    }

    private void UpdatePlayerSprites()
    {
        int playerCount = PhotonNetwork.PlayerList.Length;
        for (int i = 0; i < playerSprites.Length; i++)
            playerSprites[i].SetActive(i < playerCount);
    }
}
