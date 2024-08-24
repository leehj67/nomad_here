using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor; // UnityEditor 네임스페이스를 사용하여 SceneAsset을 참조합니다.

public class GameStop : MonoBehaviour
{
	// 유니티 에디터에서 씬을 드래그 앤 드롭으로 할당할 수 있는 SceneAsset 변수
	public SceneAsset lobbyScene;

	// Update는 매 프레임마다 호출됩니다.
	void Update()
	{
		// Esc 키를 눌렀을 때 로비 씬으로 전환
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ReturnToLobby();
		}
	}

	// 버튼 클릭 시 호출될 메서드
	public void OnGameStopButtonClicked()
	{
		ReturnToLobby();
	}

	// 로비 씬으로 돌아가는 메서드
	void ReturnToLobby()
	{
		if (lobbyScene != null)
		{
			string scenePath = AssetDatabase.GetAssetPath(lobbyScene);
			string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
			SceneManager.LoadScene(sceneName);
		}
		else
		{
			Debug.LogError("Lobby scene is not assigned!");
		}
	}
}
