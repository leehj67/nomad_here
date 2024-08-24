using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashButton : MonoBehaviour
{
	private Button dashButton;

	void Start()
	{
		// Button 컴포넌트를 가져옵니다.
		dashButton = GetComponent<Button>();

		// 만약 Button 컴포넌트를 찾지 못했다면 오류 메시지를 출력합니다.
		if (dashButton == null)
		{
			Debug.LogError("Button component not found on this GameObject.");
		}

		// 버튼 클릭 시 DisableButtonForASecond 메서드를 실행하도록 설정합니다.
		dashButton.onClick.AddListener(() => StartCoroutine(DisableButtonForASecond()));
	}

	void Update()
	{
		// dashButton이 활성화된 상태에서만 K키 입력을 처리합니다.
		if (dashButton != null && dashButton.interactable)
		{
			if (Input.GetKeyDown(KeyCode.K))
			{
				dashButton.onClick.Invoke();
			}
		}
	}

	// 버튼을 1초 동안 비활성화하는 Coroutine
	private IEnumerator DisableButtonForASecond()
	{
		// 버튼을 비활성화합니다.
		dashButton.interactable = false;

		// 1초 동안 대기합니다.
		yield return new WaitForSeconds(1f);

		// 버튼을 다시 활성화합니다.
		dashButton.interactable = true;
	}
}
