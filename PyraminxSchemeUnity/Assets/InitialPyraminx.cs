using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class InitialPyraminx : MonoBehaviour {

	public GameObject pyramidFace;

	private Vector3 rotationAxis;
	private PyramidFace frontFace;
	private PyramidFace leftFace;
	private PyramidFace rightFace;
	private PyramidFace bottomFace;

	float faceOffset;

	private int rotationTime = 30;
	private int currRotTime = -1;
	private float rotationAmount;
	private float rotationDirection = 1;

	public Vector3 offset;

	void Start () {
		rotationAmount = 120 / (float)rotationTime;
		rotationAxis = new Vector3 (0, 1, 0);
		CreateFaces ();
	}
		
	float pyramidAngle = 70.528779f;
	private void CreateFaces() {
		faceOffset = Mathf.Tan (60 * Mathf.PI / 180) * 0.5f / 3.0f;
		float heightOffset = Mathf.Sqrt (6) / 12.0f;
		//float heightOffset = 1 / Mathf.Sqrt(2) / 2;
		// Create 'front' face.
		GameObject newFace = (GameObject)Instantiate(pyramidFace, new Vector3(0,0,0), Quaternion.identity);

		newFace.transform.Rotate (new Vector3 (1, 0, 0), -pyramidAngle);
		newFace.transform.position = new Vector3 (0, -heightOffset, -faceOffset);
		newFace.transform.parent = this.transform;

		frontFace = newFace.GetComponent<PyramidFace> ();
		frontFace.Initialise (4);

		// Create 'right' face.
		newFace = (GameObject)Instantiate(pyramidFace, new Vector3(0,0,0), Quaternion.identity);
		newFace.transform.Rotate (new Vector3 (0, 1, 0), -120);
		newFace.transform.Rotate (new Vector3 (1, 0, 0), -pyramidAngle);
		newFace.transform.position = new Vector3 (faceOffset*Mathf.Cos((30.0f)/180.0f*Mathf.PI), -heightOffset, faceOffset*Mathf.Sin((30.0f)/180.0f*Mathf.PI));
		newFace.transform.parent = this.transform;

		rightFace = newFace.GetComponent<PyramidFace> ();
		rightFace.Initialise (4);

		// Create 'left' face.
		newFace = (GameObject)Instantiate(pyramidFace, new Vector3(0,0,0), Quaternion.identity);
		newFace.transform.Rotate (new Vector3 (0, 1, 0), 120);
		newFace.transform.Rotate (new Vector3 (1, 0, 0), -pyramidAngle);
		newFace.transform.position = new Vector3 (-faceOffset*Mathf.Cos((30.0f)/180.0f*Mathf.PI), -heightOffset, faceOffset*Mathf.Sin((30.0f)/180.0f*Mathf.PI));
		newFace.transform.parent = this.transform;

		leftFace = newFace.GetComponent<PyramidFace> ();
		leftFace.Initialise (4);

		// Create 'bottom' face.
		newFace = (GameObject)Instantiate(pyramidFace, new Vector3(0,0,0), Quaternion.identity);
		newFace.transform.Rotate (new Vector3 (0, 1, 0), 180);
		newFace.transform.Rotate (new Vector3 (1, 0, 0), 180);
		newFace.transform.position = new Vector3 (0, -heightOffset, -faceOffset);
		newFace.transform.parent = this.transform;

		bottomFace = newFace.GetComponent<PyramidFace> ();
		bottomFace.Initialise (4);
	}

	public int[,] GetFrontFace() {
		return frontFace.GetFaceColors ();
	}

	public int[,] GetLeftFace() {
		return leftFace.GetFaceColors ();
	}

	public int[,] GetRightFace() {
		return rightFace.GetFaceColors ();
	}

	public int[,] GetBottomFace() {
		return bottomFace.GetFaceColors ();
	}

	public void SetFrontFace(int[,] faceColors) {
		frontFace.SetFaceColors (faceColors);
	}

	public void SetLeftFace(int[,] faceColors) {
		leftFace.SetFaceColors (faceColors);
	}

	public void SetRightFace(int[,] faceColors) {
		rightFace.SetFaceColors (faceColors);
	}

	public void SetBottomFace(int[,] faceColors) {
		bottomFace.SetFaceColors (faceColors);
	}

	int currRotation = 0;
	public void NextRotation() {
		currRotTime = 0;
		currRotation += 1;
	}

	void Update () {

		if (currRotTime < rotationTime && currRotTime != -1) {
			currRotTime += 1;
			if (currRotation < 3) {
				frontFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), rotationDirection * rotationAmount);
				leftFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), rotationDirection * rotationAmount);
				rightFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), rotationDirection * rotationAmount);
				bottomFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), rotationDirection * rotationAmount);
			} else if (currRotation == 3) {
				rotationAxis = new Vector3 (1, 0, 0);
				frontFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), rotationDirection * rotationAmount);
				leftFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), rotationDirection * rotationAmount);
				rightFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), rotationDirection * rotationAmount);
				bottomFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), rotationDirection * rotationAmount);

			} else {
				rotationAxis = new Vector3 (0, -Mathf.Cos(pyramidAngle/180.0f*Mathf.PI), Mathf.Sin(pyramidAngle/180.0f*Mathf.PI));

				frontFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), -rotationDirection * rotationAmount*0.48f);
				leftFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), -rotationDirection * rotationAmount*0.48f);
				rightFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), -rotationDirection * rotationAmount*0.48f);
				bottomFace.transform.RotateAround (offset, transform.rotation * (rotationAxis), -rotationDirection * rotationAmount*0.48f);
			}
		} else if (currRotTime != -1) {
			currRotTime = -1;
			if (currRotation == 3) {
				currRotation = 4;
				currRotTime = 0;
			}
		}
		//transform.Rotate (new Vector3 (0, 1, 1), 1);
	}

	void OnDestroy() {
		
		Destroy (frontFace);
		Destroy (leftFace);
		Destroy (rightFace);
		Destroy (bottomFace);
		Destroy (gameObject);
	}
}
