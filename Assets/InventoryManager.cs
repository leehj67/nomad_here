using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InventoryManager : MonoBehaviour
{
    [Header("인벤토리 슬롯 4개를 순서대로 넣기")]
    public InventorySlotUI[] slots;

    [Header("드랍 위치 기준 (플레이어)")]
    public Transform dropPoint;
    public Vector2 dropOffset = new Vector2(0.5f, 0f);

    private int selectedIndex = 0;

    void Start()
    {
        SelectSlot(0);
    }

    public bool AddItem(ItemData data)
    {
        if (data == null) return false;

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].hasItem)
            {
                slots[i].SetItem(data);
                Debug.Log($"[InventoryUI] {data.itemName} => 슬롯 {i + 1}");
                return true;
            }
        }

        Debug.Log("[InventoryUI] 인벤토리가 가득 참");
        return false;
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;

        selectedIndex = index;
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetSelected(i == selectedIndex);
    }

    // ✅ Player_Move가 요구하는 API (추가)
    public int GetSelectedSlotIndex()
    {
        return selectedIndex;
    }

    // ✅ Player_Move가 요구하는 API (추가)
    //    "PlayerInventory.items"가 정본이고 UI는 따라 그림
    public void RebuildFromInventory(List<ItemData> items)
    {
        ClearAll();

        if (items == null) { SelectSlot(0); return; }

        // 순서를 보장해서 슬롯 0..N에 그대로 매핑
        for (int i = 0; i < slots.Length && i < items.Count; i++)
        {
            if (items[i] != null)
                slots[i].SetItem(items[i]);
        }

        // 기존 선택 유지 (범위 보정)
        SelectSlot(Mathf.Clamp(selectedIndex, 0, slots.Length - 1));
    }

    public ItemData DropSelectedItem_ReturnData()
    {
        InventorySlotUI slot = slots[selectedIndex];
        if (!slot.hasItem || slot.itemData == null) return null;

        ItemData data = slot.itemData; // ✅ 먼저 캡쳐!

        if (dropPoint != null)
        {
            Vector3 pos = dropPoint.position + (Vector3)dropOffset;
            PhotonNetwork.Instantiate(data.itemId, pos, Quaternion.identity);
        }

        slot.Clear();
        return data;
    }

    // ✅ 기존 코드들이 DropSelectedItem()을 호출해도 되게 호환용 래퍼
    public void DropSelectedItem()
    {
        DropSelectedItem_ReturnData();
    }

    public void ClearAll()
    {
        foreach (var s in slots)
            s.Clear();
        SelectSlot(0);
    }

    public void RefreshFromInventory(PlayerInventory inv)
    {
        if (inv == null) return;

        ClearAll();
        for (int i = 0; i < slots.Length && i < inv.items.Count; i++)
        {
            if (inv.items[i] != null)
                slots[i].SetItem(inv.items[i]);
        }
        SelectSlot(selectedIndex);
    }

    public ItemData GetSelectedItemData()
    {
        var slot = slots[selectedIndex];
        return slot != null && slot.hasItem ? slot.itemData : null;
    }
}
