using UnityEngine;
using System.Collections;

public class InputRotation : MonoBehaviour {
	public float mouseSensitivity = 10000.0f;
	public float clampAngle = 80.0f;

	private float rotY = 0.0f;
	private float rotX = 0.0f;

	private bool dragging = false;
	void Update () {
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		if (Input.touchCount != 0)
		{
			Touch touch = Input.GetTouch(0);
			rotY -= touch.deltaPosition.x * 15 * Time.deltaTime;
			rotX += touch.deltaPosition.y * 15 * Time.deltaTime;
		}
		else {
			rotY += mouseX * 360 * Time.deltaTime;
			rotX += mouseY * 360 * Time.deltaTime;
		}
		rotX = Mathf.Clamp(rotX, -90, 90);
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		Quaternion localRotation = Quaternion.Euler (rotX, rotY, 0.0f);
		transform.rotation = localRotation;
	}
}