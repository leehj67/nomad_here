using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireSkillAi : MonoBehaviour
{
	public float lifeTime = 5f; // �߻�ü�� ���� �ð�
	public float speed = 5f; // �߻�ü�� �ӵ�

	private Rigidbody2D rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();

		// �߻�ü�� �ӵ� ����
		if (rb != null)
		{
			// �߻�ü�� ���� ���� ����
			rb.velocity = transform.right * speed;
		}

		// lifeTime �Ŀ� �߻�ü ����
		Destroy(gameObject, lifeTime);
	}

}
