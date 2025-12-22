using Photon.Pun;
using UnityEngine;

public class InGameDepositUI : MonoBehaviour
{
    [Header("로컬 플레이어 인벤(씬에서 찾아도 됨)")]
    public PlayerInventory myInventory;

    void Start()
    {
        if (myInventory == null)
            myInventory = FindLocalPlayerInventory();

        // InGame 들어올 때 로컬 인벤을 CustomProperties로부터 복원
        RestoreLocalInventoryFromPhoton();
        // TODO: 여기서 네 인벤 UI 슬롯 갱신 호출하면 됨
    }

    public void DepositSlot0() => Deposit(0);
    public void DepositSlot1() => Deposit(1);
    public void DepositSlot2() => Deposit(2);
    public void DepositSlot3() => Deposit(3);

    void Deposit(int slot)
    {
        if (ShipStorage.Instance == null || myInventory == null) return;
        ShipStorage.Instance.RequestDepositSlot(slot, myInventory);

        // 반납 Ack는 RPC로 오지만, UI는 즉시 갱신하고 싶으면 아래처럼 약간 딜레이 후 재복원도 가능
        Invoke(nameof(RestoreLocalInventoryFromPhoton), 0.15f);
    }

    void RestoreLocalInventoryFromPhoton()
    {
        if (myInventory == null) return;

        myInventory.Clear();

        var ids = PlayerInventorySync.LoadLocalIds();
        foreach (var id in ids)
        {
            var data = ItemDatabase.Get(id);
            if (data != null)
                myInventory.Add(data);
        }

        Debug.Log($"[InGameDepositUI] 인벤 복원 완료: {myInventory.items.Count}개");
    }

    PlayerInventory FindLocalPlayerInventory()
    {
        // PlayScene에서 플레이어를 새로 만들면 InGame에도 플레이어가 있을 수도/없을 수도 있음.
        // 만약 InGame에서 플레이어가 없다면, InGame 전용 로컬 인벤 오브젝트를 하나 만들어 붙이는 방식으로 가면 됨.
        foreach (var inv in GameObject.FindObjectsOfType<PlayerInventory>(true))
        {
            var pv = inv.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine) return inv;
        }
        return null;
    }
}
