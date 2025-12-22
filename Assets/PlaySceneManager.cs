using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviourPunCallbacks
{
    [Header("플레이 시간")]
    public float playDuration = 60f;
    private float timer;
    private bool isReturningToGameScene = false;

    [Header("프리팹")]
    public GameObject playerPrefab;
    public GameObject uiPrefab;
    public GameObject monsterSpawnerPrefab; // 몬스터는 Photon으로 (필요 시)

    void Start()
    {
        timer = playDuration;

        // (안전) ItemDatabase 존재 체크 - 없으면 인벤 복원이 실패함
        if (ItemDatabase.Instance == null)
        {
            Debug.LogWarning("[PlaySceneManager] ItemDatabase.Instance가 null입니다. " +
                             "PlayScene에 ItemDatabase 오브젝트가 있는지, allItems에 ItemData가 등록됐는지 확인하세요.");
        }

        // 1) Player 생성 (Photon)
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, GetSpawnPosition(), Quaternion.identity);
        Player_Move pm = player.GetComponent<Player_Move>();

        // 2) UI 생성 (로컬)
        GameObject uiObject = Instantiate(uiPrefab);
        GameUIRoot uiRoot = uiObject.GetComponent<GameUIRoot>();

        pm.joystick          = uiRoot.joystick;
        pm.inventoryUI       = uiRoot.inventoryManager;
        pm.staminaImage      = uiRoot.staminaImage;
        pm.lowStaminaOverlay = uiRoot.lowStaminaOverlay;
        pm.flashlightIcon    = uiRoot.flashlightIcon;

        uiRoot.inventoryManager.dropPoint = player.transform;

        // ✅ 3) (핵심) Photon CustomProperties에서 "개인 인벤" 복원
        RestoreInventoryFromPhoton(player, uiRoot);

        // 4) 스포너들 (마스터만)
        if (PhotonNetwork.IsMasterClient)
        {
            // ✅ (추가) PlayScene 진입 시 기믹은 항상 초기화 (다인 동기화)
            TryResetGimmicks_MasterOnce();

            if (monsterSpawnerPrefab != null)
            {
                PhotonNetwork.Instantiate(monsterSpawnerPrefab.name, Vector3.zero, Quaternion.identity);
            }
            // ✅ 아이템 스포너는 씬 고정 존재(ItemSpawnManager)라 여기서 생성하지 않음
        }
    }

    /// <summary>
    /// 마스터가 PlayScene 시작 시 기믹을 1회 초기화한다.
    /// (GimmickManager.ResetAll()은 내부에서 RPC로 전원 동기화됨)
    /// </summary>
    private void TryResetGimmicks_MasterOnce()
    {
        // 씬에 GimmickManager가 없을 수도 있으니 안전 처리
        var gimmick = FindObjectOfType<GimmickManager>();
        if (gimmick == null)
        {
            Debug.LogWarning("[PlaySceneManager] GimmickManager를 찾지 못했습니다. (씬에 오브젝트가 있는지 확인)");
            return;
        }

        gimmick.ResetAll();
        Debug.Log("[PlaySceneManager] Master triggered gimmick ResetAll()");
    }

    /// <summary>
    /// Photon Player CustomProperties("INV")에 저장된 itemId들을 읽어 PlayerInventory에 복원하고,
    /// 필요 시 인벤 UI 갱신 트리거까지 호출할 수 있게 연결해둔다.
    /// </summary>
    void RestoreInventoryFromPhoton(GameObject player, GameUIRoot uiRoot)
    {
        var inv = player.GetComponent<PlayerInventory>();
        if (inv == null)
        {
            Debug.LogWarning("[PlaySceneManager] PlayerInventory 컴포넌트가 플레이어 프리팹에 없습니다. " +
                             "playerPrefab에 PlayerInventory를 추가하세요.");
            return;
        }

        inv.Clear();

        var ids = PlayerInventorySync.LoadLocalIds();
        foreach (var id in ids)
        {
            var data = ItemDatabase.Get(id);
            if (data != null)
                inv.Add(data);
            else
                Debug.LogWarning($"[PlaySceneManager] ItemDatabase에서 itemId='{id}'를 찾지 못했습니다. ItemData 등록을 확인하세요.");
        }

        Debug.Log($"[PlaySceneManager] 인벤 복원 완료: {inv.items.Count}개");

        // ✅ (선택) 네 인벤 UI가 "리스트로 갱신"을 지원하면 여기서 호출
        // uiRoot.inventoryManager.RefreshFromInventory(inv.items);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && !isReturningToGameScene)
        {
            isReturningToGameScene = true;
            CheckResourcesAndReturn();
        }
    }

    private void CheckResourcesAndReturn()
    {
        GameStateManager gameStateManager = GameStateManager.Instance;
        if (gameStateManager != null)
        {
            if (gameStateManager.ShipFood <= 0 ||
                gameStateManager.ShipParts <= 0 ||
                gameStateManager.ShipEnergy <= 0)
            {
                Debug.Log("Game Over: One or more resources depleted. Transitioning to EndScene.");
                PhotonNetwork.LoadLevel("EndScene");
            }
            else
            {
                ReturnToGameScene();
            }
        }
        else
        {
            Debug.LogError("GameStateManager instance not found. Cannot check resources.");
            ReturnToGameScene();
        }
    }

    private void ReturnToGameScene()
    {
        Debug.Log("Returning to GameScene...");
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        PhotonNetwork.LoadLevel("GameScene");
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            Debug.Log("GameScene loaded, advancing day...");
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.AdvanceDay();
                GameStateManager.Instance.ShowDayPanel();
                GameStateManager.Instance.ShowTimerPanel();
            }
            SceneManager.sceneLoaded -= OnGameSceneLoaded;
        }
    }

    Vector3 GetSpawnPosition()
    {
        // 원본 유지
        return new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.IsMasterClient)
        {
            photonView.RPC("OnHostDisconnected", RpcTarget.Others);
        }
    }

    [PunRPC]
    private void OnHostDisconnected()
    {
        Debug.Log("Host has disconnected. Transitioning to EndScene.");
        PhotonNetwork.LoadLevel("EndScene");
    }
}
