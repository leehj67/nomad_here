using UnityEngine;
using UnityEngine.UI;

public class TestButtonManager : MonoBehaviour
{
    public Button nextDayButton;

    private void Start()
    {
        // 버튼 클릭 이벤트에 메서드 연결
        nextDayButton.onClick.AddListener(GameStateManager.Instance.AdvanceDay);
    }
}
