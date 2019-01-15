using UnityEngine;
using UnityEngine.UI;
public class JudgeText : MonoBehaviour
{
	static Text judgeText;
	static CanvasGroup canvasGroup;
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
		canvasGroup = GetComponent<CanvasGroup>();
	}
	void Update ()
	{
		if (judgeTextFade) JudgeTextFade();
	}
	void JudgeTextFade()
	{
		canvasGroup.alpha -= Time.deltaTime * judgeTextFadeSpeed;
		if (canvasGroup.alpha == 0)
		{
			judgeTextFade = false;
		}
	}
	public static void JudgeTextDisplay(Judge judge)
	{
		if (judge == Judge.SHINY)
		{
			judgeText.text = "<color=#ff0000>S</color><color=#ffff00>H</color><color=#00ff00>I</color><color=#00ffff>N</color><color=#0000ff>Y</color><color=#ff00ff>!</color>";
		}
		else
		{
			judgeText.text = "MISS";
		}
		canvasGroup.alpha = 1;
		judgeTextFade = true;
	}
}
