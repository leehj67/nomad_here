using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ShipStorage : MonoBehaviourPun
{
    public static ShipStorage Instance;

    private readonly Dictionary<ItemType, int> counts = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        counts.Clear();
        foreach (ItemType t in System.Enum.GetValues(typeof(ItemType)))
            counts[t] = 0;
    }

    public int GetCount(ItemType t) => counts.TryGetValue(t, out var v) ? v : 0;

    // === 반납 요청: 클라 -> 마스터 ===
    public void RequestDepositSlot(int slotIndex, PlayerInventory localInv)
    {
        if (localInv == null) return;
        var item = localInv.Get(slotIndex);
        if (item == null) return;

        // itemId만 마스터에게 전달
        photonView.RPC(nameof(RPC_DepositItem), RpcTarget.MasterClient,
            PhotonNetwork.LocalPlayer.ActorNumber,
            item.itemId,
            slotIndex
        );
    }

    // 마스터가 처리: 공용창고 누적 + 해당 플레이어 인벤(CustomProperties)에서 제거 승인
    [PunRPC]
    void RPC_DepositItem(int actorNumber, string itemId, int clientSlotIndex, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Player p = FindPlayerByActor(actorNumber);
        if (p == null) return;

        // 플레이어의 CustomProperties에서 인벤 목록 읽기
        var ids = PlayerInventorySync.LoadIdsOf(p);
        if (ids.Count == 0) return;

        // 같은 itemId 하나 제거(첫 매칭)
        int idx = ids.FindIndex(x => x == itemId);
        if (idx < 0) return;

        ids.RemoveAt(idx);

        // 공용 창고 누적
        ItemData data = ItemDatabase.Get(itemId);
        if (data == null) return;

        counts[data.type] += 1;

        // 마스터가 해당 플레이어 CustomProperties 갱신
        var ht = new ExitGames.Client.Photon.Hashtable { ["INV"] = ids.ToArray() };
        p.SetCustomProperties(ht);

        // 해당 플레이어에게 로컬 UI 갱신 요청(ack)
        photonView.RPC(nameof(RPC_DepositAck), p, itemId);
    }

    [PunRPC]
    void RPC_DepositAck(string itemId)
    {
        // 이 RPC는 "반납한 사람" 클라에서 실행됨
        Debug.Log($"[ShipStorage] 반납 완료: {itemId}");
        // 로컬 인벤/UI 다시 로드해서 갱신하면 됨 (아래 InGameDepositUI에서 처리)
    }

    Player FindPlayerByActor(int actor)
    {
        foreach (var pl in PhotonNetwork.PlayerList)
            if (pl.ActorNumber == actor) return pl;
        return null;
    }
}
