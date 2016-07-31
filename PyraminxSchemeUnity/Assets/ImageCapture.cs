using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class ImageCapture : MonoBehaviour {
	[DllImport("PyraminxSchemeCV.Windows")]
	private static extern void StartCapture();

	[DllImport("PyraminxSchemeCV.Windows")]
	private static extern void EndCapture();

	[DllImport("PyraminxSchemeCV.Windows")]
	private static extern void extResetColourCounts();

	[DllImport("PyraminxSchemeCV.Windows")]
	private static extern bool GetColours(bool mirrored, int expectedOrientation, int[] colours);

	public GameObject initialPyraminxPrefab;
	private InitialPyraminx initialPyraminx, swappedPyraminx;
	private int currFace = 0;

	public GameObject pyraminxVisualPrefab;

	void Start () {
		GameObject pyrmGameObject = (GameObject)Instantiate (initialPyraminxPrefab, Vector3.zero, Quaternion.identity);

		initialPyraminx = pyrmGameObject.GetComponent<InitialPyraminx> ();

		//pyrmGameObject = (GameObject)Instantiate (initialPyraminxPrefab, Vector3.zero, Quaternion.identity);
		//swappedPyraminx = pyrmGameObject.GetComponent<InitialPyraminx> ();
		//
		extResetColourCounts();
		StartCapture();
	}
	bool initial = true;
	void Update () {
		if (initial) {
			initialPyraminx.transform.position = new Vector3 (0, 0, 0);
			initialPyraminx.offset = new Vector3 (0, 0, 0);

			//swappedPyraminx.transform.position = new Vector3 (0.5f, 0, 0);
			//swappedPyraminx.offset = new Vector3 (0.5f, 0, 0);
		}

		bool progress = false;
		if (Input.touchCount > 0) {
			if (Input.touches [0].tapCount == 2) {
				progress = true;
			}
		}
		if (Input.GetKeyDown ("a") || progress) {
			if (currFace == 3) {
				// Create the actual pyraminx.
				GameObject newPyrm = (GameObject)Instantiate(pyraminxVisualPrefab, Vector3.zero, Quaternion.identity);
				PyraminxVisual pyrm = newPyrm.GetComponent<PyraminxVisual> ();
				pyrm.Instantiate ();
				pyrm.SetFrontFace (initialPyraminx.GetFrontFace ());
				pyrm.SetLeftFace (initialPyraminx.GetLeftFace ());
				pyrm.SetRightFace (initialPyraminx.GetRightFace ());
				pyrm.SetBottomFace (initialPyraminx.GetBottomFace ());
				pyrm.SetUpPyraminx ();

				Destroy (this);
			}
			currFace += 1;
			initialPyraminx.NextRotation ();
			extResetColourCounts();
		}

		int[] col = new int[9];
		bool result = GetColours(false, 0, col);

		if (result) {
			// Create face array.
			int[,] faceArray = new int[,] {{-1, -1, col[0], -1, -1}, {-1, col[1], col[2], col[3], -1}, {col[4], col[5], col[6], col[7], col[8]}};
			if (currFace == 0) {
				initialPyraminx.SetFrontFace (faceArray);
				//swappedPyraminx.SetFrontFace (faceArray);
			}
			if (currFace == 1) {
				initialPyraminx.SetRightFace (faceArray);
				//swappedPyraminx.SetRightFace (faceArray);
			}
			if (currFace == 2) {
				initialPyraminx.SetLeftFace (faceArray);
				//swappedPyraminx.SetLeftFace (faceArray);
			}
			if (currFace == 3) {
				initialPyraminx.SetBottomFace (faceArray);
				//swappedPyraminx.SetBottomFace (faceArray);
			}
		}
	}

	void OnDestroy() {
		Destroy (initialPyraminx);
	}

	void OnApplicationQuit() {
		EndCapture();
	}
}
