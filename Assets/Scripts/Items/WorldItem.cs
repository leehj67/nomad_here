using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class WorldItem : MonoBehaviourPun
{
    public ItemData data;

    bool picked = false;

    public void RequestPickup()
    {
        if (picked) return;
        photonView.RPC(nameof(RPC_RequestPickup), RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_RequestPickup(PhotonMessageInfo info)
    {
        if (picked) return;
        picked = true;

        // ✅ 요청자에게 먼저 주고(성공 여부를 다시 받음)
        photonView.RPC(nameof(RPC_GiveItem), info.Sender, data.itemId, photonView.ViewID);
    }

    [PunRPC]
    void RPC_GiveItem(string itemId, int worldViewId, PhotonMessageInfo info)
    {
        // 이 RPC는 "줍는 사람" 클라에서 실행됨

        // ✅ 내 로컬 플레이어 정확히 잡기(중복 Player_Move 방지)
        var pm  = LocalPlayerUtil.FindLocalPlayerMove();
        var inv = pm != null ? pm.GetComponent<PlayerInventory>() : null;
        var ui  = LocalPlayerUtil.FindInventoryUI();

        if (inv == null || ui == null)
        {
            photonView.RPC(nameof(RPC_PickResult), RpcTarget.MasterClient, worldViewId, false);
            return;
        }

        ItemData d = ItemDatabase.Get(itemId);
        if (d == null)
        {
            photonView.RPC(nameof(RPC_PickResult), RpcTarget.MasterClient, worldViewId, false);
            return;
        }

        if (!inv.Add(d))
        {
            Debug.Log("[Pickup] 인벤 가득 참");
            photonView.RPC(nameof(RPC_PickResult), RpcTarget.MasterClient, worldViewId, false);
            return;
        }

        ui.AddItem(d);

        // ✅ A안 안정화: 플레이 중 CustomProperties 저장은 하지 않음
        // PlayerInventorySync.Save(inv.items);

        // ✅ 성공 통보
        photonView.RPC(nameof(RPC_PickResult), RpcTarget.MasterClient, worldViewId, true);
    }

    [PunRPC]
    void RPC_PickResult(int worldViewId, bool ok)
    {
        // 마스터에서 실행
        if (!PhotonNetwork.IsMasterClient) return;

        if (!ok)
        {
            picked = false; // 다시 주울 수 있게 복구
            return;
        }

        // ✅ 성공일 때만 월드 아이템 제거
        PhotonView pv = PhotonView.Find(worldViewId);
        if (pv != null)
            PhotonNetwork.Destroy(pv.gameObject);
    }
}
