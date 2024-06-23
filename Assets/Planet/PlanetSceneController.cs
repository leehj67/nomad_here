using UnityEngine;
using TMPro;
using MyGameNamespace; // GameManager 네임스페이스 참조

public class PlanetSceneController : MonoBehaviour
{
    public TextMeshProUGUI riskText;
    public TextMeshProUGUI assetsText;

    void Start()
    {
        int planetIndex = GameManager.Instance.SelectedPlanetIndex; // 선택된 행성 인덱스를 가져옴
        var planetInfo = GameManager.Instance.GetPlanetInfo(planetIndex);
        if (planetInfo != null)
        {
            riskText.text = "Risk: " + planetInfo.Risk;
            assetsText.text = "Assets: " + planetInfo.Asset;
        }
    }
}
