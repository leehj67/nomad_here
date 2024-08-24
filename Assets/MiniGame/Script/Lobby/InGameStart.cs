using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 버튼 클릭 이벤트를 위해 추가

public class InGameStart : MonoBehaviour
{
	// 메인 게임 씬 이름을 유니티 에디터에서 설정할 수 있도록 public 변수로 선언
	public string mainGameSceneName;

	// 버튼 컴포넌트를 캐싱하기 위한 변수
	private Button button;

	// Start 함수에서 버튼 클릭 이벤트를 설정
	void Start()
	{
		// 버튼 컴포넌트를 가져오기
		button = GetComponent<Button>();

		// 버튼이 제대로 할당되지 않았다면 에러 메시지를 출력
		if (button == null)
		{
			Debug.LogError("Button component is missing on this GameObject.");
			return;
		}

		// 버튼 클릭 시 OnButtonClick 함수가 호출되도록 이벤트 리스너 추가
		button.onClick.AddListener(OnButtonClick);
	}

	// Update 함수에서 엔터키 입력을 체크
	void Update()
	{
		// 엔터키(키코드: KeyCode.Return 또는 KeyCode.KeypadEnter)가 눌렸을 때
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			// 버튼 클릭 이벤트 트리거
			button.onClick.Invoke();
		}
	}

	// 버튼 클릭 시 호출될 함수
	void OnButtonClick()
	{
		// 메인 게임 씬으로 전환
		SceneManager.LoadScene(mainGameSceneName);
	}
}
