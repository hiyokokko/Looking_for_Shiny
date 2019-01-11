using UnityEngine;
using UnityEngine.UI;
public class Note : MonoBehaviour
{
	[SerializeField] ParticleSystem noteBreakParticle;
	public Lane lane;
	public float speed;
	Judge judge = Judge.PERFECT;
	void FixedUpdate ()
	{
		transform.position -= transform.up * Time.deltaTime * speed;
		JudgeCheck();
		
	}
	void JudgeCheck()
	{
		switch (judge)
		{
			case Judge.PERFECT:
				if (transform.position.y <= -3.5f)
				{
					float liftPos = MainManager.lift[(int)lane].transform.position.x;
					if (transform.position.x - (transform.localScale.x + MainManager.lift[(int)lane].transform.localScale.x / 2) < liftPos && liftPos < transform.position.x + (transform.localScale.x + MainManager.lift[(int)lane].transform.localScale.x / 2))
					{
						MainManager.JudgeResult(judge, transform.position);
						Instantiate(noteBreakParticle, transform.position, Quaternion.identity);
						Destroy(gameObject);
					}
					else
					{
						judge++;
					}
					GameObject.Find("ShinySource").GetComponent<AudioSource>().Play();
				}
				break;
			case Judge.GREAT:
				if (transform.position.y >= -4.1f)
				{
					float liftPos = MainManager.lift[(int)lane].transform.position.x;
					if (transform.position.x - (transform.localScale.x + MainManager.lift[(int)lane].transform.localScale.x / 2) < liftPos && liftPos < transform.position.x + (transform.localScale.x + MainManager.lift[(int)lane].transform.localScale.x / 2))
					{
						MainManager.JudgeResult(judge, transform.position);
						Instantiate(noteBreakParticle, transform.position, Quaternion.identity);
						Destroy(gameObject);
					}
				}
				else
				{
					judge++;
				}
				break;
			case Judge.MISS:
				if (transform.position.y < -6.0f)
				{
					MainManager.JudgeResult(judge, transform.position);
					Destroy(gameObject);
				}
				break;
		}
	}
}
