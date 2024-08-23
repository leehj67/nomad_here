using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMonster : MonoBehaviour
{
	public GameObject projectilePrefab; // 발사체 프리팹
	public Transform firePoint; // 발사체 발사 위치
	public float fireInterval = 1f; // 발사 간격

	private Transform playerTransform; // 플레이어 위치
	private bool playerInRange = false; // 플레이어가 범위 내에 있는지 여부
	private Coroutine fireCoroutine; // 발사 코루틴

	// 콜라이더의 트리거 진입 시 호출
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerTransform = collision.transform;
			playerInRange = true;

			// 발사 시작
			if (fireCoroutine == null)
			{
				fireCoroutine = StartCoroutine(FireProjectile());
			}
		}
	}

	// 콜라이더의 트리거에서 벗어날 시 호출
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerInRange = false;

			// 발사 중지
			if (fireCoroutine != null)
			{
				StopCoroutine(fireCoroutine);
				fireCoroutine = null;
			}
		}
	}

	// 발사체 발사 루프
	private IEnumerator FireProjectile()
	{
		while (playerInRange)
		{
			// 발사체 생성
			GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

			// 플레이어 방향 계산
			Vector2 direction = (playerTransform.position - firePoint.position).normalized;

			// 발사체의 진행 방향 설정
			projectile.transform.right = direction;

			// 1초 대기
			yield return new WaitForSeconds(fireInterval);
		}
	}
}
