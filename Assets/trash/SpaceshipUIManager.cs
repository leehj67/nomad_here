using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceshipUIManager : MonoBehaviour
{
    public RectTransform foodBar;
    public RectTransform partsBar;
    public RectTransform energyBar;

    public TMP_Text foodStatusText;
    public TMP_Text partsStatusText;
    public TMP_Text energyStatusText;

    public TMP_Text foodValueText; // Food value 텍스트 추가
    public TMP_Text partsValueText; // Parts value 텍스트 추가
    public TMP_Text energyValueText; // Energy value 텍스트 추가

    private float maxBarWidth;

    private void Start()
    {
        // GameStateManager의 인스턴스를 설정
        GameStateManager.Instance.SetSpaceshipUIManager(this);

        // 바의 최대 길이를 초기화
        maxBarWidth = foodBar.sizeDelta.x;
        UpdateUI();
    }

    public void UpdateUI()
    {
        // 우주선 상태 업데이트
        UpdateBar(foodBar, GameStateManager.Instance.ShipFood, foodValueText);
        UpdateBar(partsBar, GameStateManager.Instance.ShipParts, partsValueText);
        UpdateBar(energyBar, GameStateManager.Instance.ShipEnergy, energyValueText);

        // 상태 텍스트 업데이트
        UpdateStatusText(foodStatusText, GameStateManager.Instance.ShipFood);
        UpdateStatusText(partsStatusText, GameStateManager.Instance.ShipParts);
        UpdateStatusText(energyStatusText, GameStateManager.Instance.ShipEnergy);
    }

    private void UpdateBar(RectTransform bar, int value, TMP_Text valueText)
    {
        float newWidth;
        if (value > 100)
        {
            newWidth = maxBarWidth; // 100 이상일 경우 최대 길이를 유지
        }
        else
        {
            newWidth = maxBarWidth * (value / 100f); // 100 이하일 경우 길이를 조정
        }
        bar.sizeDelta = new Vector2(newWidth, bar.sizeDelta.y);
        
        // 현재 값 텍스트 업데이트
        valueText.text = value.ToString();
    }

    private void UpdateStatusText(TMP_Text statusText, int value)
    {
        if (value >= 75)
        {
            statusText.text = "Good";
        }
        else if (value >= 50)
        {
            statusText.text = "Normal";
        }
        else if (value >= 25)
        {
            statusText.text = "Danger";
        }
        else
        {
            statusText.text = "Critical";
        }
    }
}
