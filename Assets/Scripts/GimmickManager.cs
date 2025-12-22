using Photon.Pun;
using UnityEngine;

public class GimmickManager : MonoBehaviourPun
{
    [Header("Gimmick Nodes (씬에 있는 노드들 드래그)")]
    public GimmickNode[] nodes;

    [Header("탈출 조건(몇 개 풀어야 하는지). 0이면 nodes.Length 자동")]
    public int requiredCount = 3; // ✅ 스위치 3개면 3 고정 추천

    [Header("Main Door Lock (Tutorial)")]
    public Door mainDoor;                // ✅ 테스트용 메인도어 1개 연결
    public bool lockDoorUntilCleared = true;

    private bool lastDoorLockedState = true;
    private bool doorInitSynced = false;

    // ✅ EscapeTimer가 찾는 프로퍼티
    public bool CanEscape
    {
        get
        {
            int req = (requiredCount <= 0) ? (nodes != null ? nodes.Length : 0) : requiredCount;
            return ClearedCount >= req && req > 0;
        }
    }

    public int ClearedCount
    {
        get
        {
            if (nodes == null) return 0;
            int c = 0;
            foreach (var n in nodes)
                if (n != null && n.solved) c++;
            return c;
        }
    }

    void Start()
    {
        // ✅ 씬 시작 시: 기본은 잠금(기믹 풀기 전에는 못 들어감)
        // 다인 기준: 마스터가 1회 전파
        if (PhotonNetwork.IsMasterClient)
        {
            // 초기 상태를 확실히 "리셋 + 잠금"으로 맞추고 시작하고 싶으면 아래 한 줄을 켜도 됨.
            // ResetAll();

            SyncDoorLockState(force: true);
        }
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // (안전) 오브젝트/참조 순서가 늦게 잡히는 경우 대비
        if (!doorInitSynced)
        {
            SyncDoorLockState(force: true);
            doorInitSynced = true;
        }

        // ✅ 기믹 상태 변화에 따른 문 잠금 상태 동기화
        SyncDoorLockState(force: false);
    }

    /// <summary>
    /// ✅ 라운드/씬 재진입 시 초기화용 (다인 동기화)
    /// - 마스터만 호출하도록 권장
    /// - 내부에서 각 노드의 photonView로 "전원 RPC_ResetNode" 호출
    /// - 그리고 문 잠금 상태도 다시 동기화
    /// </summary>
    public void ResetAll()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[GimmickManager] ResetAll() must be called by MASTER.");
            return;
        }

        // 노드 리셋은 각 노드의 PhotonView를 통해 전원에게 동기화
        if (nodes != null)
        {
            foreach (var n in nodes)
            {
                if (n == null) continue;

                var pv = n.GetComponent<PhotonView>();
                if (pv == null)
                {
                    Debug.LogWarning($"[GimmickManager] Node '{n.name}' has no PhotonView. Reset won't sync.");
                    n.RPC_ResetNode(); // fallback (로컬)
                    continue;
                }

                // ✅ 전원 동기화 리셋
                pv.RPC(nameof(GimmickNode.RPC_ResetNode), RpcTarget.All);
            }
        }

        Debug.Log("[GimmickManager] ResetAll() synced via each node PhotonView");

        // 문도 다시 잠금 상태로 맞춤(기믹이 모두 false가 됐으니 shouldLock=true)
        SyncDoorLockState(force: true);
    }

    // =========================
    // Door Lock Sync (Master -> All)
    // =========================
    void SyncDoorLockState(bool force)
    {
        if (!lockDoorUntilCleared) return;
        if (mainDoor == null) return;

        // CanEscape==true이면 문 열림(locked=false)
        bool shouldLock = !CanEscape;

        if (!force && shouldLock == lastDoorLockedState) return;
        lastDoorLockedState = shouldLock;

        // ✅ 전원에게 동일하게 세팅
        photonView.RPC(nameof(RPC_SetMainDoorLocked), RpcTarget.All, shouldLock);
    }

    [PunRPC]
    void RPC_SetMainDoorLocked(bool locked)
    {
        if (mainDoor == null) return;
        mainDoor.locked = locked;
        Debug.Log($"[GimmickManager] MainDoor locked={locked}");
    }
}
