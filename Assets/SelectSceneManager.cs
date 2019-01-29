using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SelectSceneManager : MonoBehaviour
{
	//曲情報関連 宣言////////////////////////////
	[SerializeField] Text[] songNameText;
	[SerializeField] Text selectSongNameText;
	[SerializeField] Text selectSongArtistText;
	[SerializeField] Text selectSongDiffText;
	[SerializeField] Text selectSongBpmText;
	public static List<MusicData> musicDatas;
	public static SongFileEnum selectSongFileEnum;
	public static int selectSongFileInt;
	public static string selectSongFileString;
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
		//設定仮代入////
		noteBasicSpeed = 24.0f;
		noteColor = new Color[2]
		{
			new Color(0.0f, 1.0f, 1.0f, 1.0f),
			new Color(1.0f, 0.0f, 1.0f, 1.0f)
		};
		liftColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
		liftLineColor = new Color(1.0f, 1.0f, 0.0f, 31 / 255f);
		judgeGapTime = 0.050f;
		//////////////
		musicDatas = new List<MusicData>();
		MusicDataRead();
		songNameText[0].text = musicDatas[0].title;
		songNameText[1].text = musicDatas[1].title;
		SelectSongFile(0);
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
	void MusicDataRead()
	{
		for (SongFileEnum songFileEnum = 0; songFileEnum != SongFileEnum.none; songFileEnum++)
		{
			string songFileString = songFileEnum.ToString();
			string[] musicDataRead = ((TextAsset)Resources.Load("SongFile/" + songFileString + "/MusicData")).text.Split('\n');
			int musicDataReadLine = 0;
			musicDatas.Add(new MusicData()
			{
				title = musicDataRead[musicDataReadLine++],
				artist = musicDataRead[musicDataReadLine++],
				basicBpm = float.Parse(musicDataRead[musicDataReadLine++]),
				offset = float.Parse(musicDataRead[musicDataReadLine++]),
				difficulty = float.Parse(musicDataRead[musicDataReadLine++]),
				damage = float.Parse(musicDataRead[musicDataReadLine++]),
				heel = float.Parse(musicDataRead[musicDataReadLine])
			});
		}
	}
	void SelectSongTextUpdate()
	{
		selectSongNameText.text = "曲名：" + musicDatas[selectSongFileInt].title;
		selectSongArtistText.text = "アーティスト：" + musicDatas[selectSongFileInt].artist;
		selectSongDiffText.text = "難易度：" + musicDatas[selectSongFileInt].difficulty.ToString();
		selectSongBpmText.text = "BPM：" + musicDatas[selectSongFileInt].basicBpm.ToString();
	}
	public void SelectSongFile(int selectSongFileInt)
	{
		selectSongFileEnum = (SongFileEnum)selectSongFileInt;
		SelectSceneManager.selectSongFileInt = selectSongFileInt;
		selectSongFileString = selectSongFileEnum.ToString();
		SelectSongTextUpdate();
	}
	public void SceneLoad(int sceneNum)
	{
		SceneManager.LoadScene(sceneNum);
	}
}
