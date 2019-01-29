using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleSceneManager : MonoBehaviour {
	
	void Start () {
		
	}
	void Update () {
		if (Input.GetMouseButtonUp(0))
		{
			SceneManager.LoadScene(1);
		}
	}
}
