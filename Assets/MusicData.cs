using System.Collections.Generic;
using UnityEngine;
public class MusicData
{
	public string title;
	public string artist;
	public float bpm;
	public float offset;
	public float speed;
	public Color color;
	public int measure = 0;
	//0:右側のノーツ情報リスト、1:左側のノーツ情報リスト。
	public List<NoteData>[] noteDataList = new List<NoteData>[2]
	{
		new List<NoteData>(),
		new List<NoteData>()
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
