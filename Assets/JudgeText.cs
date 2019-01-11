using UnityEngine;
using UnityEngine.UI;
public class JudgeText : MonoBehaviour
{
	static Text judgeText;
	static Color[] judgeTextColor = new Color[3]
	{
		new Color(1.0f, 0.0f, 0.0f, 1.0f),
		new Color(0.0f, 1.0f, 0.0f, 1.0f),
		new Color(0.0f, 0.0f, 1.0f, 1.0f)
	};
	static bool judgeTextFade = false;
	float judgeTextFadeSpeed = 5.0f;
	void Start()
	{
		judgeText = GetComponent<Text>();
	}
	void Update ()
	{
		if (judgeTextFade) JudgeTextFade();
	}
	void JudgeTextFade()
	{
		Color tempColor = judgeText.color;
		tempColor.a -= Time.deltaTime * judgeTextFadeSpeed;
		if (tempColor.a < 0)
		{
			tempColor.a = 0;
			judgeTextFade = false;
		}
		judgeText.color = tempColor;
	}
	public static void JudgeTextDisplay(Judge judge)
	{
		judgeText.text = judge.ToString();
		judgeText.color = judgeTextColor[(int)judge];
		judgeTextFade = true;
	}
}
