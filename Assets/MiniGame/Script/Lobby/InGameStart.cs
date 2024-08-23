using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameStart : MonoBehaviour
{
	// ����Ƽ �����Ϳ��� �� �̸��� ���� �Է��� �� �ִ� ����
	public string inGameSceneName;

	// Start is called before the first frame update
	void Start()
	{
		// �ʿ��� �ʱ� ������ �ִٸ� ���⿡ �߰��մϴ�.
	}

	// Update is called once per frame
	void Update()
	{
		// �� �����Ӹ��� ����Ǿ�� �ϴ� ������ �ִٸ� ���⿡ �߰��մϴ�.
	}

	// ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
	public void OnStartButtonClicked()
	{
		if (!string.IsNullOrEmpty(inGameSceneName))
		{
			// �� �̸��� ������� ���� ��ȯ
			SceneManager.LoadScene(inGameSceneName);
		}
		else
		{
			Debug.LogError("InGame �� �̸��� �������� �ʾҽ��ϴ�.");
		}
	}
}
