public class NoteData
{
	public float time;
	public float pos;
	public NoteData(float time, string shinyDataRead)
	{
		this.time = time;
		pos = float.Parse(shinyDataRead);
	}
}
