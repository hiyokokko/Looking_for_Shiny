using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {
	[SerializeField] GameObject prefab;
	float wait = 1.0f;
	float time;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time >= wait)
		{
			Instantiate(prefab, new Vector2(Random.Range(-8.0f, 8.0f) , 8.0f), Quaternion.identity);
			time -= wait;
		}
	}
}
