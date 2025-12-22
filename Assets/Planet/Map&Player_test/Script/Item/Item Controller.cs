using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class ItemController : MonoBehaviourPun
{
    [Header("맵 루트(예: map). 비워두면 이름 'map' 자동 탐색")]
    public Transform mapRoot;

    [Header("Square 부모(예: map/square). 비워두면 mapRoot 아래서 'square' 자동 탐색")]
    public Transform squareRoot;

    [Header("아이템 프리팹들 (Resources 폴더에 있어야 PhotonNetwork.Instantiate 가능)")]
    public GameObject[] itemPrefabs;

    [Header("뿌릴 아이템 개수")]
    public int spawnCount = 10;

    [Header("같은 바닥칸 중복 스폰 방지")]
    public bool noDuplicateSquares = true;

    [Header("Square 이름 필터(기본: 'Square'로 시작)")]
    public string squareNameStartsWith = "Square";

    [Header("Square 중심 랜덤 오프셋(자연스러움/겹침 방지)")]
    public float randomOffsetRadius = 0.1f;

    private List<Transform> squareTiles = new();

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        CollectSquareTiles();
        SpawnItemsOnSquares();
    }

    void CollectSquareTiles()
    {
        if (mapRoot == null)
        {
            var mapObj = GameObject.Find("map");
            if (mapObj != null) mapRoot = mapObj.transform;
        }

        if (mapRoot == null)
        {
            Debug.LogError("[ItemController] mapRoot를 찾지 못했습니다. 씬에 'map' 오브젝트가 있는지 확인하세요.");
            return;
        }

        if (squareRoot == null)
        {
            var child = mapRoot.GetComponentsInChildren<Transform>(true)
                               .FirstOrDefault(t => t.name.ToLower() == "square");
            if (child != null) squareRoot = child;
        }

        Transform searchRoot = squareRoot != null ? squareRoot : mapRoot;

        squareTiles = searchRoot.GetComponentsInChildren<Transform>(true)
                                .Where(t => t != searchRoot && t.name.StartsWith(squareNameStartsWith))
                                .ToList();

        Debug.Log($"[ItemController] Square 후보 수집: {squareTiles.Count}개 (searchRoot={searchRoot.name})");
    }

    void SpawnItemsOnSquares()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogError("[ItemController] itemPrefabs가 비어있습니다(최소 1개).");
            return;
        }

        if (squareTiles == null || squareTiles.Count == 0)
        {
            Debug.LogError("[ItemController] Square 후보가 없습니다. squareRoot/이름 필터를 확인하세요.");
            return;
        }

        int actualSpawnCount = Mathf.Min(spawnCount, squareTiles.Count);

        if (noDuplicateSquares)
        {
            Shuffle(squareTiles);
            for (int i = 0; i < actualSpawnCount; i++)
                SpawnOneAt(squareTiles[i].position);
        }
        else
        {
            for (int i = 0; i < spawnCount; i++)
            {
                var pick = squareTiles[Random.Range(0, squareTiles.Count)];
                SpawnOneAt(pick.position);
            }
        }
    }

    void SpawnOneAt(Vector3 basePos)
    {
        Vector2 off = Random.insideUnitCircle * randomOffsetRadius;
        Vector3 spawnPos = new Vector3(basePos.x + off.x, basePos.y + off.y, 0f);

        var prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        if (prefab == null) return;

        PhotonNetwork.Instantiate(prefab.name, spawnPos, Quaternion.identity);
    }

    void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
