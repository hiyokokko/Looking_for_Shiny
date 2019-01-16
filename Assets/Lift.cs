using UnityEngine;
public class Lift : MonoBehaviour
{
	Camera cam;
	[SerializeField] Lane lane;
	Vector3 thisPos;
	float size;
	bool thisNowMove = false;
	Vector3 beforeWorldMousePos;
	Vector2[] liftMoveRest = new Vector2[2]
	{
		new Vector2(1.0f, 7.0f),
		new Vector2(-7.0f, -1.0f)
	};
	void Start ()
	{
		cam = GameObject.Find("Camera").GetComponent<Camera>();
		thisPos = transform.position;
		size = transform.localScale.x;
	}
	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 nowScreenMousePos = Input.mousePosition;
			nowScreenMousePos.z = cam.transform.position.z;
			Vector3 nowWorldMousePos = -cam.ScreenToWorldPoint(nowScreenMousePos);
			if (nowWorldMousePos.x >= thisPos.x - size / 2 &&
				nowWorldMousePos.x <= thisPos.x + size / 2 &&
				nowWorldMousePos.y >= -9.0f &&
				nowWorldMousePos.y <= -6.0f)
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
		Vector3 nowWorldMousePos = -cam.ScreenToWorldPoint(nowScreenMousePos);
		thisPos.x += nowWorldMousePos.x - beforeWorldMousePos.x;
		if (thisPos.x < liftMoveRest[(int)lane].x)
		{
			thisPos.x = liftMoveRest[(int)lane].x;
		}
		else if (thisPos.x > liftMoveRest[(int)lane].y)
		{
			thisPos.x = liftMoveRest[(int)lane].y;
		}
		transform.position = thisPos;
		beforeWorldMousePos = nowWorldMousePos;
	}
}
