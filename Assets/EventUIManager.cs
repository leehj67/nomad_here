using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventUIManager : MonoBehaviour
{
    public static EventUIManager Instance;
    public RectTransform eventPanel; // RectTransform으로 변경
    public TMP_Text currentEventText; // 현재 이벤트 텍스트
    public Button toggleButton;
    public float slideDuration = 0.5f; // 슬라이드 지속 시간

    private bool isPanelVisible = false;
    private Vector2 hiddenPosition;
    private Vector2 visiblePosition;

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

    private void Start()
    {
        // ToggleButton 클릭 이벤트에 TogglePanel 메서드 연결
        toggleButton.onClick.AddListener(TogglePanel);

        // 패널의 초기 위치 설정
        hiddenPosition = new Vector2(-eventPanel.rect.width, eventPanel.anchoredPosition.y); // 화면 밖 (왼쪽)
        visiblePosition = new Vector2(0, eventPanel.anchoredPosition.y); // 화면 안 (왼쪽)

        // 패널을 숨김 상태로 시작
        eventPanel.anchoredPosition = hiddenPosition;

        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        // 현재 이벤트 표시
        if (EventManager.Instance.currentEvent != null)
        {
            currentEventText.text = $"Current Event: {EventManager.Instance.currentEvent.eventName}\n{EventManager.Instance.currentEvent.description}";
        }
        else
        {
            currentEventText.text = "No current event";
        }
    }

    private void TogglePanel()
    {
        isPanelVisible = !isPanelVisible;
        StopAllCoroutines();
        StartCoroutine(SlidePanel(isPanelVisible ? visiblePosition : hiddenPosition));
    }

    private System.Collections.IEnumerator SlidePanel(Vector2 targetPosition)
    {
        Vector2 startPosition = eventPanel.anchoredPosition;
        float elapsedTime = 0;

        while (elapsedTime < slideDuration)
        {
            eventPanel.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        eventPanel.anchoredPosition = targetPosition;
    }
}
