using UnityEngine;
using UnityEngine.UI;
public class HealthText : MonoBehaviour
{
	static Text healthText;
	void Start()
	{
		healthText = GetComponent<Text>();
	}
	public static void HealthTextDisplay(int health)
	{
		healthText.text = health.ToString();
	}
}
