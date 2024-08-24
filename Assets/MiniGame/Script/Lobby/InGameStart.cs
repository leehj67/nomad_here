using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // ��ư Ŭ�� �̺�Ʈ�� ���� �߰�

public class InGameStart : MonoBehaviour
{
	// ���� ���� �� �̸��� ����Ƽ �����Ϳ��� ������ �� �ֵ��� public ������ ����
	public string mainGameSceneName;

	// Start �Լ����� ��ư Ŭ�� �̺�Ʈ�� ����
	void Start()
	{
		// ��ư ������Ʈ�� ��������
		Button button = GetComponent<Button>();

		// ��ư�� ����� �Ҵ���� �ʾҴٸ� ���� �޽����� ���
		if (button == null)
		{
			Debug.LogError("Button component is missing on this GameObject.");
			return;
		}

		// ��ư Ŭ�� �� OnButtonClick �Լ��� ȣ��ǵ��� �̺�Ʈ ������ �߰�
		button.onClick.AddListener(OnButtonClick);
	}

	// ��ư Ŭ�� �� ȣ��� �Լ�
	void OnButtonClick()
	{
		// ���� ���� ������ ��ȯ
		SceneManager.LoadScene(mainGameSceneName);
	}
}
