using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space_01 : MonoBehaviour
{
	private Coroutine destroyCoroutine;

	// Ʈ���� �浹�� ���۵� �� ȣ��Ǵ� �޼���
	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Player �±׸� ���� ������Ʈ�� �浹�ߴ��� Ȯ��
		if (collision.CompareTag("Player"))
		{
			// �ڷ�ƾ ���� (2�� �� ������Ʈ ����)
			destroyCoroutine = StartCoroutine(DestroyAfterDelay(1f));
		}
	}

	// Ʈ���� �浹�� ���� �� ȣ��Ǵ� �޼���
	private void OnTriggerExit2D(Collider2D collision)
	{
		// Player �±׸� ���� ������Ʈ�� ������ �� �ڷ�ƾ ����
		if (collision.CompareTag("Player"))
		{
			if (destroyCoroutine != null)
			{
				StopCoroutine(destroyCoroutine);
			}
		}
	}

	// ������ �ð� �� ������Ʈ�� �����ϴ� �ڷ�ƾ
	private IEnumerator DestroyAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
	}
}
