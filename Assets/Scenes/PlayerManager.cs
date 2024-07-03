using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public GameObject[] playerSprites; // 1~4번 스프라이트

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            UpdatePlayerSprites();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerSprites();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerSprites();
    }

    private void UpdatePlayerSprites()
    {
        int playerCount = PhotonNetwork.PlayerList.Length;
        if (playerCount <= playerSprites.Length)
        {
            for (int i = 0; i < playerSprites.Length; i++)
            {
                playerSprites[i].SetActive(i < playerCount);
            }
        }
    }
}
