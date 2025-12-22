using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public static class PlayerInventorySync
{
    private const string KEY_INV = "INV";

    // 형주 코드 호환: Save(List<ItemData>)
    public static void Save(List<ItemData> items)
    {
        if (!PhotonNetwork.InRoom) return;

        List<string> ids = new List<string>();
        if (items != null)
        {
            foreach (var it in items)
                if (it != null && !string.IsNullOrEmpty(it.itemId))
                    ids.Add(it.itemId);
        }

        Hashtable ht = new Hashtable { { KEY_INV, ids.ToArray() } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
    }

    // 로컬 로드
    public static List<string> LoadLocalIds()
    {
        return LoadIds(PhotonNetwork.LocalPlayer);
    }

    // ✅ 특정 Player 로드(ShipStorage 등에서 사용)
    public static List<string> LoadIds(Player p)
    {
        if (p == null) return new List<string>();

        if (p.CustomProperties != null &&
            p.CustomProperties.TryGetValue(KEY_INV, out object obj) &&
            obj is string[] arr)
        {
            return new List<string>(arr);
        }

        return new List<string>();
    }

    // ✅ ShipStorage가 찾는 이름(호환 alias)
    public static List<string> LoadIdsOf(Player p)
    {
        return LoadIds(p);
    }

    // 로컬 비우기
    public static void ClearLocal()
    {
        if (!PhotonNetwork.InRoom) return;

        Hashtable ht = new Hashtable { { KEY_INV, new string[0] } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
    }

    // (옵션) 마스터가 전원 비우기
    public static void ClearAllPlayers_Master()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (var p in PhotonNetwork.PlayerList)
        {
            Hashtable ht = new Hashtable { { KEY_INV, new string[0] } };
            p.SetCustomProperties(ht);
        }
    }
}
