using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Move : MonoBehaviour
{
	public Transform player; // 플레이어 오브젝트를 참조하는 변수
	public GameObject projectilePrefab; // 발사체 프리팹
	public Transform firePoint; // 발사체가 발사될 위치
	public float attackRange = 10f; // 원거리 공격 범위, 유니티에서 조절 가능
	public float fireRate = 1f; // 발사 간격 (초), 유니티에서 조절 가능
	public MonsterScore monsterScoreManager; // MonsterScore 스크립트를 참조 (드래그 앤 드롭)

	private float nextFireTime = 0f; // 다음 발사 시간

	void Update()
	{
		// 플레이어와의 거리 계산
		float distanceToPlayer = Vector2.Distance(transform.position, player.position);

		// 플레이어가 공격 범위 내에 있는지 확인
		if (distanceToPlayer <= attackRange)
		{
			// 일정 시간마다 발사체 발사
			if (Time.time >= nextFireTime)
			{
				FireProjectile();
				nextFireTime = Time.time + fireRate; // 다음 발사 시간 설정
			}
		}
	}

	void FireProjectile()
	{
		// 플레이어의 방향 계산
		Vector2 direction = (player.position - firePoint.position).normalized;

		// 발사체 생성 및 회전 설정
		GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 방향을 각도로 변환
		projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

		// 발사체에 속도 부여 (플레이어를 향해 발사)
		projectile.GetComponent<Rigidbody2D>().velocity = direction * 10f; // 10f는 발사체 속도, 필요시 조절 가능
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		// "Player_Skill" 태그를 가진 오브젝트와 충돌하면 몬스터 오브젝트 삭제
		if (collision.CompareTag("Player_Skill"))
		{
			if (monsterScoreManager != null)
			{
				monsterScoreManager.AddScore(20); // 몬스터 점수 20점 추가
			}
			Destroy(gameObject); // 몬스터 오브젝트 삭제
		}
	}
}
