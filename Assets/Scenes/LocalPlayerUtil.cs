using UnityEngine;
using Photon.Pun;

public static class LocalPlayerUtil
{
    // 내 로컬 플레이어(Player_Move) 정확히 찾기: FindObjectOfType 금지
    public static Player_Move FindLocalPlayerMove()
    {
        var players = Object.FindObjectsOfType<Player_Move>();
        foreach (var p in players)
        {
            if (p == null) continue;
            var pv = p.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
                return p;
        }
        return null;
    }

    public static PlayerInventory FindLocalInventory()
    {
        var pm = FindLocalPlayerMove();
        return pm != null ? pm.GetComponent<PlayerInventory>() : null;
    }

    public static InventoryManager FindInventoryUI()
    {
        // UI는 로컬 1개가 정상. (중복이면 씬 구성 문제)
        return Object.FindObjectOfType<InventoryManager>();
    }
}
