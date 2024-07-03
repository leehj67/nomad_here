using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    public List<GameEvent> events = new List<GameEvent>();
    public GameEvent currentEvent;

    // 이벤트 발생 확률 (0 - 1 사이의 값)
    [Range(0f, 1f)]
    public float eventTriggerProbability = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectRandomEvent()
    {
        if (Random.value <= eventTriggerProbability)
        {
            // 이벤트가 발생할 확률에 해당하는 경우
            float totalProbability = 0;
            foreach (var gameEvent in events)
            {
                totalProbability += gameEvent.triggerProbability;
            }

            float randomPoint = Random.value * totalProbability;

            foreach (var gameEvent in events)
            {
                if (randomPoint < gameEvent.triggerProbability)
                {
                    currentEvent = gameEvent;
                    return;
                }
                else
                {
                    randomPoint -= gameEvent.triggerProbability;
                }
            }

            currentEvent = null;
        }
        else
        {
            // 이벤트가 발생하지 않을 확률에 해당하는 경우
            currentEvent = null;
        }
    }

    public List<GameEvent> GetIncompleteEvents()
    {
        // 실제 구현에서는 이벤트의 완료 여부를 관리할 필요가 있음
        return events; // 이 예제에서는 모든 이벤트를 반환
    }
}
