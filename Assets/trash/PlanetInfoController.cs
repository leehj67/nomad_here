using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyGameNamespace; // GameManager 네임스페이스 참조

public class PlanetInfoController : MonoBehaviour
{
    public Image[] planetImages; // 행성 이미지 배열
    public TextMeshProUGUI riskText; // 위험도 텍스트
    public TextMeshProUGUI assetsText; // 자원 텍스트

    private int selectedPlanetIndex = 0;
    private Vector3 originalScale;
    private string[] riskLevels = { "Normal", "Hard", "Critical" };

    // 각 행성의 최소 및 최대 가치 설정
    public int[] minAssetValues = { 50, 100, 150, 200, 250, 300 };
    public int[] maxAssetValues = { 150, 200, 250, 300, 350, 400 };

    private int[] actualAssetValues; // 실제 자원 가치 저장
    private bool isInitialized = false; // 초기화 여부 확인

    void Start()
    {
        InitializePlanetInfo();
    }

    public void InitializePlanetInfo()
    {
        if (!isInitialized)
        {
            actualAssetValues = new int[planetImages.Length];
            for (int i = 0; i < planetImages.Length; i++)
            {
                actualAssetValues[i] = Random.Range(minAssetValues[i], maxAssetValues[i]);
                GameManager.Instance.SetPlanetInfo(i, riskLevels[Random.Range(0, riskLevels.Length)], actualAssetValues[i]);
            }
            isInitialized = true;
        }

        selectedPlanetIndex = 0;
        originalScale = planetImages[0].transform.localScale;
        UpdatePlanetInfo();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedPlanetIndex = (selectedPlanetIndex - 1 + planetImages.Length) % planetImages.Length;
            UpdatePlanetInfo();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedPlanetIndex = (selectedPlanetIndex + 1) % planetImages.Length;
            UpdatePlanetInfo();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadPlanetScene();
        }
    }

    void UpdatePlanetInfo()
    {
        for (int i = 0; i < planetImages.Length; i++)
        {
            if (i == selectedPlanetIndex)
            {
                planetImages[i].transform.localScale = originalScale * 1.2f;
                planetImages[i].color = Color.yellow;
            }
            else
            {
                planetImages[i].transform.localScale = originalScale;
                planetImages[i].color = Color.white;
            }
        }

        // 위험도와 자원 정보를 업데이트
        var planetInfo = GameManager.Instance.GetPlanetInfo(selectedPlanetIndex);
        if (planetInfo != null)
        {
            riskText.text = "Risk: " + planetInfo.Risk;
            assetsText.text = "Assets: " + planetInfo.Asset;
        }
    }

    void LoadPlanetScene()
    {
        GameManager.Instance.SelectedPlanetIndex = selectedPlanetIndex; // 선택된 행성 인덱스를 저장
        GameManager.Instance.LoadPlanetScene(selectedPlanetIndex); // 행성 씬으로 전환
    }
}
