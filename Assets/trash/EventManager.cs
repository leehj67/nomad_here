using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class EventManager : MonoBehaviourPunCallbacks
{
    public static EventManager Instance;

    public List<GameEvent> events = new List<GameEvent>();
    public GameEvent currentEvent;

    [Range(0f, 1f)]
    public float eventTriggerProbability = 1.0f;

    public float eventDisplayDuration = 3f;
    public float blinkDuration = 0.5f;

    private GameObject currentEventCutscene;
    private CanvasGroup canvasGroup;

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

    public void TryTriggerRandomEvent()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SelectRandomEvent", RpcTarget.All);
        }
    }

    [PunRPC]
    public string SelectRandomEvent()
    {
        Debug.Log("SelectRandomEvent called.");
        if (Random.value <= eventTriggerProbability)
        {
            float totalProbability = 0;
            foreach (var gameEvent in events)
            {
                totalProbability += gameEvent.triggerProbability;
            }

            float randomPoint = Random.value * totalProbability;
            Debug.Log($"Total Probability: {totalProbability}, Random Point: {randomPoint}");

            foreach (var gameEvent in events)
            {
                if (randomPoint < gameEvent.triggerProbability)
                {
                    currentEvent = gameEvent;
                    return currentEvent.eventName; // 이벤트 이름 반환
                }
                else
                {
                    randomPoint -= gameEvent.triggerProbability;
                }
            }
        }
        Debug.Log("No event triggered.");
        return null; // 이벤트가 트리거되지 않은 경우 null 반환
    }

    [PunRPC]
    public void StartEventCutsceneRPC(string eventName)
    {
        GameEvent gameEvent = events.Find(e => e.eventName == eventName);
        if (gameEvent == null) return;

        currentEvent = gameEvent;
        StartEventCutscene(currentEvent);
    }

    private void StartEventCutscene(GameEvent gameEvent)
    {
        if (currentEventCutscene != null)
        {
            Destroy(currentEventCutscene);
        }

        if (gameEvent.eventCutsceneUI != null)
        {
            Debug.Log($"Starting event cutscene for event: {gameEvent.eventName}");
            currentEventCutscene = Instantiate(gameEvent.eventCutsceneUI, transform);
            canvasGroup = currentEventCutscene.AddComponent<CanvasGroup>();
            StartCoroutine(BlinkEventCutscene());
        }
    }

    private IEnumerator BlinkEventCutscene()
    {
        float timer = 0;
        while (timer < eventDisplayDuration)
        {
            yield return StartCoroutine(FadeInAndOut(canvasGroup, blinkDuration));
            timer += blinkDuration * 2;
        }

        // Optionally end the cutscene and handle the aftermath here
    }

    private IEnumerator FadeInAndOut(CanvasGroup canvasGroup, float duration)
    {
        float halfDuration = duration / 2;
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / halfDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;

        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / halfDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
public GameEvent GetEventById(string eventId)
{
    // events 리스트에서 eventId와 일치하는 이벤트를 찾습니다.
    foreach (GameEvent gameEvent in events)
    {
        if (gameEvent.eventName == eventId)
        {
            return gameEvent;  // 일치하는 이벤트를 반환
        }
    }
    return null;  // 일치하는 이벤트가 없으면 null 반환
}

    public void SetEventById(string eventId)
    {
        GameEvent gameEvent = events.Find(e => e.eventName == eventId);
        if (gameEvent != null)
        {
            currentEvent = gameEvent;
            // Apply effects or handle the event specifics here
            Debug.Log("Event set: " + currentEvent.eventName);
        }
        else
        {
            Debug.LogError("Event not found: " + eventId);
        }
    }
}
