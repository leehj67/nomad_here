using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameStart : MonoBehaviour
{
	// 유니티 에디터에서 씬 이름을 직접 입력할 수 있는 변수
	public string inGameSceneName;

	// Start is called before the first frame update
	void Start()
	{
		// 필요한 초기 설정이 있다면 여기에 추가합니다.
	}

	// Update is called once per frame
	void Update()
	{
		// 매 프레임마다 실행되어야 하는 내용이 있다면 여기에 추가합니다.
	}

	// 버튼 클릭 시 호출되는 함수
	public void OnStartButtonClicked()
	{
		if (!string.IsNullOrEmpty(inGameSceneName))
		{
			// 씬 이름을 기반으로 씬을 전환
			SceneManager.LoadScene(inGameSceneName);
		}
		else
		{
			Debug.LogError("InGame 씬 이름이 지정되지 않았습니다.");
		}
	}
}
