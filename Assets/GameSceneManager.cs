using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameSceneManager : MonoBehaviour
{
	//全般 変数宣言//////////////////////////////
	[SerializeField] Camera camera;
	float time;
	/////////////////////////////////////////////

	//サウンド関連 変数宣言//////////////////////
	[SerializeField] AudioSource songAudio;
	[SerializeField] AudioSource judgeAudio;
	float songPlayWaitTime = 5.0f;
	bool songPlaying;
	float songPlayingTime;
	/////////////////////////////////////////////

	//ノーツ関連 変数宣言////////////////////////
	[SerializeField] GameObject notePrefab;
	Vector3[] noteBasicPos = new Vector3[2]
	{
		new Vector3(1.0f, 32.0f, 32.0f),
		new Vector3(-7.0f, 32.0f, 32.0f)
	};
	List<NoteData>[] noteDatas;
	List<NoteData>[] noteInstDatas;
	int[] noteSpawnCount;
	bool[] noteSpawnEnd;
	/////////////////////////////////////////////

	//タッチ関連 変数宣言////////////////////////
	[SerializeField] GameObject[] lift;
	Vector2 halfScreenPos = new Vector2(960.0f, 540.0f);
	Vector2[] liftPosRest = new Vector2[2]
	{
		new Vector2(1.0f, 7.0f),
		new Vector2(-7.0f, -1.0f)
	};
	bool[] liftActive;
	bool touchDown;
	bool touchUp;
	/////////////////////////////////////////////

	//判定関連 変数宣言//////////////////////////
	[SerializeField] bool autoPlay;
	float[] tapJudgeTime = new float[3]
	{
		0.033f,
		0.066f,
		0.099f
	};
	float[] holdJudgePos = new float[3]
	{
		0.6f,
		0.9f,
		1.2f
	};
	float[] scoreJudgeCoef = new float[4]
	{
		4.0f,
		2.0f,
		1.0f,
		0.0f
	};
	float[] scoreFeverCoef = new float[2]
	{
		1.0f,
		2.0f
	};
	int maxHealth = 1000;
	float tapJudgePosX = 1.2f;
	public static RecordData recordData;
	int nowCombo;
	int nowHealth;
	/////////////////////////////////////////////

	//UI関連 変数宣言////////////////////////////
	[SerializeField] Text judgeText;
	[SerializeField] CanvasGroup judgeTextCanvasGroup;
	[SerializeField] Text comboText;
	[SerializeField] CanvasGroup comboTextCanvasGroup;
	[SerializeField] Text scoreText;
	[SerializeField] Text healthText;
	[SerializeField] Image healthBarImage;
	float textFadeSpeed = 5.0f;
	bool judgeTextFadeOut;
	bool comboTextFadeOut;
	/////////////////////////////////////////////

	//MonoBehaviour関数//////////////////////////
	void Awake()
	{
		time = 0.0f;
		songPlaying = false;
		songPlayingTime = 0.0f;
		noteDatas = new List<NoteData>[2]
		{
			new List<NoteData>(),
			new List<NoteData>()
		};
		noteInstDatas = new List<NoteData>[2]
		{
			new List<NoteData>(),
			new List<NoteData>()
		};
		noteSpawnCount = new int[2];
		noteSpawnEnd = new bool[2];
		liftActive = new bool[2];
		touchDown = false;
		touchUp = false;
		recordData = new RecordData();
		nowCombo = 0;
		nowHealth = maxHealth;
		judgeTextFadeOut = false;
		comboTextFadeOut = false;
	}
	void Start()
	{
		//仮実装////////////////////
		//SelectSceneManagerが完成したらAwake関数に移動//////////
		for (LaneEnum laneEnum = LaneEnum.right; laneEnum != LaneEnum.none; laneEnum++)
		{
			int laneInt = (int)laneEnum;
			lift[laneInt].GetComponent<Renderer>().material.color = SelectSceneManager.liftColor;
			lift[laneInt].GetComponent<Renderer>().material.SetColor("_EmissionColor", SelectSceneManager.liftColor);
		}
		songAudio.clip = (AudioClip)Resources.Load(SelectSceneManager.songFileString + "/Song");
		NoteDataRead(SelectSceneManager.songFileString);
		/////////////////////////////////////////////////////////
		///////////////////////////
	}
	void Update()
	{
		//仮実装////////////////////
		//現在は左クリックでタッチ動作をさせている。後でスマホ実機に対応させる。/////
		touchDown = Input.GetMouseButtonDown(0);
		touchUp = Input.GetMouseButtonUp(0);
		/////////////////////////////////////////////////////////////////////////////
		///////////////////////////
	}
	void FixedUpdate ()
	{
		time += Time.fixedDeltaTime;
		if (!songPlaying && time >= songPlayWaitTime) SongPlay();
		if (songPlaying) SongPlaying();
		if (touchDown) TouchDown();
		if (touchUp) TouchUp();
		for (LaneEnum laneEnum = LaneEnum.right; laneEnum != LaneEnum.none; laneEnum++)
		{
			int laneInt = (int)laneEnum;
			if (liftActive[laneInt]) LiftActive(laneEnum);
		}
		if (judgeTextFadeOut) judgeTextFadeOut = TextFadeOut(judgeTextCanvasGroup);
		if (comboTextFadeOut) comboTextFadeOut = TextFadeOut(comboTextCanvasGroup);
	}
	/////////////////////////////////////////////

	//サウンド関連///////////////////////////////
	void SongPlay()
	{
		songAudio.Play();
		songPlayingTime = time - songPlayWaitTime;
		songPlaying = true;
	}
	void SongPlaying()
	{
		for (LaneEnum laneEnum = LaneEnum.right; laneEnum != LaneEnum.none; laneEnum++)
		{
			int laneInt = (int)laneEnum;
			NoteSpawn(laneEnum);
			if (autoPlay && noteInstDatas[laneInt].Count > 0) AutoPlay(laneEnum);
			for (int noteNum = 0; noteNum != noteInstDatas[laneInt].Count; noteNum++)
			{
				NoteMove(laneEnum, noteNum);
				if (noteInstDatas[laneInt][noteNum].holdJudge) HoldJudgeCheck(laneEnum, noteNum);
				BadJudgeCheck(laneEnum, ref noteNum);
			}
		}
		songPlayingTime += Time.fixedDeltaTime;
	}
	/////////////////////////////////////////////

	//ノーツ関連 関数////////////////////////////
	void NoteDataRead(string songFileString)
	{
		string[] noteDataRead = ((TextAsset)Resources.Load(songFileString + "/NoteData")).text.Split('\n');
		float bpm = SelectSceneManager.musicData.basicBpm;
		float noteSpeed = 1.0f;
		float[] notePerfectJudgeTime = new float[2]
		{
			SelectSceneManager.musicData.offset,
			SelectSceneManager.musicData.offset
		};
		FeverEnum feverEnum = FeverEnum.no;
		int noteDataReadLine = 0;
		while (noteDataReadLine != noteDataRead.Length)
		{
			//コマンド読み取り
			string[] commandRead = noteDataRead[noteDataReadLine++].Split('/');
			int commandReadLine = 0;
			while (commandReadLine != commandRead.Length)
			{
				switch (commandRead[commandReadLine++])
				{
					case "bpm":
						bpm = float.Parse(commandRead[commandReadLine++]);
						break;
					case "speed":
						noteSpeed = float.Parse(commandRead[commandReadLine++]);
						break;
					case "fever":
						switch (commandRead[commandReadLine++][0])
						{
							case '0':
								feverEnum = FeverEnum.no;
								break;
							case '1':
								feverEnum = FeverEnum.yes;
								break;
						}
						break;
					case "comment":
						commandReadLine++;
						break;
				}
			}
			//ノーツ配置読み取り
			for (LaneEnum laneEnum = LaneEnum.right; laneEnum != LaneEnum.none; laneEnum++)
			{
				int laneInt = (int)laneEnum;
				string[] notePosRead = noteDataRead[noteDataReadLine++].Split('/');
				for (int notePosReadLine = 0; notePosReadLine != notePosRead.Length; notePosReadLine++)
				{
					if (notePosRead[notePosReadLine][0] != '*')
					{
						noteDatas[(int)laneEnum].Add
						(
							new NoteData()
							{
								noteSpeed = SelectSceneManager.noteBasicSpeed * noteSpeed,
								notePerfectJudgeTime = notePerfectJudgeTime[laneInt],
								noteSpawnTime = notePerfectJudgeTime[laneInt] - noteBasicPos[laneInt].y / (SelectSceneManager.noteBasicSpeed * noteSpeed),
								noteSpawnPosX = float.Parse(notePosRead[notePosReadLine]),
								feverEnum = feverEnum,
								laneEnum = laneEnum
							}
						);
					}
					notePerfectJudgeTime[laneInt] += (60.0f / bpm) * (4.0f / notePosRead.Length);
				}
			}
		}
		noteDatas[(int)LaneEnum.right].Sort(NoteSpawnTimeCompare);
		noteDatas[(int)LaneEnum.left].Sort(NoteSpawnTimeCompare);
	}
	void NoteSpawn(LaneEnum laneEnum)
	{
		int laneInt = (int)laneEnum;
		if (!noteSpawnEnd[laneInt] && noteDatas[laneInt][noteSpawnCount[laneInt]].noteSpawnTime <= songPlayingTime)
		{
			NoteData tempNoteData = noteDatas[laneInt][noteSpawnCount[laneInt]];
			float gapPos = -(songPlayingTime - tempNoteData.noteSpawnTime) * tempNoteData.noteSpeed;
			Color noteColor = SelectSceneManager.noteColor[(int)tempNoteData.feverEnum];
			Vector3 notePos = noteBasicPos[laneInt] + new Vector3(tempNoteData.noteSpawnPosX, gapPos, gapPos);
			tempNoteData.noteInst = Instantiate(notePrefab, notePos, Quaternion.identity);
			tempNoteData.noteInst.GetComponent<Renderer>().material.color = noteColor;
			tempNoteData.noteInst.GetComponent<Renderer>().material.SetColor("_EmissionColor", noteColor);
			noteInstDatas[laneInt].Add(tempNoteData);
			noteInstDatas[laneInt].Sort(NotePerfectJudgeTimeCompare);
			noteSpawnCount[laneInt]++;
			if (noteSpawnCount[laneInt] == noteDatas[laneInt].Count) noteSpawnEnd[laneInt] = true;
		}
	}
	void NoteMove(LaneEnum laneEnum, int noteNum)
	{
		int laneInt = (int)laneEnum;
		Vector3 movePos = (transform.up + transform.forward) * noteInstDatas[laneInt][noteNum].noteSpeed * Time.fixedDeltaTime;
		noteInstDatas[laneInt][noteNum].noteInst.transform.position -= movePos;
	}
	/////////////////////////////////////////////

	//判定関連 関数//////////////////////////////
	void Judge(JudgeEnum judgeEnum, LaneEnum laneEnum, int noteNum)
	{
		int judgeInt = (int)judgeEnum;
		int laneInt = (int)laneEnum;
		int feverInt = (int)noteInstDatas[laneInt][noteNum].feverEnum;
		float scoreCoef = scoreJudgeCoef[judgeInt] * scoreFeverCoef[feverInt];
		switch (judgeEnum)
		{
			case JudgeEnum.perfect:
				judgeAudio.Play();
				nowCombo++;
				recordData.score += (int)(scoreCoef * nowCombo * float.Parse(Mathf.Sqrt(nowCombo).ToString("F2")));
				nowHealth = nowHealth + SelectSceneManager.musicData.heel < maxHealth ? nowHealth + SelectSceneManager.musicData.heel : maxHealth;
				break;
			case JudgeEnum.great:
				judgeAudio.Play();
				nowCombo++;
				recordData.score += (int)(scoreCoef * nowCombo * float.Parse(Mathf.Sqrt(nowCombo).ToString("F2")));
				nowHealth = nowHealth + SelectSceneManager.musicData.heel < maxHealth ? nowHealth + SelectSceneManager.musicData.heel : maxHealth;
				break;
			case JudgeEnum.good:
				nowCombo /= 2;
				recordData.score += (int)(scoreCoef * nowCombo * float.Parse(Mathf.Sqrt(nowCombo).ToString("F2")));
				break;
			case JudgeEnum.bad:
				nowCombo = 0;
				nowHealth = nowHealth - SelectSceneManager.musicData.damage > 0 ? nowHealth - SelectSceneManager.musicData.damage : 0;
				break;
		}
		recordData.judge[judgeInt]++;
		JudgeTextUpdate(judgeEnum);
		ComboTextUpdate();
		ScoreTextUpdate();
		HealthTextUpdate();
		HealthBarImageUpdate();
		Destroy(noteInstDatas[laneInt][noteNum].noteInst);
		noteInstDatas[laneInt].RemoveAt(noteNum);
	}
	void TapJudgeCheck(LaneEnum laneEnum)
	{
		int laneInt = (int)laneEnum;
		int noteNum = 0;
		float liftPosX = lift[laneInt].transform.position.x;
		if (noteInstDatas[laneInt].Count > noteNum && liftPosX + tapJudgePosX >= noteInstDatas[laneInt][noteNum].noteInst.transform.position.x - tapJudgePosX && liftPosX - tapJudgePosX <= noteInstDatas[laneInt][noteNum].noteInst.transform.position.x + tapJudgePosX)
		{
			float notePerfectJudgeTime = noteInstDatas[laneInt][noteNum].notePerfectJudgeTime;
			for (JudgeEnum judgeEnum = JudgeEnum.perfect; judgeEnum != JudgeEnum.bad; judgeEnum++)
			{
				int judgeInt = (int)judgeEnum;
				if (songPlayingTime >= notePerfectJudgeTime - tapJudgeTime[judgeInt] && songPlayingTime <= notePerfectJudgeTime + tapJudgeTime[judgeInt])
				{
					Judge(judgeEnum, laneEnum, noteNum);
					break;
				}
			}
			
		}
	}
	void HoldJudgeCheck(LaneEnum laneEnum, int noteNum)
	{
		
	}
	void BadJudgeCheck(LaneEnum laneEnum, ref int noteNum)
	{
		int laneInt = (int)laneEnum;
		float notePerfectJudgeTime = noteInstDatas[laneInt][noteNum].notePerfectJudgeTime;
		if (songPlayingTime > notePerfectJudgeTime + tapJudgeTime[(int)JudgeEnum.good])
		{
			Judge(JudgeEnum.bad, laneEnum, noteNum);
			noteNum--;
		}
	}
	void AutoPlay(LaneEnum laneEnum)
	{
		int laneInt = (int)laneEnum;
		int noteNum = 0;
		float notePerfectJudgeTime = noteInstDatas[laneInt][noteNum].notePerfectJudgeTime;
		if (songPlayingTime >= notePerfectJudgeTime) Judge(JudgeEnum.perfect, laneEnum, noteNum);
	}
	/////////////////////////////////////////////

	//タッチ関連 関数////////////////////////////

	void TouchDown()
	{
		Vector3 tapPos = Input.mousePosition;
		if (tapPos.y < halfScreenPos.y)
		{
			LaneEnum laneEnum = tapPos.x >= halfScreenPos.x ? LaneEnum.right : LaneEnum.left;
			int laneInt = (int)laneEnum;
			lift[laneInt].SetActive(true);
			liftActive[laneInt] = true;
			LiftActive(laneEnum);
			TapJudgeCheck(laneEnum);
		}
	}
	void TouchUp()
	{
		for (LaneEnum laneEnum = LaneEnum.right; laneEnum != LaneEnum.none; laneEnum++)
		{
			int laneInt = (int)laneEnum;
			lift[laneInt].SetActive(false);
			liftActive[laneInt] = false;
		}
	}
	void LiftActive(LaneEnum laneEnum)
	{
		int laneInt = (int)laneEnum;
		Vector3 tapPos = Input.mousePosition;
		tapPos.z = -9.0f;
		Vector3 tapWorldPos = -camera.ScreenToWorldPoint(tapPos);
		float liftPosX = tapWorldPos.x;
		liftPosX = liftPosX >= liftPosRest[laneInt].x ? liftPosX : liftPosRest[laneInt].x;
		liftPosX = liftPosX < liftPosRest[laneInt].y ? liftPosX : liftPosRest[laneInt].y;
		lift[laneInt].transform.position = transform.right * liftPosX;
	}
	/////////////////////////////////////////////

	//UI関連 関数////////////////////////////////
	void JudgeTextUpdate(JudgeEnum judgeEnum)
	{
		switch (judgeEnum)
		{
			case JudgeEnum.perfect:
				//虹色で"ぱぁふぇくと"
				judgeText.text = "<color=#ff0000>ぱ</color><color=#ffff00>ぁ</color><color=#00ff00>ふ</color><color=#00ffff>ぇ</color><color=#0000ff>く</color><color=#ff00ff>と</color>";
				break;
			case JudgeEnum.great:
				//黄色で"ぐれぇと"
				judgeText.text = "<color=#ffff00>ぐれぇと</color>";
				break;
			case JudgeEnum.good:
				//緑色で"ぐれぇと"
				judgeText.text = "<color=#00ff00>ぐっど</color>";
				break;
			case JudgeEnum.bad:
				//青色で"ばっど"
				judgeText.text = "<color=#0000ff>ばっど</color>";
				break;
		}
		judgeTextCanvasGroup.alpha = 1;
		judgeTextFadeOut = true;
	}
	void ComboTextUpdate()
	{
		comboText.text = nowCombo.ToString();
		comboTextCanvasGroup.alpha = 1;
		comboTextFadeOut = true;
	}
	void ScoreTextUpdate()
	{
		scoreText.text = "Score" + '\n' + '\n' + recordData.score.ToString();
	}
	void HealthTextUpdate()
	{
		healthText.text = nowHealth.ToString();
	}
	void HealthBarImageUpdate()
	{
		healthBarImage.fillAmount = nowHealth / 1000.0f;
	}
	bool TextFadeOut(CanvasGroup canvasGroup)
	{
		canvasGroup.alpha -= textFadeSpeed * Time.fixedDeltaTime;
		if (canvasGroup.alpha == 0)
		{
			return false;
		}
		return true;
	}
	/////////////////////////////////////////////

	//ソート関連 関数////////////////////////////
	int NoteSpawnTimeCompare(NoteData a, NoteData b)
	{
		if (a.noteSpawnTime >= b.noteSpawnTime)
		{
			return 1;
		}
		else if (a.noteSpawnTime <= b.noteSpawnTime)
		{
			return -1;
		}
		return 0;
	}
	int NotePerfectJudgeTimeCompare(NoteData a, NoteData b)
	{

		if (a.notePerfectJudgeTime >= b.notePerfectJudgeTime)
		{
			return 1;
		}
		else if (a.notePerfectJudgeTime <= b.notePerfectJudgeTime)
		{
			return -1;
		}
		return 0;
	}
	/////////////////////////////////////////////
}
