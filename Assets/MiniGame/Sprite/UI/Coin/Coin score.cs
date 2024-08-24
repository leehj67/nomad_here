using UnityEngine;
using UnityEngine.UI;

public class Coinscore : MonoBehaviour
{
	public int score = 0;
	public Text scoreText;

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
		scoreText.text = "Score: " + score.ToString();
	}
}
