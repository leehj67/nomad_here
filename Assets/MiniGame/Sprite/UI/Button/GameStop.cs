using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor; // UnityEditor ���ӽ����̽��� ����Ͽ� SceneAsset�� �����մϴ�.

public class GameStop : MonoBehaviour
{
	// ����Ƽ �����Ϳ��� ���� �巡�� �� ������� �Ҵ��� �� �ִ� SceneAsset ����
	public SceneAsset lobbyScene;

	// Update�� �� �����Ӹ��� ȣ��˴ϴ�.
	void Update()
	{
		// Esc Ű�� ������ �� �κ� ������ ��ȯ
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ReturnToLobby();
		}
	}

	// ��ư Ŭ�� �� ȣ��� �޼���
	public void OnGameStopButtonClicked()
	{
		ReturnToLobby();
	}

	// �κ� ������ ���ư��� �޼���
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
