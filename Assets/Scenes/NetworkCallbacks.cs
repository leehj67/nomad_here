using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkCallbacks : MonoBehaviourPunCallbacks
{
    public void UpdatePlayerCount()
    {
        // 플레이어 수 업데이트 로직
        // 예: roomItem.playerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/4";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount();
    }
}
