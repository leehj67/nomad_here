using UnityEngine;
using UnityEngine.UI;

public class MonsterScore : MonoBehaviour
{
	public int score = 0;
	public Text[] scoreTexts; // 3���� �ؽ�Ʈ ������Ʈ�� �����ϱ� ���� �迭

	void Start()
	{
		UpdateScoreUI();
	}

	public void AddScore(int value)
	{
		score += value;
		UpdateScoreUI();
	}

	void UpdateScoreUI()
	{
		foreach (Text scoreText in scoreTexts)
		{
			if (scoreText != null)
			{
				scoreText.text = "Monster Score: " + score.ToString();
			}
		}
	}
}
