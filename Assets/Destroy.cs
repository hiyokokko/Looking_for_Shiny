using UnityEngine;
using UnityEngine.UI;

public class Destroy : MonoBehaviour
{
	public float speed;
	bool auto = false;
	void Update ()
	{
		transform.position -= transform.up * Time.deltaTime * speed;
		if (auto & transform.position.y <= -3.5f)
		{
			GameObject.Find("ShinySource").GetComponent<AudioSource>().Play();
			Destroy(gameObject);
		}
	}
	void OnCollisionEnter(Collision col)
	{
		GameObject.Find("ShinySource").GetComponent<AudioSource>().Play();
		Color tempColor = GameObject.Find("JudgeText").GetComponent<Text>().color;
		tempColor.a = 1;
		GameObject.Find("JudgeText").GetComponent<Text>().color = tempColor;
		GameObject.Find("MainManager").GetComponent<MainManager>().Particle(transform.position.x);
		Destroy(gameObject);
	}
}
