using System.Collections;
using UnityEngine;
using Photon.Pun;

public class ItemSpawner : MonoBehaviour
{
    [Header("스폰할 아이템 프리팹들 (WorldItem Prefab들)")]
    public GameObject[] itemPrefabs;   // 각 프리팹에는 PhotonView + WorldItem 붙어 있어야 함

    [Header("초기 스폰 개수")]
    public int initialSpawnCount = 10;

    [Header("주기 스폰 설정 (0이면 주기 스폰 없음)")]
    public float spawnInterval = 0f;   // 예: 10f로 두면 10초마다 스폰

    [Header("스폰 영역 (월드 좌표 박스)")]
    public Vector2 minPos = new Vector2(-8f, -4f);
    public Vector2 maxPos = new Vector2( 8f,  4f);

    void Start()
    {
        // 아이템 스폰은 마스터 클라이언트만 담당
        if (!PhotonNetwork.IsMasterClient)
            return;

        // 1) 시작할 때 한번 쫙 뿌리기
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnOneItem();
        }

        // 2) 간헐적으로 계속 뿌리고 싶으면
        if (spawnInterval > 0f)
        {
            StartCoroutine(SpawnLoop());
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnOneItem();
        }
    }

    void SpawnOneItem()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0)
            return;

        // 랜덤 프리팹 선택
        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

        // 랜덤 위치
        float x = Random.Range(minPos.x, maxPos.x);
        float y = Random.Range(minPos.y, maxPos.y);
        Vector3 pos = new Vector3(x, y, 0);

        // Photon으로 아이템 생성 → 모든 클라이언트에 동기화
        PhotonNetwork.Instantiate(prefab.name, pos, Quaternion.identity);
    }
}
