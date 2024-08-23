using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireSkillAi : MonoBehaviour
{
	public float lifeTime = 5f; // 발사체의 생명 시간
	public float speed = 5f; // 발사체의 속도

	private Rigidbody2D rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();

		// 발사체의 속도 설정
		if (rb != null)
		{
			// 발사체의 진행 방향 설정
			rb.velocity = transform.right * speed;
		}

		// lifeTime 후에 발사체 제거
		Destroy(gameObject, lifeTime);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// 발사체가 자신을 발사한 몬스터와 충돌하지 않도록 함
		if (!collision.CompareTag("Monster"))
		{
			Destroy(gameObject);
		}
	}
}
