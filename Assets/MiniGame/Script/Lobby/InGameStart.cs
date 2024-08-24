using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // ��ư Ŭ�� �̺�Ʈ�� ���� �߰�

public class InGameStart : MonoBehaviour
{
	// ���� ���� �� �̸��� ����Ƽ �����Ϳ��� ������ �� �ֵ��� public ������ ����
	public string mainGameSceneName;

	// ��ư ������Ʈ�� ĳ���ϱ� ���� ����
	private Button button;

	// Start �Լ����� ��ư Ŭ�� �̺�Ʈ�� ����
	void Start()
	{
		// ��ư ������Ʈ�� ��������
		button = GetComponent<Button>();

		// ��ư�� ����� �Ҵ���� �ʾҴٸ� ���� �޽����� ���
		if (button == null)
		{
			Debug.LogError("Button component is missing on this GameObject.");
			return;
		}

		// ��ư Ŭ�� �� OnButtonClick �Լ��� ȣ��ǵ��� �̺�Ʈ ������ �߰�
		button.onClick.AddListener(OnButtonClick);
	}

	// Update �Լ����� ����Ű �Է��� üũ
	void Update()
	{
		// ����Ű(Ű�ڵ�: KeyCode.Return �Ǵ� KeyCode.KeypadEnter)�� ������ ��
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			// ��ư Ŭ�� �̺�Ʈ Ʈ����
			button.onClick.Invoke();
		}
	}

	// ��ư Ŭ�� �� ȣ��� �Լ�
	void OnButtonClick()
	{
		// ���� ���� ������ ��ȯ
		SceneManager.LoadScene(mainGameSceneName);
	}
}
