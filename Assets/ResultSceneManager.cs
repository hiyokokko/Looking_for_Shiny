using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ResultSceneManager : MonoBehaviour {

	[SerializeField] Text scoreText;
	[SerializeField] Text[] judgeText;
	[SerializeField] Text comboText;
	[SerializeField] Text rateText;
	// Use this for initialization
	void Start () {
		scoreText.text = "すこあ:" + GameSceneManager.score.ToString("F0");
		judgeText[(int)JudgeEnum.perfect].text = "<color=#ff0000>ぱ</color><color=#ffff00>ぁ</color><color=#00ff00>ふ</color><color=#00ffff>ぇ</color><color=#0000ff>く</color><color=#ff00ff>と</color>：" + GameSceneManager.judge[(int)JudgeEnum.perfect];
		judgeText[(int)JudgeEnum.great].text = "<color=#ffff00>ぐれぇと</color>"  + GameSceneManager.judge[(int)JudgeEnum.great];
		judgeText[(int)JudgeEnum.good].text = "<color=#00ff00>ぐっど</color>" + GameSceneManager.judge[(int)JudgeEnum.good];
		judgeText[(int)JudgeEnum.bad].text = "<color=#0000ff>ばっど</color>" + GameSceneManager.judge[(int)JudgeEnum.bad];
		judgeText[(int)JudgeEnum.miss].text = "<color=#7f7f7f>みす</color>" + GameSceneManager.judge[(int)JudgeEnum.miss];
		comboText.text = "こんぼ：" + GameSceneManager.maxCombo;
	}
	public void SceneLoad(int sceneNum)
	{
		SceneManager.LoadScene(sceneNum);
	}
}
