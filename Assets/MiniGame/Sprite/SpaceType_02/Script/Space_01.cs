using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space_01 : MonoBehaviour
{
	private Coroutine destroyCoroutine;

	// 트리거 충돌이 시작될 때 호출되는 메서드
	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Player 태그를 가진 오브젝트와 충돌했는지 확인
		if (collision.CompareTag("Player"))
		{
			// 코루틴 시작 (2초 후 오브젝트 삭제)
			destroyCoroutine = StartCoroutine(DestroyAfterDelay(1f));
		}
	}

	// 트리거 충돌이 끝날 때 호출되는 메서드
	private void OnTriggerExit2D(Collider2D collision)
	{
		// Player 태그를 가진 오브젝트가 떠났을 때 코루틴 종료
		if (collision.CompareTag("Player"))
		{
			if (destroyCoroutine != null)
			{
				StopCoroutine(destroyCoroutine);
			}
		}
	}

	// 지정된 시간 후 오브젝트를 삭제하는 코루틴
	private IEnumerator DestroyAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
	}
}
