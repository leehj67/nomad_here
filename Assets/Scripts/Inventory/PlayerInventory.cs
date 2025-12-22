using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int capacity = 4;
    public List<ItemData> items = new();

    public bool Add(ItemData data)
    {
        if (data == null) return false;
        if (items.Count >= capacity) return false;
        items.Add(data);
        return true;
    }

    public ItemData Get(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return null;
        return items[slotIndex];
    }

    public ItemData RemoveAt(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return null;
        var item = items[slotIndex];
        items.RemoveAt(slotIndex);
        return item;
    }
    public float TotalWeight()
    {
        float sum = 0f;
        foreach (var it in items)
            if (it != null) sum += Mathf.Max(0f, it.weight);
        return sum;
    }

    public void Clear() => items.Clear();
}
