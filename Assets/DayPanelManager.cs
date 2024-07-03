using UnityEngine;
using TMPro;

public class DayPanelManager : MonoBehaviour
{
    public GameObject dayPanel;
    public TMP_Text dayText;
    public float displayDuration = 10f; // 패널이 사라지는 시간 조정 가능

    private void Start()
    {
        ShowDayPanel();
    }

    private void ShowDayPanel()
    {
        dayPanel.SetActive(true);
        dayText.text = $"Day-{GameStateManager.Instance.Day}";

        // 일정 시간 후 패널 비활성화
        Invoke("HideDayPanel", displayDuration);
    }

    private void HideDayPanel()
    {
        dayPanel.SetActive(false);
    }
}
