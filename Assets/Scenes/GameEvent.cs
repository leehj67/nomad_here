using UnityEngine;

[CreateAssetMenu(fileName = "NewGameEvent", menuName = "GameEvent")]
public class GameEvent : ScriptableObject
{
    public string eventName;
    public string description;
    public float triggerProbability; // 이벤트 발생 확률 (0-100%)
}
