using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float speed = 4f;  // �߻�ü�� �ӵ�
	public float lifeTime = 2f; // �߻�ü�� �����ֱ�(���� �ð�)

	void Start()
	{
		// ���� �ð��� ������ �߻�ü�� �ı�
		Destroy(gameObject, lifeTime);
	}

	void Update()
	{
		// �߻�ü�� ������ �̵���Ŵ
		transform.Translate(Vector2.right * speed * Time.deltaTime);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// �浹�� ��ü�� ������ �Ǵ� �ı� ������ ��ü���� Ȯ�� ��, ó��
		if (collision.CompareTag("Enemy"))
		{
			// ��: ������ �������� �ְ� �߻�ü�� �ı�
			Destroy(collision.gameObject); // ���÷� �� ������Ʈ�� �ı���
			Destroy(gameObject); // �߻�ü�� �ı���
		}
		else if (collision.CompareTag("Obstacle"))
		{
			// ��ֹ��� �ε����� ��, �߻�ü �ı�
			Destroy(gameObject);
		}
	}
}