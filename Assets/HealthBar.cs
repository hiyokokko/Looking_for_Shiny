using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	static Image image;
	// Use this for initialization
	void Start ()
	{
		image = GetComponent <Image>();
	}
	public static void HealthBarDisplay(int health)
	{
		image.fillAmount = health / 1000.0f;
	}
}
