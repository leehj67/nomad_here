using System.Collections;
using UnityEngine;

public class Wallcheck : MonoBehaviour
{
	public bool Wallimpact = false;  // ������ �浹 ����

	void Update()
	{
		if (Wallimpact) // Wallimpact ���� ���� ��� �޽��� ���
		{
			Debug.Log("���� �浹");
		}
	}

	// ���� ���������� �浹 ���� �� ȣ��˴ϴ�.
	void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Wall"))
		{
			if (!Wallimpact) // Wallimpact ���°� ����� ���� �޽����� ����ϵ��� ���� �߰�
			{
				Debug.Log("���� �浹");
			}
			Wallimpact = true;  // ���� �浹 ���̸� Wallimpact ���� true�� ����
		}
	}

	// ������ �浹�� ������ �� ȣ��˴ϴ�.
	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Wall"))
		{
			Wallimpact = false;  // ������ �浹�� ������ Wallimpact ���� false�� ����
			Debug.Log("������ �浹 ����");  // ������ �浹�� ����Ǿ��� �� �޽��� ���
		}
	}
}
