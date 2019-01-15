using UnityEngine;
using UnityEngine.UI;
public class ComboText : MonoBehaviour
{
	static Text comboText;
	static CanvasGroup canvasGroup;
	static bool judgeTextFade = false;
	float judgeTextFadeSpeed = 5.0f;
	void Start()
	{
		comboText = GetComponent<Text>();
		canvasGroup = GetComponent<CanvasGroup>();
	}
	void Update()
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
	public static void ComboTextDisplay(int combo)
	{
		comboText.text = combo.ToString();
		canvasGroup.alpha = 1;
		judgeTextFade = true;
	}
}
