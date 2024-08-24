using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
	public int coinValue = 10;  // 코인 획득 시 추가할 점수
	public Coinscore scoreManager;  // Coinscore 스크립트 참조

	void OnTriggerEnter2D(Collider2D other)
	{
		// 충돌한 오브젝트가 "Player" 태그를 가졌는지 확인
		if (other.CompareTag("Player"))
		{
			// scoreManager가 할당되어 있는지 확인
			if (scoreManager != null)
			{
				// 점수를 추가하고 UI를 업데이트
				scoreManager.AddScore(coinValue);
			}

			// 코인 오브젝트 삭제
			Destroy(gameObject);
		}
	}
}
