using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainManager : MonoBehaviour
{
	[SerializeField] GameObject note;
	[SerializeField] TextAsset musicDataText;
	[SerializeField] AudioSource musicSource;
	[SerializeField] GameObject shinyPrefab;
	float parfectPos = -3.5f;
	public static GameObject[] lift;

	static MusicData musicData;
	Vector3[] spawnPos = new Vector3[2]
	{
		new Vector3(1.0f, 32.0f, 32.0f),
		new Vector3(-7.0f, 32.0f, 32.0f)
	};
	float spawnPosY = 6.5f;
	float shinySpawnTimer = 0.0f;
	int[] shinySpawnCount = new int[2];
	float waitTime = 5.0f;
	static int[] judgeCount = new int[3];
	static int combo = 0;
	static int score = 0;
	static int health = 1000;
	static int healthMax = 1000;
	bool[] spawn = new bool[2]
	{
		true,
		true
	};
	void Start ()
	{
		lift = new GameObject[2]
		{
			GameObject.Find("RightLift"),
			GameObject.Find("LeftLift")
		};
		MusicDataRead();
		StartCoroutine("Wait");
	}
	void Update ()
	{
		shinySpawnTimer += Time.deltaTime;
		ShinySpawn();
	}
	IEnumerator Wait()
	{
		yield return new WaitForSeconds(waitTime);
		musicSource.Play();
	}
	void MusicDataRead()
	{
		string[] musicDataRead = musicDataText.text.Split(char.Parse("\n"));
		int musicDataReadLine = 0;
		int musicDataReadLineMax = musicDataRead.Length;
		musicData = new MusicData(musicDataRead, ref musicDataReadLine);
		while (musicDataReadLine < musicDataReadLineMax)
		{
			musicDataReadLine++;
			//0:右側のノーツ情報読み込み、1:左側のノーツで情報読み込み、2:ループ終了。
			for (Lane lane = Lane.RightLane; lane != Lane.NONE; lane++)
			{
				string[] noteDataRead = musicDataRead[musicDataReadLine++].Split(' ');
				int noteDataReadLine = 0;
				int noteDataReadLineMax = noteDataRead.Length;
				while (noteDataReadLine < noteDataReadLineMax)
				{
					if (noteDataRead[noteDataReadLine][0] != '*')
					{
						float time = 60.0f / musicData.bpm * (musicData.measure * 4.0f + noteDataReadLine * (4.0f / noteDataReadLineMax)) - (32.0f - 0.25f) / musicData.speed + waitTime + musicData.offset;
						NoteData shinyData = new NoteData(time, noteDataRead[noteDataReadLine]);
						musicData.noteDataList[(int)lane].Add(shinyData);
					}
					noteDataReadLine++;
				}
			}
			musicData.measure++;
		}
	}
	void ShinySpawn()
	{
		for (Lane lane = Lane.RightLane; lane != Lane.NONE; lane++)
		{
			if (spawn[(int)lane] && musicData.noteDataList[(int)lane][shinySpawnCount[(int)lane]].time <= shinySpawnTimer)
			{
				float gapPos = (musicData.noteDataList[(int)lane][shinySpawnCount[(int)lane]].time - shinySpawnTimer) * musicData.speed;
				Vector3 notePos = spawnPos[(int)lane] + new Vector3(musicData.noteDataList[(int)lane][shinySpawnCount[(int)lane]].pos, gapPos, gapPos);
				GameObject noteInst = Instantiate(note, notePos, Quaternion.identity);
				noteInst.GetComponent<Note>().speed = musicData.speed;
				noteInst.GetComponent<Note>().lane = lane;
				noteInst.GetComponent<Renderer>().material.color = musicData.color;
				noteInst.GetComponent<Renderer>().material.SetColor("_EmissionColor", musicData.color);
				shinySpawnCount[(int)lane]++;
				if (shinySpawnCount[(int)lane] == musicData.noteDataList[(int)lane].Count)
				{
					spawn[(int)lane] = false;
				}
			}
		}
	}
	public static void JudgeResult(Judge judge, Vector3 notePos)
	{
		judgeCount[(int)judge]++;
		if (judge == Judge.SHINY)
		{
			combo++;
			ComboText.ComboTextDisplay(combo);
			score += (int)(combo * float.Parse(Mathf.Sqrt(combo).ToString("F1")));
			ScoreText.ScoreTextDisplay(score);
			health += musicData.heel;
			if (health > healthMax)
			{
				health = healthMax;
			}
		}
		else
		{
			combo = 0;
			health -= musicData.damage;
			if (health <= 0)
			{
				health = 0;
			}
		}
		JudgeText.JudgeTextDisplay(judge);
		HealthText.HealthTextDisplay(health);
		HealthBar.HealthBarDisplay(health);
	}
}
public enum Judge
{
	SHINY = 0,
	MISS = 1,
	NONE = 2
}
public enum Lane
{
	RightLane = 0,
	LeftLane = 1,
	NONE = 2
}
