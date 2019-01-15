using UnityEngine;
public class Note : MonoBehaviour
{
	[SerializeField] ParticleSystem noteBreakParticle;
	public Lane lane;
	public float speed;
	Judge judge = Judge.SHINY;
	void FixedUpdate ()
	{
		transform.position -= (transform.up + transform.forward) * Time.deltaTime * speed;
		JudgeCheck();
		
	}
	void JudgeCheck()
	{
		switch (judge)
		{
			case Judge.SHINY:
				if (transform.position.y <= 0.0f)
				{
					float liftPos = MainManager.lift[(int)lane].transform.position.x;
					if (transform.position.x - (transform.localScale.x + MainManager.lift[(int)lane].transform.localScale.x / 2) < liftPos && liftPos < transform.position.x + (transform.localScale.x + MainManager.lift[(int)lane].transform.localScale.x / 2))
					{
						MainManager.JudgeResult(judge, transform.position);
						GameObject.Find("ShinySourceRight").GetComponent<AudioSource>().Play();
						Instantiate(noteBreakParticle, transform.position, Quaternion.identity);
						Destroy(gameObject);
					}
					else
					{
						judge++;
					}
				}
				break;
			case Judge.MISS:
				if (transform.position.y < -1.0f)
				{
					MainManager.JudgeResult(judge, transform.position);
					Destroy(gameObject);
				}
				break;
		}
	}
}
