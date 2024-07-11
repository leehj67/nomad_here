using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

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
    public void SelectRandomEvent()
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
            foreach (var gameEvent in events)
            {
                if (randomPoint < gameEvent.triggerProbability)
                {
                    currentEvent = gameEvent;
                    photonView.RPC("StartEventCutsceneRPC", RpcTarget.All, currentEvent.eventName);
                    photonView.RPC("ApplyEventEffects", RpcTarget.All, currentEvent.eventName);
                    return;
                }
                randomPoint -= gameEvent.triggerProbability;
            }
        }
        Debug.Log("No event triggered.");
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

        EndEventCutscene(true);
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

    private void EndEventCutscene(bool eventOccurred)
    {
        if (currentEventCutscene != null)
        {
            Destroy(currentEventCutscene);
            currentEventCutscene = null;
        }

        if (eventOccurred && currentEvent != null)
        {
            Debug.Log("Event ended successfully.");
        }
    }

    [PunRPC]
    public void ApplyEventEffects(string eventName)
    {
        GameEvent gameEvent = GetEventById(eventName);
        if (gameEvent != null)
        {
            // 이벤트 효과를 적용하고 GameStateManager에 알림
            GameStateManager.Instance.ApplyEventEffects(gameEvent);
        }
    }

    public GameEvent GetEventById(string eventId)
    {
        foreach (GameEvent gameEvent in events)
        {
            if (gameEvent.eventName == eventId)
            {
                return gameEvent;
            }
        }
        return null;
    }
}
