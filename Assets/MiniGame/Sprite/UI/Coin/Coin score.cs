using UnityEngine;
using UnityEngine.UI;

public class Coinscore : MonoBehaviour
{
	public int score = 0;
	public Text scoreText;

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
		scoreText.text = "CoinScore: " + score.ToString();
	}
}
