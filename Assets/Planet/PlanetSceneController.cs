using UnityEngine;
using TMPro;
using MyGameNamespace;

public class PlanetSceneController : MonoBehaviour
{
    public TextMeshProUGUI riskText;
    public TextMeshProUGUI assetsText;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[PlanetSceneController] GameManager.Instance가 null입니다. " +
                           "첫 씬에 GameManager 오브젝트가 있고 DontDestroyOnLoad인지 확인하세요.");
            return;
        }

        if (riskText == null || assetsText == null)
        {
            Debug.LogError("[PlanetSceneController] riskText/assetsText가 인스펙터에 연결되지 않았습니다.");
            return;
        }

        int planetIndex = GameManager.Instance.SelectedPlanetIndex;
        var planetInfo = GameManager.Instance.GetPlanetInfo(planetIndex);

        if (planetInfo == null)
        {
            Debug.LogWarning($"[PlanetSceneController] planetInfo가 null입니다. index={planetIndex}");
            return;
        }

        riskText.text   = "Risk: " + planetInfo.Risk;
        assetsText.text = "Assets: " + planetInfo.Asset;
    }
}
