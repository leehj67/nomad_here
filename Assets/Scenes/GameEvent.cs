using UnityEngine;

[CreateAssetMenu(fileName = "NewGameEvent", menuName = "GameEvent")]
public class GameEvent : ScriptableObject
{
    public string eventName;
    public string description;
    public float triggerProbability; // 이벤트 발생 확률 (0-1)
    public GameObject eventCutsceneUI; // 이벤트 컷씬 UI 프리팹

    // 스탯 변경 정보
    public int foodChange;
    public int partsChange;
    public int energyChange;

    // 플레이어 스탯 변경 정보
    public int healthChange;
    public int staminaChange;
    public int hungerChange;
}
