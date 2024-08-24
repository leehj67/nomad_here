using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashButton : MonoBehaviour
{
	private Button dashButton;

	void Start()
	{
		// Button ������Ʈ�� �����ɴϴ�.
		dashButton = GetComponent<Button>();

		// ���� Button ������Ʈ�� ã�� ���ߴٸ� ���� �޽����� ����մϴ�.
		if (dashButton == null)
		{
			Debug.LogError("Button component not found on this GameObject.");
		}

		// ��ư Ŭ�� �� DisableButtonForASecond �޼��带 �����ϵ��� �����մϴ�.
		dashButton.onClick.AddListener(() => StartCoroutine(DisableButtonForASecond()));
	}

	void Update()
	{
		// dashButton�� Ȱ��ȭ�� ���¿����� KŰ �Է��� ó���մϴ�.
		if (dashButton != null && dashButton.interactable)
		{
			if (Input.GetKeyDown(KeyCode.K))
			{
				dashButton.onClick.Invoke();
			}
		}
	}

	// ��ư�� 1�� ���� ��Ȱ��ȭ�ϴ� Coroutine
	private IEnumerator DisableButtonForASecond()
	{
		// ��ư�� ��Ȱ��ȭ�մϴ�.
		dashButton.interactable = false;

		// 1�� ���� ����մϴ�.
		yield return new WaitForSeconds(1f);

		// ��ư�� �ٽ� Ȱ��ȭ�մϴ�.
		dashButton.interactable = true;
	}
}
