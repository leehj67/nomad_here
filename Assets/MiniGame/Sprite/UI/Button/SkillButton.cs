using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
	public bool isSkillActive = false;

	private Button skillButton;

	void Start()
	{
		// Button 컴포넌트를 가져와서 클릭 이벤트에 메서드를 연결
		skillButton = GetComponent<Button>();
		skillButton.onClick.AddListener(OnSkillButtonClicked);
	}

	void Update()
	{
		// 키보드 입력을 감지하여 'I' 키가 눌렸을 때 스킬 버튼 클릭 효과를 실행
		if (Input.GetKeyDown(KeyCode.I))
		{
			OnSkillButtonClicked();
		}
	}

	void OnSkillButtonClicked()
	{
		if (!skillButton.interactable)
			return; // 버튼이 비활성화된 경우 동작하지 않도록 설정

		isSkillActive = true; // 클릭 시 isSkillActive 변수를 true로 변경
		Debug.Log("Skill button clicked or I key pressed! isSkillActive set to true.");

		// 버튼을 비활성화하고 0.5초 후 다시 활성화
		skillButton.interactable = false;
		StartCoroutine(ResetSkill());
	}

	IEnumerator ResetSkill()
	{
		yield return new WaitForSeconds(0.5f); // 0.5초 대기
		isSkillActive = false; // isSkillActive 변수를 false로 변경
		Debug.Log("isSkillActive set to false.");

		// 버튼을 다시 활성화
		skillButton.interactable = true;
	}
}
