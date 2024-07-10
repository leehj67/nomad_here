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

    [PunRPC]
    public void SelectRandomEvent()
    {
        Debug.Log("SelectRandomEvent called.");
        if (Random.value <= eventTriggerProbability)
        {
            Debug.Log("Attempting to trigger an event.");
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
                    StartEventCutscene(currentEvent);
                    return;
                }
                else
                {
                    randomPoint -= gameEvent.triggerProbability;
                }
            }

            Debug.Log("No event triggered.");
            currentEvent = null;
            EndEventCutscene(false);
        }
        else
        {
            Debug.Log("Event not triggered by probability check.");
            currentEvent = null;
            EndEventCutscene(false);
        }
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
        else
        {
            Debug.Log("No event cutscene UI found.");
            EndEventCutscene(true);
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
            ApplyEventEffects(currentEvent);
        }

        GameStateManager.Instance.ShowDayPanel();
    }

    private void ApplyEventEffects(GameEvent gameEvent)
    {
        GameStateManager.Instance.UpdateShipFood(gameEvent.foodChange);
        GameStateManager.Instance.UpdateShipParts(gameEvent.partsChange);
        GameStateManager.Instance.UpdateShipEnergy(gameEvent.energyChange);

        for (int i = 0; i < GameStateManager.Instance.PlayerStates.Length; i++)
        {
            GameStateManager.Instance.UpdatePlayerHealth(i, gameEvent.healthChange);
            GameStateManager.Instance.UpdatePlayerStamina(i, gameEvent.staminaChange);
            GameStateManager.Instance.UpdatePlayerHunger(i, gameEvent.hungerChange);
        }
    }

    public List<GameEvent> GetIncompleteEvents()
    {
        return events;
    }
}
