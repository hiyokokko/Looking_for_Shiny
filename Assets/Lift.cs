using UnityEngine;
public class Lift : MonoBehaviour
{
	[SerializeField] Camera cam;
	Vector2 thisPos;
	float size;
	bool thisNowMove = false;
	Vector2 beforeWorldMousePos;
	void Start ()
	{
		thisPos = transform.position;
		size = transform.localScale.x;
	}
	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 nowScreenMousePos = Input.mousePosition;
			nowScreenMousePos.z = cam.transform.position.z;
			Vector2 nowWorldMousePos = cam.ScreenToWorldPoint(nowScreenMousePos);
			Debug.Log(nowWorldMousePos);
			if (nowWorldMousePos.x >= thisPos.x - size / 2 &&
				nowWorldMousePos.x <= thisPos.x + size / 2 &&
				nowWorldMousePos.y >= thisPos.y - size / 2 &&
				nowWorldMousePos.y <= thisPos.y + size / 2)
			{
				thisNowMove = true;
				beforeWorldMousePos = nowWorldMousePos;
			}
		}
		if (thisNowMove)
		{
			Move();
			if (Input.GetMouseButtonUp(0))
			{
				thisNowMove = false;
			}
		}
	}
	void Move()
	{
		Vector3 nowScreenMousePos = Input.mousePosition;
		nowScreenMousePos.z = cam.transform.position.z;
		Vector2 nowWorldMousePos = cam.ScreenToWorldPoint(nowScreenMousePos);
		transform.position += transform.right * (nowWorldMousePos.x - beforeWorldMousePos.x);
		thisPos = transform.position;
		beforeWorldMousePos = nowWorldMousePos;
	}
}
