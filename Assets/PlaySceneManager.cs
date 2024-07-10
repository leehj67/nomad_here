using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviourPunCallbacks
{
    public float playDuration = 60f; // 게임 플레이 시간
    private float timer; // 타이머
    private bool isReturningToGameScene = false; // 게임 씬으로 돌아가는 중인지 확인

    private void Start()
    {
        timer = playDuration; // 타이머 초기화
    }

    private void Update()
    {
        timer -= Time.deltaTime; // 시간 감소
        if (timer <= 0 && !isReturningToGameScene)
        {
            isReturningToGameScene = true;
            CheckResourcesAndReturn(); // 자원 체크 후 적절한 조치
        }
    }

    // 자원 상태를 확인하고 적절한 씬으로 이동
    private void CheckResourcesAndReturn()
    {
        GameStateManager gameStateManager = GameStateManager.Instance;
        if (gameStateManager != null)
        {
            if (gameStateManager.ShipFood <= 0 || gameStateManager.ShipParts <= 0 || gameStateManager.ShipEnergy <= 0)
            {
                Debug.Log("Game Over: One or more resources depleted. Transitioning to EndScene.");
                PhotonNetwork.LoadLevel("EndScene"); // 자원이 하나라도 0 이하이면 EndScene 로드
            }
            else
            {
                ReturnToGameScene(); // 자원이 충분하면 GameScene 로드
            }
        }
        else
        {
            Debug.LogError("GameStateManager instance not found. Cannot check resources.");
            ReturnToGameScene(); // GameStateManager 인스턴스를 찾을 수 없으면 GameScene 로드
        }
    }

    private void ReturnToGameScene()
    {
        Debug.Log("Returning to GameScene...");
        SceneManager.sceneLoaded += OnGameSceneLoaded; // 씬 로드 이벤트에 메서드 등록
        PhotonNetwork.LoadLevel("GameScene");
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            Debug.Log("GameScene loaded, advancing day...");
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.AdvanceDay();
                GameStateManager.Instance.ShowDayPanel();
                GameStateManager.Instance.ShowTimerPanel();
            }
            SceneManager.sceneLoaded -= OnGameSceneLoaded; // 이벤트 리스너 해제
        }
    }
}