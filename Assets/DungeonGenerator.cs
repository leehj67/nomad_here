using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public int width = 50;
    public int height = 50;
    public int minRoomSize = 5;
    public int maxLeafSize = 20; // 최대 분할 크기
    public GameManager gameManager; // GameManager 참조

    private void Start()
    {
        Leaf root = new Leaf(0, 0, width, height);
        List<Leaf> leaves = new List<Leaf> { root };

        bool didSplit = true;
        // 공간을 계속 분할할 수 있는 동안 반복
        while (didSplit)
        {
            didSplit = false;
            List<Leaf> newLeaves = new List<Leaf>();
            foreach (var leaf in leaves)
            {
                if (leaf.leftChild == null && leaf.rightChild == null) // 분할할 수 있는지 확인
                {
                    if (leaf.width > maxLeafSize || leaf.height > maxLeafSize || Random.Range(0f, 1f) > 0.25)
                    {
                        if (leaf.Split(minRoomSize)) // 분할 시도
                        {
                            newLeaves.Add(leaf.leftChild);
                            newLeaves.Add(leaf.rightChild);
                            didSplit = true;
                        }
                    }
                }
            }
            leaves.AddRange(newLeaves);
        }

        // 각 리프에 방을 생성
        foreach (var leaf in leaves)
        {
            leaf.CreateRooms(wallPrefab, floorPrefab);
        }

        // 무작위 방을 선택하여 시작 위치로 설정
        Leaf startLeaf = leaves[Random.Range(0, leaves.Count)];
        Vector3 startRoomCenter = new Vector3(startLeaf.room.x + startLeaf.room.width / 2, startLeaf.room.y + startLeaf.room.height / 2, 0);
        gameManager.SetPlayerStartPosition(startRoomCenter);
    }
}

public class Leaf
{
    public int x, y, width, height;
    public Leaf leftChild, rightChild;
    public Rect room; // 방의 사각형

    public Leaf(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    // 리프를 분할하는 함수
    public bool Split(int minSize)
    {
        if (leftChild != null || rightChild != null)
            return false; // 이미 분할됨

        bool splitH = Random.Range(0f, 1f) > 0.5;
        if (width > height && width / height >= 1.25)
            splitH = false;
        else if (height > width && height / width >= 1.25)
            splitH = true;

        int max = (splitH ? height : width) - minSize;
        if (max <= minSize)
            return false;

        int split = Random.Range(minSize, max);
        if (splitH)
        {
            leftChild = new Leaf(x, y, width, split);
            rightChild = new Leaf(x, y + split, width, height - split);
        }
        else
        {
            leftChild = new Leaf(x, y, split, height);
            rightChild = new Leaf(x + split, y, width - split, height);
        }
        return true;
    }

    // 리프 내에 방을 생성
    public void CreateRooms(GameObject wallPrefab, GameObject floorPrefab)
    {
        room = new Rect(x + 1, y + 1, width - 2, height - 2); // 방 크기는 리프보다 약간 작게 설정
        // 방의 바닥과 벽을 생성하는 로직 추가 필요
    }
}
