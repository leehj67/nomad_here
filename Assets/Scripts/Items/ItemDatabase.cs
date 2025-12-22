using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [Header("모든 ItemData를 여기에 등록")]
    public List<ItemData> allItems = new();

    private readonly Dictionary<string, ItemData> map = new();

    void Awake()
    {
        // ✅ 씬 넘어가도 1개만 유지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildMap();
    }

    void BuildMap()
    {
        map.Clear();
        foreach (var item in allItems)
        {
            if (item == null) continue;
            if (string.IsNullOrWhiteSpace(item.itemId)) continue;

            map[item.itemId] = item;
        }

        Debug.Log($"[ItemDatabase] Loaded: {map.Count} items");
    }

    public static ItemData Get(string id)
    {
        if (Instance == null || string.IsNullOrWhiteSpace(id)) return null;
        return Instance.map.TryGetValue(id, out var data) ? data : null;
    }
}
