using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainManager : MonoBehaviour
{
	[SerializeField] ParticleSystem particle;

	[SerializeField] TextAsset musicDataText;
	[SerializeField] AudioSource musicSource;
	[SerializeField] GameObject shinyPrefab;
	MusicData musicData;
	Vector3[] spawnPos = new Vector3[2]
	{
		new Vector3(1.0f, 6.5f, 0.0f),
		new Vector3(-7.0f, 6.5f, 0.0f)
	};
	float shinySpawnTimer = 0.0f;
	int[] shinySpawnCount = new int[2];
	float waitTime = 5.0f;
	bool[] spawn = new bool[2]{
		true,true };
	void Start ()
	{
		MusicDataRead();
		StartCoroutine("Wait");
	}
	void FixedUpdate ()
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
			for (int shinyDataListNum = 0; shinyDataListNum < 2; shinyDataListNum++)
			{
				string[] noteDataRead = musicDataRead[musicDataReadLine++].Split(' ');
				int noteDataReadLine = 0;
				int noteDataReadLineMax = noteDataRead.Length;
				while (noteDataReadLine < noteDataReadLineMax)
				{
					if (noteDataRead[noteDataReadLine][0] != '*')
					{
						float time = 60.0f / musicData.bpm * (musicData.measure * 4.0f + noteDataReadLine * (4.0f / noteDataReadLineMax)) - 10.0f / musicData.speed;
						ShinyData shinyData = new ShinyData(time, noteDataRead[noteDataReadLine]);
						Debug.Log(shinyData.time);
						musicData.shinyDataList[shinyDataListNum].Add(shinyData);
					}
					noteDataReadLine++;
				}
			}
			musicData.measure++;
		}
	}
	void ShinySpawn()
	{
		for (int shinyDataListNum = 0; shinyDataListNum < 2; shinyDataListNum++)
		{
			if (spawn[shinyDataListNum] && musicData.shinyDataList[shinyDataListNum][shinySpawnCount[shinyDataListNum]].time + musicData.offset + waitTime <= shinySpawnTimer)
			{
				GameObject shinyInst = Instantiate(shinyPrefab, spawnPos[shinyDataListNum] + transform.right * musicData.shinyDataList[shinyDataListNum][shinySpawnCount[shinyDataListNum]].pos, Quaternion.identity);
				shinyInst.GetComponent<Destroy>().speed = musicData.speed;
				shinyInst.GetComponent<Renderer>().material.color = musicData.color;
				shinyInst.GetComponent<Renderer>().material.SetColor("_EmissionColor", musicData.color);
				ParticleSystem.MainModule shinyParticle = shinyInst.GetComponentInChildren<ParticleSystem>().main;
				shinyParticle.startColor = musicData.color;
				shinySpawnCount[shinyDataListNum]++;
				if (shinySpawnCount[shinyDataListNum] == musicData.shinyDataList[shinyDataListNum].Count)
				{
					spawn[shinyDataListNum] = false;
				}
			}
		}
	}
	public void Particle(float pos)
	{
		Instantiate(particle, new Vector2(pos, -3.8f), Quaternion.identity);
	}
}
public class MusicData
{
	public string title;
	public string artist;
	public float bpm;
	public float offset;
	public float speed;
	public Color color;
	public int measure = 0;
	//0:右側のシャイニー情報リスト、1:左側のシャイニー情報リスト。
	public List<ShinyData>[] shinyDataList = new List<ShinyData>[2]
	{
		new List<ShinyData>(),
		new List<ShinyData>()
	};
	public MusicData(string[] musicDataRead, ref int musicDataReadLine)
	{
		title = musicDataRead[musicDataReadLine++];
		artist = musicDataRead[musicDataReadLine++];
		bpm = float.Parse(musicDataRead[musicDataReadLine++]);
		offset = float.Parse(musicDataRead[musicDataReadLine++]);
		speed = float.Parse(musicDataRead[musicDataReadLine++]);
		string[] colorData = musicDataRead[musicDataReadLine++].Split('/');
		color = new Color(float.Parse(colorData[0]), float.Parse(colorData[1]), float.Parse(colorData[2]), 1.0f);
	}
}
public class ShinyData
{
	public float time;
	public float pos;
	public ShinyData(float time, string shinyDataRead)
	{
		this.time = time;
		pos = float.Parse(shinyDataRead);
	}
}
