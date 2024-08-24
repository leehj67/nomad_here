using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
	public int coinValue = 10;  // ���� ȹ�� �� �߰��� ����
	public Coinscore scoreManager;  // Coinscore ��ũ��Ʈ ����

	void OnTriggerEnter2D(Collider2D other)
	{
		// �浹�� ������Ʈ�� "Player" �±׸� �������� Ȯ��
		if (other.CompareTag("Player"))
		{
			// scoreManager�� �Ҵ�Ǿ� �ִ��� Ȯ��
			if (scoreManager != null)
			{
				// ������ �߰��ϰ� UI�� ������Ʈ
				scoreManager.AddScore(coinValue);
			}

			// ���� ������Ʈ ����
			Destroy(gameObject);
		}
	}
}
