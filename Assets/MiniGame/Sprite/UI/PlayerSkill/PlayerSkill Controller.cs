using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float speed = 4f;  // 발사체의 속도
	public float lifeTime = 2f; // 발사체의 생명주기(존재 시간)

	void Start()
	{
		// 일정 시간이 지나면 발사체를 파괴
		Destroy(gameObject, lifeTime);
	}

	void Update()
	{
		// 발사체를 앞으로 이동시킴
		transform.Translate(Vector2.right * speed * Time.deltaTime);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// 충돌한 객체가 적인지 또는 파괴 가능한 물체인지 확인 후, 처리
		if (collision.CompareTag("Enemy"))
		{
			// 예: 적에게 데미지를 주고 발사체를 파괴
			Destroy(collision.gameObject); // 예시로 적 오브젝트를 파괴함
			Destroy(gameObject); // 발사체도 파괴됨
		}
		else if (collision.CompareTag("Obstacle"))
		{
			// 장애물에 부딪혔을 때, 발사체 파괴
			Destroy(gameObject);
		}
	}
}