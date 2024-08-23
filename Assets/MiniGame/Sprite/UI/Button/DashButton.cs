using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashButton : MonoBehaviour
{
	public bool isDashing = false;

	private Button dashButton;

	void Start()
	{
		dashButton = GetComponent<Button>();
		dashButton.onClick.AddListener(OnDashButtonClicked);
	}

	void Update()
	{
		// J Ű�� ������ ���� ��ư�� Ŭ���� �Ͱ� ������ ������ ����
		if (Input.GetKeyDown(KeyCode.J))
		{
			OnDashButtonClicked();
		}
	}

	void OnDashButtonClicked()
	{
		if (!dashButton.interactable)
			return; // ��ư�� ��Ȱ��ȭ�� ��� �������� �ʵ��� ����

		isDashing = true;
		Debug.Log("Dash button clicked or J key pressed! isDashing set to true.");

		dashButton.interactable = false;
		StartCoroutine(ResetDash());
	}

	IEnumerator ResetDash()
	{
		yield return new WaitForSeconds(1f);
		isDashing = false;
		Debug.Log("isDashing set to false.");
		dashButton.interactable = true;
	}
}
