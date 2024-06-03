using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public int width = 50;
    public int height = 50;
    public int minRoomSize = 5;
    public int maxLeafSize = 20; // �ִ� ���� ũ��
    public GameManager gameManager; // GameManager ����

    private void Start()
    {
        Leaf root = new Leaf(0, 0, width, height);
        List<Leaf> leaves = new List<Leaf> { root };

        bool didSplit = true;
        // ������ ��� ������ �� �ִ� ���� �ݺ�
        while (didSplit)
        {
            didSplit = false;
            List<Leaf> newLeaves = new List<Leaf>();
            foreach (var leaf in leaves)
            {
                if (leaf.leftChild == null && leaf.rightChild == null) // ������ �� �ִ��� Ȯ��
                {
                    if (leaf.width > maxLeafSize || leaf.height > maxLeafSize || Random.Range(0f, 1f) > 0.25)
                    {
                        if (leaf.Split(minRoomSize)) // ���� �õ�
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

        // �� ������ ���� ����
        foreach (var leaf in leaves)
        {
            leaf.CreateRooms(wallPrefab, floorPrefab);
        }

        // ������ ���� �����Ͽ� ���� ��ġ�� ����
        Leaf startLeaf = leaves[Random.Range(0, leaves.Count)];
        Vector3 startRoomCenter = new Vector3(startLeaf.room.x + startLeaf.room.width / 2, startLeaf.room.y + startLeaf.room.height / 2, 0);
        gameManager.SetPlayerStartPosition(startRoomCenter);
    }
}

public class Leaf
{
    public int x, y, width, height;
    public Leaf leftChild, rightChild;
    public Rect room; // ���� �簢��

    public Leaf(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    // ������ �����ϴ� �Լ�
    public bool Split(int minSize)
    {
        if (leftChild != null || rightChild != null)
            return false; // �̹� ���ҵ�

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

    // ���� ���� ���� ����
    public void CreateRooms(GameObject wallPrefab, GameObject floorPrefab)
    {
        room = new Rect(x + 1, y + 1, width - 2, height - 2); // �� ũ��� �������� �ణ �۰� ����
        // ���� �ٴڰ� ���� �����ϴ� ���� �߰� �ʿ�
    }
}
