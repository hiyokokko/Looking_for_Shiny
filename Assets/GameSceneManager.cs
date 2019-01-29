using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameSceneManager : MonoBehaviour
{
	//全般 変数宣言//////////////////////////////
	[SerializeField] Camera cam;
	float time;
	/////////////////////////////////////////////

	//サウンド関連 変数宣言//////////////////////
	[SerializeField] AudioSource songAudio;
	[SerializeField] AudioSource judgeAudio;
	[SerializeField] AudioClip judgeClip;
	float songPlayWaitTime = 5.0f;
	bool songPlaying;
	float songPlayingTime;
	bool firstSongPlayingUpdate;
	float songPlayingUpdateTime;
	/////////////////////////////////////////////

	//ノーツ関連 変数宣言////////////////////////
	[SerializeField] Transform notePrefab;
	[SerializeField] ParticleSystem noteParticlePrefab;
	Vector3[] noteBasicPos = new Vector3[2]
	{
		new Vector3(1.0f, 24.0f, 24.0f),
		new Vector3(-7.0f, 24.0f, 24.0f)
	};
	List<NoteData>[] noteDatas;
	List<NoteData>[] noteInstDatas;
	int[] noteSpawnCount;
	bool[] noteSpawnEnd;
	/////////////////////////////////////////////

	//タッチ関連 変数宣言////////////////////////
	[SerializeField] Transform[] lift;
	[SerializeField] Transform[] liftLine;
	Vector2 halfScreenPos = new Vector2(960.0f, 540.0f);
	Vector2[] liftPosRest = new Vector2[2]
	{
		new Vector2(1.0f, 7.0f),
		new Vector2(-7.0f, -1.0f)
	};
	Vector3 liftLineBasicPos = new Vector3(0.0f, 11.5f, 11.5f);
	bool touchDown;
	bool touchUp;
	bool[] touchHold;
	int[] touchHoldFingerId;
	/////////////////////////////////////////////

	//判定関連 変数宣言//////////////////////////
	[SerializeField] bool[] autoPlay;
	float[] tapJudgeTime = new float[4]
	{
		0.050f,
		0.083f,
		0.116f,
		0.150f
	};
	float[] holdJudgePos = new float[4]
	{
		0.6f,
		0.9f,
		1.2f,
		1.5f
	};
	int[] judgeCoef = new int[5]
	{
		4,
		2,
		1,
		0,
		0
	};
	int[] feverCoef = new int[2]
	{
		1,
		2
	};
	float maxHealth = 100.0f;
	float tapJudgePosX = 1.5f;
	public static float score;
	public static int[] judge;
	float nowCombo;
	public static float maxCombo;
	float nowHealth;
	public static int judgeCount;
	float gapTimeAdd = 0.0f;
	/////////////////////////////////////////////

	//UI関連 変数宣言////////////////////////////

	//デバッグ用
	[SerializeField] Text fpsText;
	[SerializeField] Text gapText;
	[SerializeField] Text inputGapText;

	[SerializeField] Text judgeText;
	[SerializeField] CanvasGroup judgeTextCanvasGroup;
	[SerializeField] Text comboText;
	[SerializeField] CanvasGroup comboTextCanvasGroup;
	[SerializeField] Text scoreText;
	[SerializeField] Text healthText;
	[SerializeField] Slider healthSlider;
	float textFadeSpeed = 5.0f;
	bool judgeTextFadeOut;
	bool comboTextFadeOut;
	/////////////////////////////////////////////

	//MonoBehaviour関数//////////////////////////
	void Awake()
	{
		time = 0.0f;
		firstSongPlayingUpdate = true;
		songPlayingUpdateTime = 0.0f;
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
		touchHold = new bool[2];
		touchHoldFingerId = new int[2]
		{
			-1,
			-1
		}
		;
		touchDown = false;
		touchUp = false;
		score = 0.0f;
		judge = new int[5];
		nowCombo = 0.0f;
		maxCombo = 0.0f;
		nowHealth = maxHealth;
		judgeCount = 0;
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
			lift[laneInt].GetComponent<SpriteRenderer>().color = SelectSceneManager.liftColor;
			liftLine[laneInt].GetComponent<SpriteRenderer>().color = SelectSceneManager.liftLineColor;
		}
		songAudio.clip = (AudioClip)Resources.Load("SongFile/" + SelectSceneManager.selectSongFileString + "/Song");
		NoteDataRead(SelectSceneManager.selectSongFileString);
		/////////////////////////////////////////////////////////
		///////////////////////////
	}
	void Update()
	{
		//FPS計測
		float fps = 1f / Time.deltaTime;
		fpsText.text = "FPS:" + fps.ToString("F0");
		if (songPlaying)
		{
			if (firstSongPlayingUpdate)
			{
				songPlayingUpdateTime += songPlayingTime;
				firstSongPlayingUpdate = false;
			}
			else
			{
				songPlayingUpdateTime += Time.deltaTime;
			}
		}
		TouchCheck();
		if (judgeTextFadeOut) judgeTextFadeOut = TextFadeOut(judgeTextCanvasGroup);
		if (comboTextFadeOut) comboTextFadeOut = TextFadeOut(comboTextCanvasGroup);
		//PC判定確認用
		if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.J))
		{
			tapJudgePosX = 10.0f;
			for (LaneEnum laneEnum = LaneEnum.right; laneEnum != LaneEnum.none; laneEnum++)
			{
				int laneInt = (int)laneEnum;
				if (noteInstDatas[laneInt].Count > 0) TapJudgeCheck(laneEnum, 0);
			}
			tapJudgePosX = 1.5f;
		}

	}
	void FixedUpdate ()
	{
		if (Input.GetKey(KeyCode.Space)) Debug.Log(Input.GetKeyDown(KeyCode.Space));
		time += Time.fixedDeltaTime;
		if (!songPlaying && time >= songPlayWaitTime) SongPlay();
		if (songPlaying) SongPlaying();

	}
	/////////////////////////////////////////////

	//サウンド関連///////////////////////////////
	void SongPlay()
	{
		songAudio.Play();
		songPlaying = true;
	}
	void SongPlaying()
	{
		for (LaneEnum laneEnum = LaneEnum.right; laneEnum != LaneEnum.none; laneEnum++)
		{
			int laneInt = (int)laneEnum;
			NoteSpawn(laneEnum);
			if (autoPlay[laneInt] && noteInstDatas[laneInt].Count > 0) AutoPlay(laneEnum);
			for (int noteNum = 0; noteNum != noteInstDatas[laneInt].Count; noteNum++)
			{
				NoteMove(laneEnum, noteNum);
				if (noteInstDatas[laneInt][noteNum].holdJudge)
				{
					if (HoldJudgeCheck(laneEnum, noteNum)) noteNum--;
				}
				else
				{
					if (MissJudgeCheck(laneEnum, noteNum)) noteNum--;
				}
			}
		}
		songPlayingTime += Time.fixedDeltaTime;
		if (songAudio.time == 0 && !songAudio.isPlaying) GameClear();
	}
	/////////////////////////////////////////////

	//ノーツ関連 関数////////////////////////////
	void NoteDataRead(string songFileString)
	{
		string[] noteDataRead = ((TextAsset)Resources.Load("SongFile/" + songFileString + "/NoteData")).text.Split('\n');
		float bpm = SelectSceneManager.musicDatas[SelectSceneManager.selectSongFileInt].basicBpm;
		float noteSpeed = 1.0f;
		float[] notePerfectJudgeTime = new float[2]
		{
			SelectSceneManager.musicDatas[SelectSceneManager.selectSongFileInt].offset + SelectSceneManager.judgeGapTime,
			SelectSceneManager.musicDatas[SelectSceneManager.selectSongFileInt].offset + SelectSceneManager.judgeGapTime
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
								noteColor = SelectSceneManager.noteColor[(int)feverEnum],
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
			float noteGapPos = -(songPlayingTime - tempNoteData.noteSpawnTime) * tempNoteData.noteSpeed;
			Vector3 notePos = noteBasicPos[laneInt] + new Vector3(tempNoteData.noteSpawnPosX, noteGapPos, noteGapPos);
			Vector3 noteRot = new Vector3(45.0f, 0.0f, 0.0f);
			Color noteColor = tempNoteData.noteColor;
			tempNoteData.noteInst = Instantiate(notePrefab, notePos, Quaternion.Euler(noteRot));
			tempNoteData.noteInst.GetComponent<SpriteRenderer>().color = noteColor;
			noteInstDatas[laneInt].Add(tempNoteData);
			noteInstDatas[laneInt].Sort(NotePerfectJudgeTimeCompare);
			noteSpawnCount[laneInt]++;
			if (noteSpawnCount[laneInt] == noteDatas[laneInt].Count) noteSpawnEnd[laneInt] = true;
		}
	}

	void NoteSpawnA(LaneEnum laneEnum, float songTime)
	{
		int laneInt = (int)laneEnum;
		if (!noteSpawnEnd[laneInt] && noteDatas[laneInt][noteSpawnCount[laneInt]].noteSpawnTime <= songTime)
		{
			NoteData tempNoteData = noteDatas[laneInt][noteSpawnCount[laneInt]];
			float noteGapPos = -(songTime - tempNoteData.noteSpawnTime) * tempNoteData.noteSpeed;
			Vector3 notePos = noteBasicPos[laneInt] + new Vector3(tempNoteData.noteSpawnPosX, noteGapPos, noteGapPos);
			Vector3 noteRot = new Vector3(45.0f, 0.0f, 0.0f);
			Color noteColor = tempNoteData.noteColor;
			tempNoteData.noteInst = Instantiate(notePrefab, notePos, Quaternion.Euler(noteRot));
			tempNoteData.noteInst.GetComponent<SpriteRenderer>().color = noteColor;
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

	void NoteMoveA(LaneEnum laneEnum, int noteNum, float songTime)
	{
		int laneInt = (int)laneEnum;
		NoteData tempNoteData = noteDatas[laneInt][noteSpawnCount[laneInt]];
		Vector3 movePos = (transform.up + transform.forward) * noteInstDatas[laneInt][noteNum].noteSpeed * Time.deltaTime;
		noteInstDatas[laneInt][noteNum].noteInst.transform.position -= movePos;
	}
	/////////////////////////////////////////////

	//判定関連 関数//////////////////////////////
	void Judge(JudgeEnum judgeEnum, LaneEnum laneEnum, int noteNum)
	{
		int judgeInt = (int)judgeEnum;
		int laneInt = (int)laneEnum;
		int feverInt = (int)noteInstDatas[laneInt][noteNum].feverEnum;
		judgeCount++;
		judge[judgeInt]++;
		JudgeTextUpdate(judgeEnum);
		if (judgeEnum == JudgeEnum.perfect || judgeEnum == JudgeEnum.great || judgeEnum == JudgeEnum.good)
		{
			int tempJudgeCoef = judgeCoef[judgeInt];
			int tempFeverCoef = feverCoef[feverInt];
			float tempHealth = nowHealth + tempJudgeCoef * tempFeverCoef * SelectSceneManager.musicDatas[SelectSceneManager.selectSongFileInt].heel;
			Vector3 noteParticlePos = noteInstDatas[laneInt][noteNum].noteInst.transform.position;
			Color noteParticleColor = noteInstDatas[laneInt][noteNum].noteColor;
			ParticleSystem noteParticleInst = Instantiate(noteParticlePrefab, noteParticlePos, Quaternion.identity);
			ParticleSystem.MainModule noteParticleInstMain = noteParticleInst.main;
			noteParticleInstMain.startColor = noteParticleColor;
			noteParticleInst.Play();
			judgeAudio.PlayOneShot(judgeClip);
			nowCombo++;
			maxCombo = Mathf.Max(maxCombo, nowCombo);
			score += tempJudgeCoef * tempFeverCoef * Mathf.Sqrt(nowCombo) * Mathf.Sqrt(SelectSceneManager.musicDatas[SelectSceneManager.selectSongFileInt].difficulty) * 10.0f;
			nowHealth = tempHealth <= maxHealth ? tempHealth : maxHealth;
			ScoreTextUpdate();
			ComboTextUpdate();
		}
		else if (judgeEnum == JudgeEnum.bad || judgeEnum == JudgeEnum.miss)
		{
			int tempFeverCoef = feverCoef[feverInt];
			float tempHealth = nowHealth - tempFeverCoef * SelectSceneManager.musicDatas[SelectSceneManager.selectSongFileInt].damage;
			nowCombo = 0;
			nowHealth = tempHealth >= 0 ? tempHealth : 0;
			if (nowHealth == 0) GameOver();
		}
		HealthTextUpdate();
		HealthSliderImageUpdate();
		Destroy(noteInstDatas[laneInt][noteNum].noteInst.gameObject);
		noteInstDatas[laneInt].RemoveAt(noteNum);
	}
	void TapJudgeCheck(LaneEnum laneEnum, int noteNum)
	{
		int laneInt = (int)laneEnum;
		float liftPosX = lift[laneInt].transform.position.x;
		float notePosX = noteInstDatas[laneInt][noteNum].noteInst.transform.position.x;

		float inputGapTime = songPlayingTime - songAudio.time;


		//判定デバッグ
		inputGapText.text = "inputgap: " + inputGapTime;
		gapTimeAdd += noteInstDatas[laneInt][noteNum].notePerfectJudgeTime - songPlayingTime;
		judgeCount++;
		gapText.text = "gap:" + (gapTimeAdd / judgeCount);


		if (liftPosX + tapJudgePosX >= notePosX - tapJudgePosX && liftPosX - tapJudgePosX <= notePosX + tapJudgePosX)
		{
			float notePerfectJudgeTime = noteInstDatas[laneInt][noteNum].notePerfectJudgeTime;
			for (JudgeEnum judgeEnum = JudgeEnum.perfect; judgeEnum != JudgeEnum.miss; judgeEnum++)
			{
				int judgeInt = (int)judgeEnum;
				if (songPlayingTime >= notePerfectJudgeTime - tapJudgeTime[judgeInt] && songPlayingTime <= notePerfectJudgeTime + tapJudgeTime[judgeInt])
				{
					Judge(judgeEnum, laneEnum, noteNum);
					return;
				}
			}
		}
	}
	bool HoldJudgeCheck(LaneEnum laneEnum, int noteNum)
	{
		int laneInt = (int)laneEnum;
		float notePerfectJudgeTime = noteInstDatas[laneInt][noteNum].notePerfectJudgeTime;
		if (songPlayingTime >= notePerfectJudgeTime)
		{
			noteInstDatas[laneInt][noteNum].holdJudge = false;
			if (!touchHold[laneInt]) return false;
			float liftPosX = lift[laneInt].transform.position.x;
			float notePosX = noteInstDatas[laneInt][noteNum].noteInst.transform.position.x;
			for (JudgeEnum judgeEnum = JudgeEnum.perfect; judgeEnum != JudgeEnum.miss; judgeEnum++)
			{
				int judgeInt = (int)judgeEnum;
				if (liftPosX + holdJudgePos[judgeInt] >= notePosX - holdJudgePos[judgeInt] && liftPosX - holdJudgePos[judgeInt] <= notePosX + holdJudgePos[judgeInt])
				{
					Judge(judgeEnum, laneEnum, noteNum);
					return true;
				}
			}
		}
		return false;
	}
	bool MissJudgeCheck(LaneEnum laneEnum, int noteNum)
	{
		int laneInt = (int)laneEnum;
		float notePerfectJudgeTime = noteInstDatas[laneInt][noteNum].notePerfectJudgeTime;
		if (songPlayingTime > notePerfectJudgeTime + tapJudgeTime[(int)JudgeEnum.bad])
		{
			Judge(JudgeEnum.miss, laneEnum, noteNum);
			return true;
		}
		return false;
	}
	void AutoPlay(LaneEnum laneEnum)
	{
		int laneInt = (int)laneEnum;
		int noteNum = 0;
		float notePerfectJudgeTime = noteInstDatas[laneInt][noteNum].notePerfectJudgeTime;
		//if (songPlayingTime >= notePerfectJudgeTime) Judge(JudgeEnum.perfect, laneEnum, noteNum);
		if (songPlayingTime >= notePerfectJudgeTime) TapJudgeCheck(laneEnum, noteNum);
	}
	void AutoPlayA(LaneEnum laneEnum, float songTime)
	{
		int laneInt = (int)laneEnum;
		int noteNum = 0;
		float notePerfectJudgeTime = noteInstDatas[laneInt][noteNum].notePerfectJudgeTime;
		//if (songPlayingTime >= notePerfectJudgeTime) Judge(JudgeEnum.perfect, laneEnum, noteNum);
		if (songTime >= notePerfectJudgeTime) TapJudgeCheck(laneEnum, noteNum);
	}
	/////////////////////////////////////////////

	//タッチ関連 関数////////////////////////////
	void TouchCheck()
	{
		for (int touchNum = 0; touchNum < Input.touchCount; touchNum++)
		{
			Touch touch = Input.GetTouch(touchNum);
			Vector3 touchPos = touch.position;
			int fingerId = touch.fingerId;
			TouchPhase touchPhase = touch.phase;
			if (touchPhase == TouchPhase.Began) TouchDown(touchPos, fingerId);
			for (LaneEnum laneEnum = LaneEnum.right; laneEnum != LaneEnum.none; laneEnum++)
			{
				int laneInt = (int)laneEnum;
				if (fingerId == touchHoldFingerId[laneInt])
				{
					if (touchPhase == TouchPhase.Moved || touchPhase == TouchPhase.Stationary) TouchHold(laneEnum, touchPos);
					if (touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled) TouchUp(laneEnum);
				}
			}
		}
	}
	void TouchDown(Vector3 touchPos, int fingerID)
	{
		if (touchPos.y < halfScreenPos.y)
		{
			LaneEnum laneEnum = touchPos.x >= halfScreenPos.x ? LaneEnum.right : LaneEnum.left;
			int laneInt = (int)laneEnum;
			TouchHold(laneEnum, touchPos);
			lift[laneInt].gameObject.SetActive(true);
			liftLine[laneInt].gameObject.SetActive(true);
			touchHold[laneInt] = true;
			touchHoldFingerId[laneInt] = fingerID;
			if (noteInstDatas[laneInt].Count > 0) TapJudgeCheck(laneEnum, 0);
			Debug.Log("TouchDown");
		}
	}
	void TouchHold(LaneEnum laneEnum, Vector3 touchPos)
	{
		int laneInt = (int)laneEnum;
		touchPos.z -= cam.transform.position.z;
		Vector3 touchWorldPos = cam.ScreenToWorldPoint(touchPos);
		float liftPosX = touchWorldPos.x;
		liftPosX = liftPosX >= liftPosRest[laneInt].x ? liftPosX : liftPosRest[laneInt].x;
		liftPosX = liftPosX < liftPosRest[laneInt].y ? liftPosX : liftPosRest[laneInt].y;
		lift[laneInt].transform.position = transform.right * liftPosX;
		liftLine[laneInt].transform.position = transform.right * liftPosX + liftLineBasicPos;
	}
	void TouchUp(LaneEnum laneEnum)
	{
		int laneInt = (int)laneEnum;
		lift[laneInt].gameObject.SetActive(false);
		liftLine[laneInt].gameObject.SetActive(false);
		touchHold[laneInt] = false;
		touchHoldFingerId[laneInt] = -1;
		Debug.Log("TouchUp");
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
			case JudgeEnum.miss:
				//灰色で"みす"
				judgeText.text = "<color=#7f7f7f>みす</color>";
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
		scoreText.text = score.ToString("F0");
	}
	void HealthTextUpdate()
	{
		healthText.text = nowHealth.ToString("F0");
	}
	void HealthSliderImageUpdate()
	{
		healthSlider.value = nowHealth;
	}
	bool TextFadeOut(CanvasGroup canvasGroup)
	{
		canvasGroup.alpha -= textFadeSpeed * Time.deltaTime;
		if (canvasGroup.alpha == 0) return false;
		return true;
	}
	/////////////////////////////////////////////

	//ソート関連 関数////////////////////////////
	int NoteSpawnTimeCompare(NoteData a, NoteData b)
	{
		if (a.noteSpawnTime > b.noteSpawnTime) return 1;
		if (a.noteSpawnTime < b.noteSpawnTime) return -1;
		return 0;
	}
	int NotePerfectJudgeTimeCompare(NoteData a, NoteData b)
	{
		if (a.notePerfectJudgeTime > b.notePerfectJudgeTime) return 1;
		if (a.notePerfectJudgeTime < b.notePerfectJudgeTime) return -1;
		return 0;
	}
	/////////////////////////////////////////////

	void GameClear()
	{
		SceneLoad(3);
	}
	void GameOver()
	{
		SceneLoad(4);
	}
	public void SceneLoad(int sceneNum)
	{
		SceneManager.LoadScene(sceneNum);
	}
}
