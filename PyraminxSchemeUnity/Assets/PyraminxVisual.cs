using UnityEngine;
using System.Collections.Generic;
using System;

public class PyraminxVisual : MonoBehaviour {

	public GameObject pyramidFace;
	public GameObject cubeSolver;

	private Vector3 rotationAxis;
	private PyramidFace frontFace;
	private PyramidFace leftFace;
	private PyramidFace rightFace;
	private PyramidFace bottomFace;

	private CubeSolver _cubeSolver;

	private List<SplitFace> stationaryFaces;
	private List<SplitFace> rotationFaces;

	float faceOffset;

	public List<SolutionStep> solutionSteps;
	private int currStep = 0;

	private int rotationTime = 30;
	private int currRotTime = -1;
	private float rotationAmount;
	private float rotationDirection = 1;

	private List<Action> rotationFunctions = new List<Action>();
	bool instantiated = false;
	void Start () {
		_cubeSolver = cubeSolver.GetComponent<CubeSolver> ();

		Instantiate ();
	}

	public void Instantiate() {
		if (!instantiated) {
			instantiated = true;
			rotationAmount = 120 / (float)rotationTime;
			rotationAxis = new Vector3 (0, 1, 0);
			stationaryFaces = new List<SplitFace> ();
			rotationFaces = new List<SplitFace> ();

			rotationFunctions.Add (RotateR1);
			rotationFunctions.Add (RotateR2);
			rotationFunctions.Add (RotateR3);

			rotationFunctions.Add (RotateL1);
			rotationFunctions.Add (RotateL2);
			rotationFunctions.Add (RotateL3);

			rotationFunctions.Add (RotateU1);
			rotationFunctions.Add (RotateU2);
			rotationFunctions.Add (RotateU3);

			rotationFunctions.Add (RotateB1);
			rotationFunctions.Add (RotateB2);
			rotationFunctions.Add (RotateB3);

			CreateFaces ();
		}
	}

	public void SetSolutionSteps(List<SolutionStep> solutionSteps) {
		this.solutionSteps = solutionSteps;
		currStep = 0;
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

	private Pyraminx p;
	public void SetUpPyraminx() {
		p = new Pyraminx ();
		p.SetFrontColors (frontFace.GetFaceColors ());
		p.SetLeftColors (leftFace.GetFaceColors ());
		p.SetRightColors (rightFace.GetFaceColors ());
		p.SetBottomColors (bottomFace.GetFaceColors ());

		Solver Solution = new Solver (p);
		solutionSteps = Solution.Solve ();
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
		frontFace.Initialise (0);

		// Create 'right' face.
		newFace = (GameObject)Instantiate(pyramidFace, new Vector3(0,0,0), Quaternion.identity);
		newFace.transform.Rotate (new Vector3 (0, 1, 0), -120);
		newFace.transform.Rotate (new Vector3 (1, 0, 0), -pyramidAngle);
		newFace.transform.position = new Vector3 (faceOffset*Mathf.Cos((30.0f)/180.0f*Mathf.PI), -heightOffset, faceOffset*Mathf.Sin((30.0f)/180.0f*Mathf.PI));
		newFace.transform.parent = this.transform;

		rightFace = newFace.GetComponent<PyramidFace> ();
		rightFace.Initialise (1);

		// Create 'left' face.
		newFace = (GameObject)Instantiate(pyramidFace, new Vector3(0,0,0), Quaternion.identity);
		newFace.transform.Rotate (new Vector3 (0, 1, 0), 120);
		newFace.transform.Rotate (new Vector3 (1, 0, 0), -pyramidAngle);
		newFace.transform.position = new Vector3 (-faceOffset*Mathf.Cos((30.0f)/180.0f*Mathf.PI), -heightOffset, faceOffset*Mathf.Sin((30.0f)/180.0f*Mathf.PI));
		newFace.transform.parent = this.transform;

		leftFace = newFace.GetComponent<PyramidFace> ();
		leftFace.Initialise (2);

		// Create 'bottom' face.
		newFace = (GameObject)Instantiate(pyramidFace, new Vector3(0,0,0), Quaternion.identity);
		newFace.transform.Rotate (new Vector3 (0, 1, 0), 180);
		newFace.transform.Rotate (new Vector3 (1, 0, 0), 180);
		newFace.transform.position = new Vector3 (0, -heightOffset, -faceOffset);
		newFace.transform.parent = this.transform;

		bottomFace = newFace.GetComponent<PyramidFace> ();
		bottomFace.Initialise (3);
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
		
	public void RotateU1() {
		// Split front face.
		SplitFace[] splitFaces = frontFace.SplitTopSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		frontFace.DisableMeshRenderer ();

		// Split left face.
		splitFaces = leftFace.SplitTopSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		leftFace.DisableMeshRenderer ();

		// Split right face.
		splitFaces = rightFace.SplitTopSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		rightFace.DisableMeshRenderer ();

		rotationAxis = new Vector3 (0, 1, 0);
	}

	public void RotateU2() {
		// Split front face.
		SplitFace[] splitFaces = frontFace.SplitTopLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		frontFace.DisableMeshRenderer ();

		// Split left face.
		splitFaces = leftFace.SplitTopLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		leftFace.DisableMeshRenderer ();

		// Split right face.
		splitFaces = rightFace.SplitTopLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		rightFace.DisableMeshRenderer ();

		rotationAxis = new Vector3 (0, 1, 0);
	}

	public void RotateU3() {
		rotationAxis = new Vector3 (0, 1, 0);

		rotationFaces.Add (frontFace.SplitWhole());
		rotationFaces.Add (leftFace.SplitWhole());
		rotationFaces.Add (rightFace.SplitWhole());
		rotationFaces.Add (bottomFace.SplitWhole());

		frontFace.DisableMeshRenderer ();
		leftFace.DisableMeshRenderer ();
		rightFace.DisableMeshRenderer ();
		bottomFace.DisableMeshRenderer ();
	}

	public void RotateL1() {
		// Split front face.
		SplitFace[] splitFaces = frontFace.SplitLeftSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		frontFace.DisableMeshRenderer ();

		// Split bottom face.
		splitFaces = bottomFace.SplitRightSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		bottomFace.DisableMeshRenderer ();

		// Split left face.
		splitFaces = leftFace.SplitRightSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		leftFace.DisableMeshRenderer ();

		rotationAxis = new Vector3 (0, -Mathf.Cos(pyramidAngle/180.0f*Mathf.PI), Mathf.Sin(pyramidAngle/180.0f*Mathf.PI));
		Quaternion rotateAxis = new Quaternion();
		rotateAxis.eulerAngles = new Vector3 (0, -120, 0);
		rotationAxis = rotateAxis * rotationAxis;
	}

	public void RotateL2() {
		// Split front face.
		SplitFace[] splitFaces = frontFace.SplitLeftLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		frontFace.DisableMeshRenderer ();

		// Split bottom face.
		splitFaces = bottomFace.SplitRightLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		bottomFace.DisableMeshRenderer ();

		// Split left face.
		splitFaces = leftFace.SplitRightLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		leftFace.DisableMeshRenderer ();

		rotationAxis = new Vector3 (0, -Mathf.Cos(pyramidAngle/180.0f*Mathf.PI), Mathf.Sin(pyramidAngle/180.0f*Mathf.PI));
		Quaternion rotateAxis = new Quaternion();
		rotateAxis.eulerAngles = new Vector3 (0, -120, 0);
		rotationAxis = rotateAxis * rotationAxis;
	}

	public void RotateL3() {
		rotationAxis = new Vector3 (0, -Mathf.Cos(pyramidAngle/180.0f*Mathf.PI), Mathf.Sin(pyramidAngle/180.0f*Mathf.PI));
		Quaternion rotateAxis = new Quaternion();
		rotateAxis.eulerAngles = new Vector3 (0, -120, 0);
		rotationAxis = rotateAxis * rotationAxis;

		rotationFaces.Add (frontFace.SplitWhole());
		rotationFaces.Add (leftFace.SplitWhole());
		rotationFaces.Add (rightFace.SplitWhole());
		rotationFaces.Add (bottomFace.SplitWhole());

		frontFace.DisableMeshRenderer ();
		leftFace.DisableMeshRenderer ();
		rightFace.DisableMeshRenderer ();
		bottomFace.DisableMeshRenderer ();
	}

	public void RotateR1() {
		// Split front face.
		SplitFace[] splitFaces = frontFace.SplitRightSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		frontFace.DisableMeshRenderer ();

		// Split bottom face.
		splitFaces = bottomFace.SplitLeftSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		bottomFace.DisableMeshRenderer ();

		// Split right face.
		splitFaces = rightFace.SplitLeftSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		rightFace.DisableMeshRenderer ();

		rotationAxis = new Vector3 (0, -Mathf.Cos(pyramidAngle/180.0f*Mathf.PI), Mathf.Sin(pyramidAngle/180.0f*Mathf.PI));
		Quaternion rotateAxis = new Quaternion();
		rotateAxis.eulerAngles = new Vector3 (0, 120, 0);
		rotationAxis = rotateAxis * rotationAxis;
	}

	public void RotateR2() {
		// Split front face.
		SplitFace[] splitFaces = frontFace.SplitRightLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		frontFace.DisableMeshRenderer ();

		// Split bottom face.
		splitFaces = bottomFace.SplitLeftLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		bottomFace.DisableMeshRenderer ();

		// Split right face.
		splitFaces = rightFace.SplitLeftLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		rightFace.DisableMeshRenderer ();

		rotationAxis = new Vector3 (0, -Mathf.Cos(pyramidAngle/180.0f*Mathf.PI), Mathf.Sin(pyramidAngle/180.0f*Mathf.PI));
		Quaternion rotateAxis = new Quaternion();
		rotateAxis.eulerAngles = new Vector3 (0, 120, 0);
		rotationAxis = rotateAxis * rotationAxis;
	}

	public void RotateR3() {
		rotationAxis = new Vector3 (0, -Mathf.Cos(pyramidAngle/180.0f*Mathf.PI), Mathf.Sin(pyramidAngle/180.0f*Mathf.PI));
		Quaternion rotateAxis = new Quaternion();
		rotateAxis.eulerAngles = new Vector3 (0, 120, 0);
		rotationAxis = rotateAxis * rotationAxis;

		rotationFaces.Add (frontFace.SplitWhole());
		rotationFaces.Add (leftFace.SplitWhole());
		rotationFaces.Add (rightFace.SplitWhole());
		rotationFaces.Add (bottomFace.SplitWhole());

		frontFace.DisableMeshRenderer ();
		leftFace.DisableMeshRenderer ();
		rightFace.DisableMeshRenderer ();
		bottomFace.DisableMeshRenderer ();
	}

	public void RotateB1() {
		// Split left face.
		SplitFace[] splitFaces = leftFace.SplitLeftSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		leftFace.DisableMeshRenderer ();

		// Split right face.
		splitFaces = rightFace.SplitRightSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		rightFace.DisableMeshRenderer ();

		// Split bottom face.
		splitFaces = bottomFace.SplitTopSmall();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		bottomFace.DisableMeshRenderer ();

		rotationAxis = new Vector3 (0, -Mathf.Cos(pyramidAngle/180.0f*Mathf.PI), Mathf.Sin(pyramidAngle/180.0f*Mathf.PI));
	}

	public void RotateB2() {
		// Split left face.
		SplitFace[] splitFaces = leftFace.SplitLeftLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		leftFace.DisableMeshRenderer ();

		// Split right face.
		splitFaces = rightFace.SplitRightLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		rightFace.DisableMeshRenderer ();

		// Split bottom face.
		splitFaces = bottomFace.SplitTopLarge();
		rotationFaces.Add (splitFaces [0]);
		stationaryFaces.Add (splitFaces [1]);
		bottomFace.DisableMeshRenderer ();

		rotationAxis = new Vector3 (0, -Mathf.Cos(pyramidAngle/180.0f*Mathf.PI), Mathf.Sin(pyramidAngle/180.0f*Mathf.PI));
	}

	public void RotateB3() {
		rotationAxis = new Vector3 (0, -Mathf.Cos(pyramidAngle/180.0f*Mathf.PI), Mathf.Sin(pyramidAngle/180.0f*Mathf.PI));

		rotationFaces.Add (frontFace.SplitWhole());
		rotationFaces.Add (leftFace.SplitWhole());
		rotationFaces.Add (rightFace.SplitWhole());
		rotationFaces.Add (bottomFace.SplitWhole());

		frontFace.DisableMeshRenderer ();
		leftFace.DisableMeshRenderer ();
		rightFace.DisableMeshRenderer ();
		bottomFace.DisableMeshRenderer ();
	}

	void Update () {
		bool progress = false;
		if (Input.touchCount > 0) {
			if (Input.touches [0].tapCount == 2) {
				progress = true;
			}
		}
		if (Input.GetKeyDown ("a") || progress) {
			if (currRotTime > rotationTime || currRotTime == -1 && currStep < solutionSteps.Count) {
				print (currStep);
				SolutionStep currSolStep = solutionSteps [currStep];
				int stepFunc = (int)currSolStep.moveStep/2;
				if ((int)currSolStep.moveStep % 2 == 0)
					rotationDirection = 1;
				else
					rotationDirection = -1;
				if (stepFunc < rotationFunctions.Count)
					rotationFunctions [stepFunc] ();

				currRotTime = 0;
			}
		}

		if (currRotTime < rotationTime && currRotTime != -1) {
			currRotTime += 1;
			for (int i = 0; i < rotationFaces.Count; i++) {
				rotationFaces [i].transform.RotateAround (new Vector3 (0, 0, 0), transform.rotation * (rotationAxis), rotationDirection * rotationAmount);
			}
		} else if (currRotTime != -1) {
			currRotTime = -1;
			for (int i = 0; i < rotationFaces.Count; i++) {
				Destroy (rotationFaces [i]);
			}
			for (int i = 0; i < stationaryFaces.Count; i++) {
				Destroy (stationaryFaces [i]);
			}
			rotationFaces.Clear ();
			stationaryFaces.Clear ();

			leftFace.EnableMeshRenderer ();
			rightFace.EnableMeshRenderer ();
			frontFace.EnableMeshRenderer ();
			bottomFace.EnableMeshRenderer ();

			SolutionStep currSolStep = solutionSteps [currStep];

			leftFace.SetFaceColors (currSolStep.newLeft);
			rightFace.SetFaceColors (currSolStep.newRight);
			frontFace.SetFaceColors (currSolStep.newFront);
			bottomFace.SetFaceColors (currSolStep.newBottom);

			currStep += 1;

			if (currStep >= solutionSteps.Count) {
				solutionSteps = _cubeSolver.solvedSteps;
				currStep = 0;
			}
		}
		//transform.Rotate (new Vector3 (0, 1, 1), 1);
	}
}
