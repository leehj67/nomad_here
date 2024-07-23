using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviourPunCallbacks
{
	public float playDuration = 60f; // 게임 플레이 시간
	private float timer; // 타이머
	private bool isReturningToGameScene = false; // 게임 씬으로 돌아가는 중인지 확인

	public GameObject playerPrefab; // 플레이어 프리팹
	public GameObject joystickCanvasPrefab; // 조이스틱 캔버스 프리팹

	private void Start()
	{
		timer = playDuration; // 타이머 초기화

		// 플레이어 오브젝트를 네트워크 상에 생성
		if (PhotonNetwork.IsConnected)
		{
			GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, GetSpawnPosition(), Quaternion.identity);

			// 조이스틱 캔버스를 인스턴스화하고 플레이어에 할당
			if (joystickCanvasPrefab != null)
			{
				GameObject joystickCanvas = Instantiate(joystickCanvasPrefab);
				VariableJoystick joystick = joystickCanvas.GetComponentInChildren<VariableJoystick>();
				if (joystick != null && player != null)
				{
					Player_Move playerMove = player.GetComponent<Player_Move>();
					if (playerMove != null)
					{
						playerMove.joystick = joystick;
					}
				}
				else
				{
					Debug.LogError("Joystick or Player is null");
				}
			}
			else
			{
				Debug.LogError("Joystick Canvas Prefab is not assigned in the Inspector");
			}
		}
		else
		{
			Debug.LogError("PhotonNetwork is not connected");
		}
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

	// 플레이어 스폰 위치를 결정하는 메소드 (예: 랜덤 위치)
	Vector3 GetSpawnPosition()
	{
		// 원하는 스폰 위치 로직을 구현
		return new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (otherPlayer.IsMasterClient)
		{
			// 호스트가 나간 경우, 남은 플레이어들에게 알림
			photonView.RPC("OnHostDisconnected", RpcTarget.Others);
		}
	}

	[PunRPC]
	private void OnHostDisconnected()
	{
		Debug.Log("Host has disconnected. Transitioning to EndScene.");
		PhotonNetwork.LoadLevel("EndScene");
	}
}
