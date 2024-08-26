using UnityEngine;
using UnityEngine.UI;

public class Coinscore : MonoBehaviour
{
	public int score = 0;
	public Text[] scoreTexts; // 3���� �ؽ�Ʈ ������Ʈ�� �����ϱ� ���� �迭

	private static Coinscore instance;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

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
				scoreText.text = "CoinScore: " + score.ToString();
			}
		}
	}
}
