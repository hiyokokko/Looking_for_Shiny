using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgeTextFade : MonoBehaviour {
	[SerializeField] Text judgeText;
	float fadeSpeed = 5.0f;
	void Update ()
	{
		if (judgeText.color.a > 0)
		{
			Color tempColor = judgeText.color;
			tempColor.a -= Time.deltaTime * fadeSpeed ;
			if (tempColor.a < 0)
			{
				tempColor.a = 0;
			}
			judgeText.color = tempColor;
		}
	}
}
