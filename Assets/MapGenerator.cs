using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject startPosPrefab;  // StartPos 프리팹 추가
    public int minX = 0;
    public int maxX = 20;
    public int minY = 0;
    public int maxY = 20;
    public float tileWidth = 1.0f;
    public float tileHeight = 1.0f;

    void Start()
    {
        GenerateMap();
        PlaceStartPos();
    }

    void GenerateMap()
    {
        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                GameObject toInstantiate = Random.Range(0, 100) < 20 ? wallPrefab : floorPrefab;
                Vector3 position = new Vector3(x * tileWidth, y * tileHeight, 0);
                Instantiate(toInstantiate, position, Quaternion.identity);
            }
        }
    }

    // StartPos를 랜덤 위치에 배치하는 함수

    // MapGenerator 스크립트 내부
    void PlaceStartPos()
    {
        int startX = Random.Range(minX, maxX);
        int startY = Random.Range(minY, maxY);
        Vector3 startPos = new Vector3(startX * tileWidth, startY * tileHeight, 0);
        Instantiate(startPosPrefab, startPos, Quaternion.identity);
        GameManager.Instance.SetPlayerStartPosition(startPos);
    }

}
