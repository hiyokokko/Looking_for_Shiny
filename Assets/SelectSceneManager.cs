using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectSceneManager : MonoBehaviour
{
	//曲情報関連 宣言////////////////////////////
	public static SongFileEnum songFileEnum;
	public static string songFileString;
	public static MusicData musicData;
	/////////////////////////////////////////////

	//設定関連 宣言//////////////////////////////
	public static float noteBasicSpeed;
	public static Color[] noteColor;
	public static Color liftColor;
	public static Color liftLineColor;
	public static float judgeGapTime;
	/////////////////////////////////////////////
	void Awake()
	{
		//SelectScene作るまでの仮代入。既存シーンで読み込んで。
		songFileEnum = SongFileEnum.Presenter;
		songFileString = songFileEnum.ToString();
		MusicDataRead(songFileEnum.ToString());
		noteBasicSpeed = 32.0f;
		noteColor = new Color[2]
		{
			new Color(1.0f, 0.25f, 1.0f, 1.0f),
			new Color(1.0f, 0.0f, 1.0f, 1.0f)
		};
		liftColor = new Color(1.0f, 127 / 255f, 0.0f, 1.0f);
		liftLineColor = new Color(1.0f, 127 / 255f, 0.0f, 15 / 255f);
		judgeGapTime = 0.050f;
	}
	void Start ()
	{
		
	}
	void Update ()
	{
		
	}
	void FixedUpdate()
	{

	}
	void MusicDataRead(string songFileString)
	{
		string[] musicDataRead = ((TextAsset)Resources.Load(songFileString + "/MusicData")).text.Split('\n');
		int musicDataReadLine = 0;
		musicData = new MusicData()
		{
			title = musicDataRead[musicDataReadLine++],
			artist = musicDataRead[musicDataReadLine++],
			basicBpm = float.Parse(musicDataRead[musicDataReadLine++]),
			offset = float.Parse(musicDataRead[musicDataReadLine++]),
			difficulty = float.Parse(musicDataRead[musicDataReadLine++]),
			damage = float.Parse(musicDataRead[musicDataReadLine++]),
			heel = float.Parse(musicDataRead[musicDataReadLine])
		};
	}
}
