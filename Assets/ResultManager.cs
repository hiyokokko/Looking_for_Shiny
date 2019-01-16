using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
	[SerializeField] Text scoreText;
	[SerializeField] Text shinyText;
	[SerializeField] Text missText;
	[SerializeField] Text maxComboText;

	// Use this for initialization
	void Start () {
		scoreText.text = "Score : " + MainManager.score.ToString();
		shinyText.text = "SHINT! : " + MainManager.judgeCount[(int)Judge.SHINY].ToString();
		missText.text = "MISS : " + MainManager.judgeCount[(int)Judge.MISS].ToString();
		maxComboText.text = "MaxCombo : " + MainManager.comboMax.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
