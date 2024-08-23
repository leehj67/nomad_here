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

}
