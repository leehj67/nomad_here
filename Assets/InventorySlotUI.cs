using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;

    [Header("선택 시 몇 배로 키울지")]
    public float selectedScaleFactor = 1.2f;

    [HideInInspector] public bool hasItem;
    [HideInInspector] public ItemData itemData;

    private Vector3 baseScale;
    private Sprite defaultSprite;

    void Awake()
    {
        baseScale = transform.localScale;

        if (iconImage == null)
            iconImage = GetComponent<Image>();

        if (iconImage != null)
            defaultSprite = iconImage.sprite;
    }

    public void SetItem(ItemData data)
    {
        hasItem = true;
        itemData = data;

        if (iconImage != null)
        {
            if (data != null && data.icon != null)
            {
                iconImage.sprite = data.icon;
                iconImage.enabled = true;
            }
            else
            {
                // 아이콘이 없으면 빈 슬롯 이미지를 유지(원하면 투명 처리 가능)
                iconImage.sprite = defaultSprite;
                iconImage.enabled = true;
            }
        }
    }

    public void Clear()
    {
        hasItem = false;
        itemData = null;

        if (iconImage != null)
        {
            iconImage.sprite = defaultSprite;
            iconImage.enabled = true;
        }
    }

    public void SetSelected(bool selected)
    {
        transform.localScale = selected ? baseScale * selectedScaleFactor : baseScale;
    }
}
