using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ShipBoardingZone : MonoBehaviourPun
{
    public static ShipBoardingZone Instance;

    // Master가 관리
    private readonly HashSet<int> boardedActors = new();

    // 로컬에서 Enter/Exit 중복 방지(콜라이더 여러개일 때)
    private int localStayCount = 0;

    void Awake()
    {
        Instance = this;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var pv = col.GetComponentInParent<PhotonView>(); // ✅ 핵심
        if (pv == null) return;
        if (!pv.IsMine) return; // 로컬 플레이어만 보고

        localStayCount++;
        if (localStayCount == 1)
        {
            Debug.Log($"[Boarding][LOCAL] Enter zone. actor={PhotonNetwork.LocalPlayer.ActorNumber}");
            photonView.RPC(nameof(RPC_SetBoarded), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        var pv = col.GetComponentInParent<PhotonView>(); // ✅ 핵심
        if (pv == null) return;
        if (!pv.IsMine) return;

        localStayCount = Mathf.Max(0, localStayCount - 1);
        if (localStayCount == 0)
        {
            Debug.Log($"[Boarding][LOCAL] Exit zone. actor={PhotonNetwork.LocalPlayer.ActorNumber}");
            photonView.RPC(nameof(RPC_SetBoarded), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, false);
        }
    }

    [PunRPC]
    void RPC_SetBoarded(int actorNumber, bool boarded, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (boarded) boardedActors.Add(actorNumber);
        else boardedActors.Remove(actorNumber);

        Debug.Log($"[Boarding][MASTER] actor={actorNumber}, boarded={boarded}  => boardedCount={boardedActors.Count}");

        // 전체에게 count 공유(디버그/표시용)
        photonView.RPC(nameof(RPC_SyncBoardedCount), RpcTarget.All, boardedActors.Count);
    }

    [PunRPC]
    void RPC_SyncBoardedCount(int count)
    {
        Debug.Log($"[Boarding][ALL] boardedCount={count}");
    }

    public int BoardedCount => boardedActors.Count;

    public List<Player> GetBoardedPlayers_Master()
    {
        List<Player> list = new();
        if (!PhotonNetwork.IsMasterClient) return list;

        foreach (var p in PhotonNetwork.PlayerList)
            if (boardedActors.Contains(p.ActorNumber))
                list.Add(p);

        return list;
    }
}
