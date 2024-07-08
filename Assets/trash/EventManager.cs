using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    public List<GameEvent> events = new List<GameEvent>();
    public GameEvent currentEvent;

    // 이벤트 발생 확률 (0 - 1 사이의 값)
    [Range(0f, 1f)]
    public float eventTriggerProbability = 1.0f; // 이벤트 발생 확률을 1로 설정

    public float eventDisplayDuration = 3f; // 이벤트 컷씬을 보여주는 시간
    public float blinkDuration = 0.5f; // 깜빡이는 주기

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

    public void SelectRandomEvent()
    {
        Debug.Log("SelectRandomEvent called.");
        if (Random.value <= eventTriggerProbability)
        {
            Debug.Log("Attempting to trigger an event.");
            // 이벤트가 발생할 확률에 해당하는 경우
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
            EndEventCutscene(false); // 이벤트가 발생하지 않을 때에도 EndEventCutscene 호출
        }
        else
        {
            Debug.Log("Event not triggered by probability check.");
            // 이벤트가 발생하지 않을 확률에 해당하는 경우
            currentEvent = null;
            EndEventCutscene(false); // 이벤트가 발생하지 않을 때에도 EndEventCutscene 호출
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

            // 시작 깜빡임 코루틴
            StartCoroutine(BlinkEventCutscene());
        }
        else
        {
            Debug.Log("No event cutscene UI found.");
            EndEventCutscene(true); // 이벤트 컷씬이 없을 때에도 EndEventCutscene 호출
        }
    }

    private IEnumerator BlinkEventCutscene()
    {
        float timer = 0;
        while (timer < eventDisplayDuration)
        {
            yield return StartCoroutine(FadeInAndOut(canvasGroup, blinkDuration));
            timer += blinkDuration * 2; // FadeIn + FadeOut 시간
        }

        EndEventCutscene(true);
    }

    private IEnumerator FadeInAndOut(CanvasGroup canvasGroup, float duration)
    {
        // Fade In
        float halfDuration = duration / 2;
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / halfDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Fade Out
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

        // 이벤트가 발생한 경우 스탯 변경 적용
        if (eventOccurred && currentEvent != null)
        {
            ApplyEventEffects(currentEvent);
        }
        
        // 날짜 패널을 표시
        GameStateManager.Instance.ShowDayPanel();
    }

    private void ApplyEventEffects(GameEvent gameEvent)
    {
        // 우주선 스탯 변경
        GameStateManager.Instance.UpdateShipFood(gameEvent.foodChange);
        GameStateManager.Instance.UpdateShipParts(gameEvent.partsChange);
        GameStateManager.Instance.UpdateShipEnergy(gameEvent.energyChange);

        // 플레이어 스탯 변경 (모든 플레이어에 적용)
        for (int i = 0; i < GameStateManager.Instance.PlayerStates.Length; i++)
        {
            GameStateManager.Instance.UpdatePlayerHealth(i, gameEvent.healthChange);
            GameStateManager.Instance.UpdatePlayerStamina(i, gameEvent.staminaChange);
            GameStateManager.Instance.UpdatePlayerHunger(i, gameEvent.hungerChange);
        }
    }

    public List<GameEvent> GetIncompleteEvents()
    {
        // 실제 구현에서는 이벤트의 완료 여부를 관리할 필요가 있음
        return events; // 이 예제에서는 모든 이벤트를 반환
    }
}
