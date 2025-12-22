using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class CarryBonusApplier : MonoBehaviour
{
    [Header("보너스 수치(원하는대로 조절)")]
    public float speedBonusMultiplier = 1.10f;  // +10%
    public float staminaBonusMultiplier = 1.15f; // +15%

    void Start()
    {
        ApplyIfHaveBonus();
    }

    void ApplyIfHaveBonus()
    {
        if (!PhotonNetwork.IsConnected) return;

        bool has = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("CARRY_BONUS", out object v) &&
                   v is bool b && b;

        if (!has) return;

        // 로컬 플레이어 찾아서 적용
        var myPlayer = FindLocalPlayerMove();
        if (myPlayer != null)
        {
            myPlayer.Speed *= speedBonusMultiplier;
            myPlayer.maxStamina *= staminaBonusMultiplier;

            Debug.Log("[CarryBonus] Applied to local player.");
        }

        // 1회 적용 후 제거
        Hashtable ht = new Hashtable
        {
            ["CARRY_BONUS"] = false,
            ["CARRY_BONUS_DAY"] = 0
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
    }

    Player_Move FindLocalPlayerMove()
    {
        foreach (var pm in GameObject.FindObjectsOfType<Player_Move>())
        {
            var pv = pm.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine) return pm;
        }
        return null;
    }
}
