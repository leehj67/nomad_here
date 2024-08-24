using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
	private Button skillButton;

	void Start()
	{
		// Button ������Ʈ�� �����ɴϴ�.
		skillButton = GetComponent<Button>();

		// ���� Button ������Ʈ�� ã�� ���ߴٸ� ���� �޽����� ����մϴ�.
		if (skillButton == null)
		{
			Debug.LogError("Button component not found on this GameObject.");
		}

		// ��ư Ŭ�� �� DisableButtonForAWhile �޼��带 �����ϵ��� �����մϴ�.
		skillButton.onClick.AddListener(() => StartCoroutine(DisableButtonForAWhile()));
	}

	void Update()
	{
		// ��ų ��ư�� Ȱ��ȭ�� ���¿����� IŰ �Է��� ó���մϴ�.
		if (skillButton != null && skillButton.interactable)
		{
			if (Input.GetKeyDown(KeyCode.I))
			{
				skillButton.onClick.Invoke();
			}
		}
	}

	// ��ư�� 2�� ���� ��Ȱ��ȭ�ϴ� Coroutine
	private IEnumerator DisableButtonForAWhile()
	{
		// ��ư�� ��Ȱ��ȭ�մϴ�.
		skillButton.interactable = false;

		// 2�� ���� ����մϴ�.
		yield return new WaitForSeconds(2f);

		// ��ư�� �ٽ� Ȱ��ȭ�մϴ�.
		skillButton.interactable = true;
	}
}
