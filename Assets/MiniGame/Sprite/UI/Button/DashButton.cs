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
		// J 키를 눌렀을 때도 버튼이 클릭된 것과 동일한 동작을 실행
		if (Input.GetKeyDown(KeyCode.J))
		{
			OnDashButtonClicked();
		}
	}

	void OnDashButtonClicked()
	{
		if (!dashButton.interactable)
			return; // 버튼이 비활성화된 경우 동작하지 않도록 설정

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
