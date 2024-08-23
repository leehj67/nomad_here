using System.Collections;
using UnityEngine;

public class PlayerRadar : MonoBehaviour
{
	public Transform player; // 플레이어 오브젝트의 Transform
	public float detectionRange = 5f; // 플레이어를 감지할 거리
	public GameObject projectilePrefab; // 발사체 프리팹
	public Transform firePoint; // 발사체 발사 위치
	public float projectileSpeed = 5f; // 발사체 속도

	private void Start()
	{
		// 1초마다 거리 체크를 시작
		StartCoroutine(CheckPlayerDistance());
	}

	private IEnumerator CheckPlayerDistance()
	{
		while (true)
		{
			// 플레이어와의 거리 계산
			float distanceToPlayer = Vector2.Distance(transform.position, player.position);

			// 설정한 거리 내로 플레이어가 진입했는지 확인
			if (distanceToPlayer <= detectionRange)
			{
				Debug.Log("플레이어 접근");

				// 발사체 발사
				FireProjectile();
			}

			// 1초 대기 후 다시 체크
			yield return new WaitForSeconds(1f);
		}
	}

	private void FireProjectile()
	{
		if (projectilePrefab != null && firePoint != null)
		{
			// 발사체 생성
			GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

			// 플레이어 방향 계산
			Vector2 direction = (player.position - firePoint.position).normalized;

			// 발사체의 진행 방향 설정
			projectile.transform.right = direction;

			// 발사체에 힘을 가해 발사
			Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
			if (rb != null)
			{
				rb.velocity = direction * projectileSpeed;
			}
		}
	}
}
