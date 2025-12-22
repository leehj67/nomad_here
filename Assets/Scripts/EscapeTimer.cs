using System.Collections;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EscapeTimer : MonoBehaviourPun
{
    [Header("플레이 제한 시간(초)")]
    public float duration = 180f;

    [Header("시간이 끝나면 이동할 씬")]
    public string successScene = "GameScene";
    public string failScene = "EndScene";

    private float t;
    private bool ended = false;

    void Start()
    {
        t = duration;
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (ended) return;

        t -= Time.deltaTime;
        if (t <= 0f)
        {
            ended = true;
            Resolve();
        }
    }

    void Resolve()
    {
        int boarded = (ShipBoardingZone.Instance != null) ? ShipBoardingZone.Instance.BoardedCount : 0;

        // ✅ static Instance 제거 대응: 씬에서 직접 찾기
        var gimmick = FindObjectOfType<GimmickManager>();
        bool canEscape = (gimmick != null) && gimmick.CanEscape;

        Debug.Log($"[EscapeTimer] End. boarded={boarded}, canEscape={canEscape}");

        if (boarded <= 0)
        {
            Debug.Log("[EscapeTimer][MASTER] FAIL: boarded == 0");
            StartCoroutine(Co_TransitionToScene(failScene));
            return;
        }

        if (!canEscape)
        {
            Debug.Log("[EscapeTimer][MASTER] FAIL: gimmick not cleared");
            StartCoroutine(Co_TransitionToScene(failScene));
            return;
        }

        Debug.Log("[EscapeTimer][MASTER] SUCCESS: boarded >=1 && gimmick cleared");

        // 성공 => 탑승자 보너스 플래그만 남김(정산은 추후)
        GrantCarryBonusToBoardedPlayers();

        Debug.Log($"[EscapeTimer][MASTER] Resolve() boarded={boarded}, canEscape={canEscape}, required={gimmick?.requiredCount}");

        StartCoroutine(Co_TransitionToScene(successScene));
    }

    IEnumerator Co_TransitionToScene(string sceneName)
    {
        if (!PhotonNetwork.IsMasterClient) yield break;

        // ✅ A안 핵심: 씬 전환 직전에 전원 정리 + 자기 플레이어 Destroy
        photonView.RPC(nameof(RPC_PrepareSceneChange), RpcTarget.All);

        // Destroy / UI정리가 네트워크/프레임에 반영될 시간을 조금 줌
        yield return new WaitForSeconds(0.2f);

        PhotonNetwork.LoadLevel(sceneName);
    }

    [PunRPC]
    void RPC_PrepareSceneChange()
    {
        // 1) 로컬 인벤 비우기
        var inv = LocalPlayerUtil.FindLocalInventory();
        if (inv != null) inv.Clear();

        // 2) 로컬 UI 비우기 (InventoryManager에 맞게 함수명 교체)
        var ui = LocalPlayerUtil.FindInventoryUI();
        if (ui != null)
        {
            // ❗ InventoryManager에 ClearAll()이 없다면,
            //    네 UI 구조에 맞는 초기화 함수로 바꿔줘.
            //    예: ui.RefreshEmpty(); 또는 ui.ClearSlots();
            // ui.ClearAll();
        }

        // 3) ✅ A안: 내 소유 플레이어는 씬 나가기 전에 Destroy (중복 Player_Move 원천 차단)
        var pm = LocalPlayerUtil.FindLocalPlayerMove();
        if (pm != null)
        {
            var pv = pm.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                PhotonNetwork.Destroy(pm.gameObject);
            }
        }

        // 4) (선택) 플레이 중 CustomProperties 인벤 흔적도 정리하고 싶으면 로컬만 비움
        // PlayerInventorySync.ClearLocal();
    }

    void GrantCarryBonusToBoardedPlayers()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (ShipBoardingZone.Instance == null) return;

        var boardedPlayers = ShipBoardingZone.Instance.GetBoardedPlayers_Master();
        foreach (var p in boardedPlayers)
        {
            var ht = new ExitGames.Client.Photon.Hashtable
{
    ["CARRY_BONUS"] = true,
    ["CARRY_BONUS_DAY"] = 1
};
p.SetCustomProperties(ht);

        }

        Debug.Log($"[CarryBonus] Granted to {boardedPlayers.Count} players");
    }
}
