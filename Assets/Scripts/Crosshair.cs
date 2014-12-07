using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
	void Update()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0.0f;
		transform.position = mousePos;
	}
}