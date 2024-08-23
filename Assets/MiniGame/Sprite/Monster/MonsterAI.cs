using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMonster : MonoBehaviour
{
	public GameObject projectilePrefab; // �߻�ü ������
	public Transform firePoint; // �߻�ü �߻� ��ġ
	public float fireInterval = 1f; // �߻� ����

	private Transform playerTransform; // �÷��̾� ��ġ
	private bool playerInRange = false; // �÷��̾ ���� ���� �ִ��� ����
	private Coroutine fireCoroutine; // �߻� �ڷ�ƾ

	// �ݶ��̴��� Ʈ���� ���� �� ȣ��
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerTransform = collision.transform;
			playerInRange = true;

			// �߻� ����
			if (fireCoroutine == null)
			{
				fireCoroutine = StartCoroutine(FireProjectile());
			}
		}
	}

	// �ݶ��̴��� Ʈ���ſ��� ��� �� ȣ��
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerInRange = false;

			// �߻� ����
			if (fireCoroutine != null)
			{
				StopCoroutine(fireCoroutine);
				fireCoroutine = null;
			}
		}
	}

	// �߻�ü �߻� ����
	private IEnumerator FireProjectile()
	{
		while (playerInRange)
		{
			// �߻�ü ����
			GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

			// �÷��̾� ���� ���
			Vector2 direction = (playerTransform.position - firePoint.position).normalized;

			// �߻�ü�� ���� ���� ����
			projectile.transform.right = direction;

			// 1�� ���
			yield return new WaitForSeconds(fireInterval);
		}
	}
}
