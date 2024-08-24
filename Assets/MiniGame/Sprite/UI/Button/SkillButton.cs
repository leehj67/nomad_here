using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
	private Button skillButton;

	void Start()
	{
		// Button 컴포넌트를 가져옵니다.
		skillButton = GetComponent<Button>();

		// 만약 Button 컴포넌트를 찾지 못했다면 오류 메시지를 출력합니다.
		if (skillButton == null)
		{
			Debug.LogError("Button component not found on this GameObject.");
		}

		// 버튼 클릭 시 DisableButtonForAWhile 메서드를 실행하도록 설정합니다.
		skillButton.onClick.AddListener(() => StartCoroutine(DisableButtonForAWhile()));
	}

	void Update()
	{
		// 스킬 버튼이 활성화된 상태에서만 I키 입력을 처리합니다.
		if (skillButton != null && skillButton.interactable)
		{
			if (Input.GetKeyDown(KeyCode.I))
			{
				skillButton.onClick.Invoke();
			}
		}
	}

	// 버튼을 2초 동안 비활성화하는 Coroutine
	private IEnumerator DisableButtonForAWhile()
	{
		// 버튼을 비활성화합니다.
		skillButton.interactable = false;

		// 2초 동안 대기합니다.
		yield return new WaitForSeconds(2f);

		// 버튼을 다시 활성화합니다.
		skillButton.interactable = true;
	}
}
