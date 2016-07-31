using UnityEngine;
using System.Collections.Generic;

public class CubeSolver : MonoBehaviour {

	public GameObject _pyraminxVisual;

	private PyraminxVisual pyramixVisual;
	private Pyraminx P1;

	public List<SolutionStep> scrambleSteps = new List<SolutionStep>();
	public List<SolutionStep> solvedSteps;

	// Use this for initialization
	void Start () {
		P1 = new Pyraminx ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.I, P1));
		P1.L1 ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.L1, P1));
		P1.R1dash ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.R1D, P1));
		P1.R2 ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.R2, P1));
		P1.U2 ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.U2, P1));
		P1.L2 ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.L2, P1));
		P1.B2 ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.B2, P1));
		P1.U2dash ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.U2D, P1));
		P1.R2 ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.R2, P1));
		P1.U2 ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.U2, P1));
		P1.R2dash ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.R2D, P1));
		P1.L2 ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.L2, P1));
		P1.R2 ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.R2, P1));
		P1.B2 ();
		scrambleSteps.Add (new SolutionStep (SolutionStep.MoveStep.B2, P1));

		print (scrambleSteps.Count);

		Solver Solution = new Solver (P1);

		solvedSteps = Solution.Solve ();
		print (solvedSteps.Count);
	}

	// Update is called once per frame
	void Update () {
	}
}

public class Pyraminx {
	//variables
	public PFace frontFace, rightFace, leftFace, bottomFace;

	//The Constructor Function
	public Pyraminx() {
		//initialises a solved cube
		frontFace = new PFace(PFace.C.R); //front face is red
		rightFace = new PFace(PFace.C.G); //right face is green
		leftFace = new PFace(PFace.C.B); // left face is blue
		bottomFace = new PFace(PFace.C.Y); // bottom face is yellow
	}
	public void SetFrontColors(int[,] frontColors) {
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				frontFace.colours [i, j] = (PFace.C)frontColors [i, j];
			}
		}
	}

	public void SetLeftColors(int[,] frontColors) {
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				leftFace.colours [i, j] = (PFace.C)frontColors [i, j];
			}
		}
	}

	public void SetRightColors(int[,] frontColors) {
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				rightFace.colours [i, j] = (PFace.C)frontColors [i, j];
			}
		}
	}

	public void SetBottomColors(int[,] frontColors) {
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				bottomFace.colours [i, j] = (PFace.C)frontColors [i, j];
			}
		}
		bottomFace.rotateCW ();
	}
	//accessors for the 3d object
	//-------------------------------------------------------------------------------------------------------------------------------------
	public int[,] getFrontColours() { //returns face colours as integers
		int[,] faceColours = new int[3, 5];
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				faceColours [i, j] = (int)frontFace.colours [i, j];
			}
		}

		return faceColours;
	}
	public int[,] getRightColours() { //returns face colours as integers
		int[,] faceColours = new int[3, 5];
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				faceColours [i, j] = (int)rightFace.colours [i, j];
			}
		}

		return faceColours;
	}
	public int[,] getLeftColours() { //returns face colours as integers
		int[,] faceColours = new int[3, 5];
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				faceColours [i, j] = (int)leftFace.colours [i, j];
			}
		}

		return faceColours;
	}
	public int[,] getBottomColours() { //returns rotated face colours as integers
		//clones and rotates the face
		PFace TempFace = bottomFace.Clone();
		TempFace.rotateCCW();
		//converts to integers
		int[,] faceColours = new int[3, 5];
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				faceColours [i, j] = (int)TempFace.colours [i, j];
			}
		}

		return faceColours;
	}
		

	//specific accessors for the solver
	//get tips
	public int[] GetRightTip() {
		int[] Output = new int[3];
		Output [0] = (int)frontFace.colours [2, 4];
		Output [1] = (int)rightFace.colours [2,0];
		Output [2] = (int)bottomFace.colours [0,2];
		return Output;
	}
	public int[] GetLeftTip() {
		int[] Output = new int[3];
		Output [0] = (int)frontFace.colours [2, 0];
		Output [1] = (int)bottomFace.colours [2,0];
		Output [2] = (int)leftFace.colours [2,4];
		return Output;
	}
	public int[] GetTopTip() {
		int[] Output = new int[3];
		Output [0] = (int)frontFace.colours [0, 2];
		Output [1] = (int)leftFace.colours [0,2];
		Output [2] = (int)rightFace.colours [0,2];
		return Output;
	}
	public int[] GetBackTip() {
		int[] Output = new int[3];
		Output [0] = (int)rightFace.colours [2, 4];
		Output [1] = (int)leftFace.colours [2,0];
		Output [2] = (int)bottomFace.colours [2,4];
		return Output;
	}
	//get tip centres
	public int[] GetRightCentre() {
		int[] Output = new int[3];
		Output [0] = (int)frontFace.colours [2, 3];
		Output [1] = (int)rightFace.colours [2,1];
		Output [2] = (int)bottomFace.colours [1,2];
		return Output;
	}
	public int[] GetLeftCentre() {
		int[] Output = new int[3];
		Output [0] = (int)frontFace.colours [2, 1];
		Output [1] = (int)bottomFace.colours [2,1];
		Output [2] = (int)leftFace.colours [2,3];
		return Output;
	}
	public int[] GetTopCentre() {
		int[] Output = new int[3];
		Output [0] = (int)frontFace.colours [1, 2];
		Output [1] = (int)leftFace.colours [1,2];
		Output [2] = (int)rightFace.colours [1,2];

		return Output;
	}
	public int[] GetBackCentre() {
		int[] Output = new int[3];
		Output [0] = (int)rightFace.colours [2, 3];
		Output [1] = (int)leftFace.colours [2,1];
		Output [2] = (int)bottomFace.colours [2,3];
		return Output;
	}

	public int[,] GetTopEdges() {//this will return the edges on the top triangle, 
		// according to which face it is on, in cw fashion starting from right on the front face.
		int[,] Output = new int[3,2];
		Output [0,0] = (int)frontFace.colours [1,3];
		Output [0,1] = (int)frontFace.colours [1, 1];
		Output [1,0] = (int)leftFace.colours [1, 3];
		Output [1,1] = (int)leftFace.colours [1, 1];
		Output [2,0] = (int)rightFace.colours [1, 3];
		Output [2,1] = (int)rightFace.colours [1, 1];
		return Output;
	}

	//get face centres

	//GetFaceEdges

	//GetTipEdges

	public override string ToString (){
		string output = "Pyraminx orientation is: \n " +
		                "-------------------- \n ";
		                
		//printing front face
		output += "Front Face: \n";
		for (int i = 0; i < 3; i++) {
			output += "[";
			for (int j = 0; j < 5; j++) {
				output += frontFace.colours [i, j].ToString() + " ";
			}
			output += "]\n";
		}
		//printing right face
		output += "Right Face: \n";
		for (int i = 0; i < 3; i++) {
			output += "[";
			for (int j = 0; j < 5; j++) {
				output += rightFace.colours [i, j].ToString() + " ";
			}
			output += "]\n";
		}
		//printing left face
		output += "Left Face: \n";
		for (int i = 0; i < 3; i++) {
			output += "[";
			for (int j = 0; j < 5; j++) {
				output += leftFace.colours [i, j].ToString() + " ";
			}
			output += "]\n";
		}
		//printing bottomface
		output += "Bottom Face: \n";
		for (int i = 0; i < 3; i++) {
			output += "[";
			for (int j = 0; j < 5; j++) {
				output += bottomFace.colours [i, j].ToString() + " ";
			}
			output += "]\n";
		}
		return output;
	}

	//mutators
	// 1 = rotating tips, 2 = rotating tips + edges, 3 = whole cube rotations
	// default is cw, dash indicates CCW'
	//unless otherwise stated, commented indications for moving of individual colours is based of looking at their position on the front face
	//Right Tip
	public void R1(){
		PFace.C temporaryTip;
		temporaryTip = frontFace.colours [2, 4];
		frontFace.colours [2, 4] = bottomFace.colours [0, 2];
		bottomFace.colours [0, 2] = rightFace.colours [2, 0];
		rightFace.colours [2, 0] = temporaryTip;
	}
	public void R1dash(){
		R1 ();
		R1 ();
	}
	public void R2(){
		R1 ();
		//rotating middle layer
		//bottom edge
		PFace.C temporaryTip;
		temporaryTip = frontFace.colours [2, 2];
		frontFace.colours [2, 2] = bottomFace.colours [1, 3];
		bottomFace.colours [1, 3] = rightFace.colours [1, 1];
		rightFace.colours [1, 1] = temporaryTip;
		//center
		temporaryTip = frontFace.colours [2, 3];
		frontFace.colours [2, 3] = bottomFace.colours [1, 2];
		bottomFace.colours [1, 2] = rightFace.colours [2, 1];
		rightFace.colours [2, 1] = temporaryTip;
		//top edge
		temporaryTip = frontFace.colours [1, 3];
		frontFace.colours [1, 3] = bottomFace.colours [1, 1];
		bottomFace.colours [1, 1] = rightFace.colours [2, 2];
		rightFace.colours [2, 2] = temporaryTip;
	}
	public void R2dash(){
		R2 ();
		R2 ();
	}
	public void R3(){
		R2 ();
		//rotating far layer
		//bottom left tip
		PFace.C temporaryTip;
		temporaryTip = frontFace.colours [2,0];
		frontFace.colours [2,0] = bottomFace.colours [2,4];
		bottomFace.colours [2,4] = rightFace.colours [0,2];
		rightFace.colours [0,2] = temporaryTip;
		//bottom center
		temporaryTip = frontFace.colours [2,1];
		frontFace.colours [2,1] = bottomFace.colours [2,3];
		bottomFace.colours [2,3] = rightFace.colours [1,2];
		rightFace.colours [1,2] = temporaryTip;
		//edge
		temporaryTip = frontFace.colours [1,1];
		frontFace.colours [1,1] = bottomFace.colours [2,2];
		bottomFace.colours [2,2] = rightFace.colours [1,3];
		rightFace.colours [1,3] = temporaryTip;
		//top center
		temporaryTip = frontFace.colours [1,2];
		frontFace.colours [1,2] = bottomFace.colours [2,1];
		bottomFace.colours [2,1] = rightFace.colours [2,3];
		rightFace.colours [2,3] = temporaryTip;
		//top edge
		temporaryTip = frontFace.colours [0,2];
		frontFace.colours [0,2] = bottomFace.colours [2,0];
		bottomFace.colours [2,0] = rightFace.colours [2,4];
		rightFace.colours [2,4] = temporaryTip;

		//rotating the opposite face
		leftFace.rotateCCW();
	}
	public void R3dash(){
		R3 ();
		R3 ();
	}
	//---------------------------------
	//Left Tip
	public void L1(){
		PFace.C temporaryTip;
		temporaryTip = frontFace.colours [2, 0];
		frontFace.colours [2, 0] = leftFace.colours [2, 4];
		leftFace.colours [2, 4] = bottomFace.colours [2, 0];
		bottomFace.colours [2, 0] = temporaryTip;
	}
	public void L1dash(){
		L1 ();
		L1 ();
	}
	public void L2(){
		L1 ();
		//rotate middle layer
		PFace.C temporaryTip;
		//top edge
		temporaryTip = frontFace.colours [1,1];
		frontFace.colours [1,1] = leftFace.colours [2,2];
		leftFace.colours [2,2] = bottomFace.colours [1,1];
		bottomFace.colours [1,1] = temporaryTip;
		//center
		temporaryTip = frontFace.colours [2,1];
		frontFace.colours [2,1] = leftFace.colours [2,3];
		leftFace.colours [2,3] = bottomFace.colours [2,1];
		bottomFace.colours [2,1] = temporaryTip;
		//bottom edge
		temporaryTip = frontFace.colours [2,2];
		frontFace.colours [2,2] = leftFace.colours [1,3];
		leftFace.colours [1,3] = bottomFace.colours [2,2];
		bottomFace.colours [2,2] = temporaryTip;
		
	}
	public void L2dash(){
		L2 ();
		L2 ();
	}
	public void L3(){
		L2 ();
		//rotate far layer
		//top tip
		PFace.C temporaryTip;
		temporaryTip = frontFace.colours [0,2];
		frontFace.colours [0,2] = leftFace.colours [2,0];
		leftFace.colours [2,0] = bottomFace.colours [0,2];
		bottomFace.colours [0,2] = temporaryTip;
		//top center
		temporaryTip = frontFace.colours [1,2];
		frontFace.colours [1,2] = leftFace.colours [2,1];
		leftFace.colours [2,1] = bottomFace.colours [1,2];
		bottomFace.colours [1,2] = temporaryTip;
		//edge
		temporaryTip = frontFace.colours [1,3];
		frontFace.colours [1,3] = leftFace.colours [1,1];
		leftFace.colours [1,1] = bottomFace.colours [1,3];
		bottomFace.colours [1,3] = temporaryTip;
		//bottom right center
		temporaryTip = frontFace.colours [2,3];
		frontFace.colours [2,3] = leftFace.colours [1,2];
		leftFace.colours [1,2] = bottomFace.colours [2,3];
		bottomFace.colours [2,3] = temporaryTip;
		//bottom right tip
		temporaryTip = frontFace.colours [2,4];
		frontFace.colours [2,4] = leftFace.colours [0,2];
		leftFace.colours [0,2] = bottomFace.colours [2,4];
		bottomFace.colours [2,4] = temporaryTip;
		//rotating right face
		rightFace.rotateCCW();
	}
	public void L3dash(){
		L3 ();
		L3 ();
	}
	//---------------------------------
	//Upper Tip
	public void U1(){
		PFace.C temporaryTip;
		temporaryTip = frontFace.colours [0, 2];
		frontFace.colours [0, 2] = rightFace.colours [0, 2];
		rightFace.colours [0, 2] = leftFace.colours [0, 2];
		leftFace.colours [0, 2] = temporaryTip;
	}
	public void U1dash(){
		U1 ();
		U1 ();
	}
	public void U2(){
		U1 ();
		//rotating middle row
		for (int i = 1; i <= 3; i++) {
			PFace.C temporaryTip;
			temporaryTip = frontFace.colours [1, i];
			frontFace.colours [1, i] = rightFace.colours [1, i];
			rightFace.colours [1, i] = leftFace.colours [1, i];
			leftFace.colours [1, i] = temporaryTip;
		}
	}
	public void U2dash(){
		U2 ();
		U2 ();
	}
	public void U3(){
		U2 ();
		//rotating far row
		for (int i = 0; i <= 4; i++) {
			PFace.C temporaryTip;
			temporaryTip = frontFace.colours [2, i];
			frontFace.colours [2, i] = rightFace.colours [2, i];
			rightFace.colours [2, i] = leftFace.colours [2, i];
			leftFace.colours [2, i] = temporaryTip;
		}
		bottomFace.rotateCCW ();
	}
	public void U3dash(){
		U3 ();
		U3 ();
	}
	//-------------------------------------
	//back Tip
	public void B1(){
		PFace.C temporaryTip;
		temporaryTip = rightFace.colours [2,4];
		rightFace.colours [2,4] = bottomFace.colours [2,4];
		bottomFace.colours [2,4] = leftFace.colours [2,0];
		leftFace.colours [2,0] = temporaryTip;
	}
	public void B1dash(){
		B1 ();
		B1 ();
	}
	public void B2(){
		B1 ();
		//rotate middle layer
		PFace.C temporaryTip;
		//descriptions will be based off looking at the right face
		//UB edge (top-right on right face)
		temporaryTip = rightFace.colours [1,3];
		rightFace.colours [1,3] = bottomFace.colours [1,3];
		bottomFace.colours [1,3] = leftFace.colours [2,2];
		leftFace.colours [2,2] = temporaryTip;
		//B center
		temporaryTip = rightFace.colours [2,3];
		rightFace.colours [2,3] = bottomFace.colours [2,3];
		bottomFace.colours [2,3] = leftFace.colours [2,1];
		leftFace.colours [2,1] = temporaryTip;
		//BR edge (bottom center on right face)
		temporaryTip = rightFace.colours [2,2];
		rightFace.colours [2,2] = bottomFace.colours [2,2];
		bottomFace.colours [2,2] = leftFace.colours [1,1];
		leftFace.colours [1,1] = temporaryTip;
	}
	public void B2dash(){
		B2 ();
		B2 ();
	}
	public void B3(){
		B2 ();
		//rotate far layer
		PFace.C temporaryTip;
		//descriptions will be based off looking at the right face
		//top tip
		temporaryTip = rightFace.colours [0,2];
		rightFace.colours [0,2] = bottomFace.colours [0,2];
		bottomFace.colours [0,2] = leftFace.colours [2,4];
		leftFace.colours [2,4] = temporaryTip;
		//top center
		temporaryTip = rightFace.colours [1,2];
		rightFace.colours [1,2] = bottomFace.colours [1,2];
		bottomFace.colours [1,2] = leftFace.colours [2,3];
		leftFace.colours [2,3] = temporaryTip;
		//UR edge (top left on right face)
		temporaryTip = rightFace.colours [1,1];
		rightFace.colours [1,1] = bottomFace.colours [1,1];
		bottomFace.colours [1,1] = leftFace.colours [1,3];
		leftFace.colours [1,3] = temporaryTip;
		//bottom left center
		temporaryTip = rightFace.colours [2,1];
		rightFace.colours [2,1] = bottomFace.colours [2,1];
		bottomFace.colours [2,1] = leftFace.colours [1,2];
		leftFace.colours [1,2] = temporaryTip;
		//bottom left tip
		temporaryTip = rightFace.colours [2,0];
		rightFace.colours [2,0] = bottomFace.colours [2,0];
		bottomFace.colours [2,0] = leftFace.colours [0,2];
		leftFace.colours [0,2] = temporaryTip;

		//rotating opposite face
		frontFace.rotateCCW();
	}
	public void B3dash(){
		B3 ();
		B3 ();
	}
	//-------------------------------------------
	//compound moves or selectors
	public SolutionStep TurnSelector(int Corner, int Direction, int Depth){
		//for corner: 0=right,1=left,2=top,3=back
		//for direction: 0=cw, 1=ccw
		//depth from 1-3

		SolutionStep solutionStep = new SolutionStep ();

		if (Direction == 0) {
			if (Corner == 0) {
				if (Depth == 1) {
					R1 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.R1);
				} else if (Depth == 2) {
					R2 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.R2);
				} else if (Depth == 3) {
					R3 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.R3);
				}
			}
			if (Corner == 1) {
				if (Depth == 1) {
					L1 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.L1);
				} else if (Depth == 2) {
					L2 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.L2);
				} else if (Depth == 3) {
					L3 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.L3);
				}
			}
			if (Corner == 2) {
				if (Depth == 1) {
					U1 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.U1);
				} else if (Depth == 2) {
					U2 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.U2);
				} else if (Depth == 3) {
					U3 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.U3);
				}
			}
			if (Corner == 3) {
				if (Depth == 1) {
					B1 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.B1);
				} else if (Depth == 2) {
					B2 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.B2);
				} else if (Depth == 3) {
					B3 ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.B3);
				}
			}
		} 
		else if (Direction == 1) {
			if (Corner == 0) {
				if (Depth == 1) {
					R1dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.R1D);
				} else if (Depth == 2) {
					R2dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.R2D);
				} else if (Depth == 3) {
					R3dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.R3D);
				}
			}
			if (Corner == 1) {
				if (Depth == 1) {
					L1dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.L1D);
				} else if (Depth == 2) {
					L2dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.L2D);
				} else if (Depth == 3) {
					L3dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.L3D);
				}
			}
			if (Corner == 2) {
				if (Depth == 1) {
					U1dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.U1D);
				} else if (Depth == 2) {
					U2dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.U2D);
				} else if (Depth == 3) {
					U3dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.U3D);
				}
			}
			if (Corner == 3) {
				if (Depth == 1) {
					B1dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.B1D);
				} else if (Depth == 2) {
					B2dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.B2D);
				} else if (Depth == 3) {
					B3dash ();
					solutionStep.SetMoveStep (SolutionStep.MoveStep.B3D);
				}
			}
		}

		solutionStep.newFront = getFrontColours ();
		solutionStep.newLeft = getLeftColours ();
		solutionStep.newRight = getRightColours ();
		solutionStep.newBottom = getBottomColours ();

		return solutionStep;
	}

	public SolutionStep[] insertRightEdge(){ //performs the sledgehammer algorithm
		SolutionStep[] steps = new SolutionStep[4];

		R2dash();
		steps [0] = new SolutionStep (SolutionStep.MoveStep.R2D, this);
		L2();
		steps [1] = new SolutionStep (SolutionStep.MoveStep.L2,  this);
		R2();
		steps [2] = new SolutionStep (SolutionStep.MoveStep.R2,  this);
		L2dash();
		steps [3] = new SolutionStep (SolutionStep.MoveStep.L2D,  this);

		return steps;
	}
	public SolutionStep[] insertLeftEdge(){ //performs the flipped sledgehammer algorithm
		SolutionStep[] steps = new SolutionStep[4];

		L2();
		steps [0] = new SolutionStep (SolutionStep.MoveStep.L2,  this);
		R2dash();
		steps [1] = new SolutionStep (SolutionStep.MoveStep.R2D, this);
		L2dash();
		steps [2] = new SolutionStep (SolutionStep.MoveStep.L2D, this);
		R2();
		steps [3] = new SolutionStep (SolutionStep.MoveStep.R2,  this);

		return steps;

	}
}

public class PFace{
	//initialising the colours
	public enum C  {R, G, B, Y, E}; //R=red, G=green, B=Blue, Y=Yellow, E=Error/Unassigned

	//variables
	public C[,] colours = new C[3,5] {{C.E, C.E, C.R, C.E, C.E},{C.E, C.R, C.R, C.R, C.E}, {C.R, C.R, C.R, C.R, C.R}};
	// The Constructor Function
	public PFace(C initialcolour){
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				if (colours [i, j] == C.R) {
					colours [i, j] = initialcolour;
				}
			}
		}
	}

	//private methods
	public void rotateCW(){
		C temporaryColour;
		//rotate tip colours
		temporaryColour = colours[0,2];
		colours [0, 2] = colours [2, 0];
		colours [2, 0] = colours [2, 4];
		colours [2,4] = temporaryColour;
		//rotate edge colours
		temporaryColour = colours[1,3];
		colours [1, 3] = colours [1, 1];
		colours [1, 1] = colours [2, 2];
		colours [2, 2] = temporaryColour;
		//rotate centre colours
		temporaryColour = colours[1,2];
		colours [1, 2] = colours [2, 1];
		colours [2, 1] = colours [2, 3];
		colours [2, 3] = temporaryColour;
	}

	public PFace Clone(){
		PFace tempFace = new PFace (C.R);

		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				tempFace.colours [i, j] = colours [i, j];
			}
		}

		return tempFace;
	}

	public void rotateCCW(){
		rotateCW ();
		rotateCW ();
	}
		
		
		
		
}