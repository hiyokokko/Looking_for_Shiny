using UnityEngine;
using UnityEngine.UI;
public class ComboText : MonoBehaviour
{
	static Text comboText;
	static Color comboTextColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
	static bool comboTextFade = false;
	float comboTextFadeSpeed = 5.0f;
	void Start()
	{
		comboText = GetComponent<Text>();
	}
	void Update()
	{
		if (comboTextFade) JudgeTextFade();
	}
	void JudgeTextFade()
	{
		Color tempColor = comboText.color;
		tempColor.a -= Time.deltaTime * comboTextFadeSpeed;
		if (tempColor.a < 0)
		{
			tempColor.a = 0;
			comboTextFade = false;
		}
		comboText.color = tempColor;
	}
	public static void ComboTextDisplay(int combo)
	{
		comboText.text = combo.ToString() + " combo";
		comboText.color = comboTextColor;
		comboTextFade = true;
	}
}
