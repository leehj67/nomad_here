using UnityEngine;

[CreateAssetMenu(menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("고유 ID (절대 중복 금지)")]
    public string itemId;        // 예: "BATTERY", "PART", "CHIP", "HEAL"

    [Header("표시용")]
    public string itemName;
    public Sprite icon;

    [Header("분류")]
    public ItemType type;
    [Header("Weight (이동속도 감소에 사용)")]
    public float weight = 0.0f; // 예: 0.2f ~ 2.0f
}
