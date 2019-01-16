using UnityEngine;
using UnityEngine.UI;
public class ScoreText : MonoBehaviour
{
	static Text scoreText;
	void Start()
	{
		scoreText = GetComponent<Text>();
	}
	public static void ScoreTextDisplay(int score)
	{
		scoreText.text = "Score" + '\n' + '\n' + score.ToString();
	}
}
