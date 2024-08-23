using System.Collections;
using UnityEngine;

public class PlayerRadar : MonoBehaviour
{
	public Transform player; // �÷��̾� ������Ʈ�� Transform
	public float detectionRange = 5f; // �÷��̾ ������ �Ÿ�
	public GameObject projectilePrefab; // �߻�ü ������
	public Transform firePoint; // �߻�ü �߻� ��ġ
	public float projectileSpeed = 5f; // �߻�ü �ӵ�

	private void Start()
	{
		// 1�ʸ��� �Ÿ� üũ�� ����
		StartCoroutine(CheckPlayerDistance());
	}

	private IEnumerator CheckPlayerDistance()
	{
		while (true)
		{
			// �÷��̾���� �Ÿ� ���
			float distanceToPlayer = Vector2.Distance(transform.position, player.position);

			// ������ �Ÿ� ���� �÷��̾ �����ߴ��� Ȯ��
			if (distanceToPlayer <= detectionRange)
			{
				Debug.Log("�÷��̾� ����");

				// �߻�ü �߻�
				FireProjectile();
			}

			// 1�� ��� �� �ٽ� üũ
			yield return new WaitForSeconds(1f);
		}
	}

	private void FireProjectile()
	{
		if (projectilePrefab != null && firePoint != null)
		{
			// �߻�ü ����
			GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

			// �÷��̾� ���� ���
			Vector2 direction = (player.position - firePoint.position).normalized;

			// �߻�ü�� ���� ���� ����
			projectile.transform.right = direction;

			// �߻�ü�� ���� ���� �߻�
			Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
			if (rb != null)
			{
				rb.velocity = direction * projectileSpeed;
			}
		}
	}
}
