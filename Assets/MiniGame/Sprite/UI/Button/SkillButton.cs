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
		// Button ������Ʈ�� �����ͼ� Ŭ�� �̺�Ʈ�� �޼��带 ����
		skillButton = GetComponent<Button>();
		skillButton.onClick.AddListener(OnSkillButtonClicked);
	}

	void Update()
	{
		// Ű���� �Է��� �����Ͽ� 'I' Ű�� ������ �� ��ų ��ư Ŭ�� ȿ���� ����
		if (Input.GetKeyDown(KeyCode.I))
		{
			OnSkillButtonClicked();
		}
	}

	void OnSkillButtonClicked()
	{
		if (!skillButton.interactable)
			return; // ��ư�� ��Ȱ��ȭ�� ��� �������� �ʵ��� ����

		isSkillActive = true; // Ŭ�� �� isSkillActive ������ true�� ����
		Debug.Log("Skill button clicked or I key pressed! isSkillActive set to true.");

		// ��ư�� ��Ȱ��ȭ�ϰ� 0.5�� �� �ٽ� Ȱ��ȭ
		skillButton.interactable = false;
		StartCoroutine(ResetSkill());
	}

	IEnumerator ResetSkill()
	{
		yield return new WaitForSeconds(0.5f); // 0.5�� ���
		isSkillActive = false; // isSkillActive ������ false�� ����
		Debug.Log("isSkillActive set to false.");

		// ��ư�� �ٽ� Ȱ��ȭ
		skillButton.interactable = true;
	}
}
