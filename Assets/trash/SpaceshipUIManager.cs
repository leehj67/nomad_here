using UnityEngine;
using TMPro;

public class SpaceshipUIManager : MonoBehaviour
{
    public RectTransform foodBar;
    public RectTransform partsBar;
    public RectTransform energyBar;

    public TMP_Text foodStatusText;
    public TMP_Text partsStatusText;
    public TMP_Text energyStatusText;

    public TMP_Text foodValueText;
    public TMP_Text partsValueText;
    public TMP_Text energyValueText;

    private float foodMaxWidth;
    private float partsMaxWidth;
    private float energyMaxWidth;

    private void Awake()
    {
        // max 폭을 각자 저장(프리팹/레이아웃이 달라도 안전)
        if (foodBar)   foodMaxWidth = foodBar.sizeDelta.x;
        if (partsBar)  partsMaxWidth = partsBar.sizeDelta.x;
        if (energyBar) energyMaxWidth = energyBar.sizeDelta.x;
    }

    private void Start()
    {
        GameStateManager.Instance.SetSpaceshipUIManager(this);
        UpdateUI();
    }

    private void OnEnable()
    {
        // InGameScene으로 “돌아왔을 때”도 갱신되게
        if (GameStateManager.Instance != null)
            UpdateUI();
    }

    public void UpdateUI()
    {
        var gsm = GameStateManager.Instance;
        if (gsm == null) return;

        UpdateBar(foodBar,   gsm.ShipFood,   foodValueText,   foodMaxWidth);
        UpdateBar(partsBar,  gsm.ShipParts,  partsValueText,  partsMaxWidth);
        UpdateBar(energyBar, gsm.ShipEnergy, energyValueText, energyMaxWidth);

        UpdateStatusText(foodStatusText,   gsm.ShipFood);
        UpdateStatusText(partsStatusText,  gsm.ShipParts);
        UpdateStatusText(energyStatusText, gsm.ShipEnergy);
    }

    private void UpdateBar(RectTransform bar, int value, TMP_Text valueText, float maxWidth)
    {
        if (bar == null) return;

        float clamped = Mathf.Clamp(value, 0, 100);
        float newWidth = maxWidth * (clamped / 100f);
        bar.sizeDelta = new Vector2(newWidth, bar.sizeDelta.y);

        if (valueText != null)
            valueText.text = value.ToString();
    }

    private void UpdateStatusText(TMP_Text statusText, int value)
    {
        if (statusText == null) return;

        if (value >= 75) statusText.text = "Good";
        else if (value >= 50) statusText.text = "Normal";
        else if (value >= 25) statusText.text = "Danger";
        else statusText.text = "Critical";
    }
}
