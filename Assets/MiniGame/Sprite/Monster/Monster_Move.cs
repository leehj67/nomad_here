using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Move : MonoBehaviour
{
	public Transform player; // �÷��̾� ������Ʈ�� �����ϴ� ����
	public GameObject projectilePrefab; // �߻�ü ������
	public Transform firePoint; // �߻�ü�� �߻�� ��ġ
	public float attackRange = 10f; // ���Ÿ� ���� ����, ����Ƽ���� ���� ����
	public float fireRate = 1f; // �߻� ���� (��), ����Ƽ���� ���� ����
	public MonsterScore monsterScoreManager; // MonsterScore ��ũ��Ʈ�� ���� (�巡�� �� ���)

	private float nextFireTime = 0f; // ���� �߻� �ð�

	void Update()
	{
		// �÷��̾���� �Ÿ� ���
		float distanceToPlayer = Vector2.Distance(transform.position, player.position);

		// �÷��̾ ���� ���� ���� �ִ��� Ȯ��
		if (distanceToPlayer <= attackRange)
		{
			// ���� �ð����� �߻�ü �߻�
			if (Time.time >= nextFireTime)
			{
				FireProjectile();
				nextFireTime = Time.time + fireRate; // ���� �߻� �ð� ����
			}
		}
	}

	void FireProjectile()
	{
		// �÷��̾��� ���� ���
		Vector2 direction = (player.position - firePoint.position).normalized;

		// �߻�ü ���� �� ȸ�� ����
		GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // ������ ������ ��ȯ
		projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

		// �߻�ü�� �ӵ� �ο� (�÷��̾ ���� �߻�)
		projectile.GetComponent<Rigidbody2D>().velocity = direction * 10f; // 10f�� �߻�ü �ӵ�, �ʿ�� ���� ����
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		// "Player_Skill" �±׸� ���� ������Ʈ�� �浹�ϸ� ���� ������Ʈ ����
		if (collision.CompareTag("Player_Skill"))
		{
			if (monsterScoreManager != null)
			{
				monsterScoreManager.AddScore(20); // ���� ���� 20�� �߰�
			}
			Destroy(gameObject); // ���� ������Ʈ ����
		}
	}
}
