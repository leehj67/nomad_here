using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GimmickNode : MonoBehaviourPun
{
    [Header("State")]
    public bool solved;

    [Header("Visual")]
    public SpriteRenderer fuseRenderer;   // 🔴 Fuse 스프라이트
    public Color solvedColor = Color.red; // 해결 시 색
    private Color originalColor;

    [Header("Anti Spam")]
    public float requestCooldown = 0.25f;
    private float lastRequestTime = -999f;

    void Awake()
    {
        // 자동 연결 (인스펙터 연결 안 했을 때)
        if (fuseRenderer == null)
            fuseRenderer = GetComponentInChildren<SpriteRenderer>();

        if (fuseRenderer != null)
            originalColor = fuseRenderer.color;
    }

    /// <summary>
    /// 스위치/Fuse 상호작용 진입점
    /// </summary>
    public void MarkSolved()
    {
        if (solved) return;

        if (Time.time - lastRequestTime < requestCooldown) return;
        lastRequestTime = Time.time;

        if (PhotonNetwork.IsMasterClient)
        {
            MasterApproveSolve(PhotonNetwork.LocalPlayer);
            return;
        }

        photonView.RPC(nameof(RPC_RequestSolve),
            RpcTarget.MasterClient,
            PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    private void RPC_RequestSolve(int actorNumber, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (solved) return;

        Player requester = PhotonNetwork.CurrentRoom != null
            ? PhotonNetwork.CurrentRoom.GetPlayer(actorNumber)
            : null;

        if (requester == null)
        {
            Debug.LogWarning($"[GimmickNode] Invalid requester actor={actorNumber}");
            return;
        }

        MasterApproveSolve(requester);
    }

    private void MasterApproveSolve(Player requester)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (solved) return;

        solved = true;
        photonView.RPC(nameof(RPC_SetSolved), RpcTarget.All, true);

        Debug.Log($"[GimmickNode] Solved by MASTER. requester={requester?.ActorNumber}");
    }

    // =========================
    // 🔴 VISUAL SYNC POINT
    // =========================
    [PunRPC]
    private void RPC_SetSolved(bool value)
    {
        solved = value;

        if (fuseRenderer != null)
        {
            fuseRenderer.color = solved ? solvedColor : originalColor;
        }

        // 👉 여기에 사운드/파티클 추가해도 안전
    }

    /// <summary>
    /// 노드 단위 리셋 (다인 동기화)
    /// </summary>
    public void ResetNode_All()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC(nameof(RPC_ResetNode), RpcTarget.All);
    }

    [PunRPC]
    public void RPC_ResetNode()
    {
        solved = false;

        if (fuseRenderer != null)
            fuseRenderer.color = originalColor;
    }
}
