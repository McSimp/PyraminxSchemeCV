using UnityEngine;
using System.Collections.Generic;

public class Solver {
	Pyraminx CurrentPyraminx = new Pyraminx();
	List<SolutionStep> solutionSteps = new List<SolutionStep>();

	//the constructor function
	public Solver(Pyraminx initialPyraminx){
		CurrentPyraminx = initialPyraminx;
		solutionSteps.Add (new SolutionStep ());

		solutionSteps[0].newFront = CurrentPyraminx.getFrontColours ();
		solutionSteps[0].newLeft = CurrentPyraminx.getLeftColours ();
		solutionSteps[0].newRight = CurrentPyraminx.getRightColours ();
		solutionSteps[0].newBottom = CurrentPyraminx.getBottomColours ();

	}

	public List<SolutionStep> Solve(){
		
		solutionSteps = new List<SolutionStep> ();

		SolveTips ();

		SolveCentres ();

		SolveBottomLayer();

		//SolveLastCorner();
		return solutionSteps;
	
	}
	public void SolveTips(){//generates the turns that orientates the tips
		//creating local copies of the colours organised into tips and centres.
		// 0 = right, 1=Left,2=Top,3=Back
		int[,] Tips = new int[4,3];
		fillRowOf2DArray (Tips,0, CurrentPyraminx.GetRightTip ());
		fillRowOf2DArray (Tips,1, CurrentPyraminx.GetLeftTip ());
		fillRowOf2DArray (Tips,2, CurrentPyraminx.GetTopTip ());
		fillRowOf2DArray (Tips,3, CurrentPyraminx.GetBackTip ());

		int[,] Centres = new int[4,3];
		fillRowOf2DArray (Centres,0, CurrentPyraminx.GetRightCentre ());
		fillRowOf2DArray (Centres,1, CurrentPyraminx.GetLeftCentre ());
		fillRowOf2DArray (Centres,2, CurrentPyraminx.GetTopCentre ());
		fillRowOf2DArray (Centres,3, CurrentPyraminx.GetBackCentre ());

		for (int i = 0; i < 4; i++) { //cycles through the different corners
			if (Tips[i,0] != Centres [i,0]) { //checks to see if the centre is misaligned
				if (Tips[i,0] == Centres [i,1]) { // checks if out by CCW
					solutionSteps.Add(CurrentPyraminx.TurnSelector(i,0,1)); //twists tip CW
				}
				if (Tips[i,0] == Centres [i,2]){ //checks if out by CW
					solutionSteps.Add(CurrentPyraminx.TurnSelector(i,1,1)); //twists tip CCW
				}
			}
		}
		//tips finished :)
	}
	public void SolveCentres(){
		//creating local copies of the centres organised into their respective tips.
		// 0 = right, 1=Left,2=Top,3=Back
		int[,] Centres = new int[4,3];
		fillRowOf2DArray (Centres,0, CurrentPyraminx.GetRightCentre ());
		fillRowOf2DArray (Centres,1, CurrentPyraminx.GetLeftCentre ());
		fillRowOf2DArray (Centres,2, CurrentPyraminx.GetTopCentre ());
		fillRowOf2DArray (Centres,3, CurrentPyraminx.GetBackCentre ());

		//checking what number is missing from the back center because we know that will be the colour of the front face
		int frontFaceColour = -1;
		if (Centres [3,0] != 0 && Centres [3,1] != 0 && Centres [3,2] != 0) {
			frontFaceColour = 0;
		}
		else if (Centres [3,0] != 1 && Centres [3,1] != 1 && Centres [3,2] != 1) {
			frontFaceColour = 1;
		}
		else if (Centres [3,0] != 2 && Centres [3,1] != 2 && Centres [3,2] != 2) {
			frontFaceColour = 2;
		}
		else if (Centres [3,0] != 3 && Centres [3,1] != 3 && Centres [3,2] != 3) {
			frontFaceColour = 3;
		}
		//---------------------------------------------
		//now we have found the colour of the front face, we will cycle around the front 3 tips performing the turns to orient them correctly
		for (int i = 0; i < 3; i++) { //cycles through the different corners
			if (Centres [i,0] != frontFaceColour) { //checks whether the front face of the tip matches the final front face colour
				if (Centres [i,1] == frontFaceColour) { //checks whether the face is out by CW rotation
					solutionSteps.Add(CurrentPyraminx.TurnSelector(i,1,2)); //performs ccw twist to depth 2
				}
				else if (Centres [i,2] == frontFaceColour) { //checks whether the face is out by CCW rotation
					solutionSteps.Add(CurrentPyraminx.TurnSelector(i,0,2)); //performs CW twist to depth 2
				}
			}
		}
		//-------------------------------------------------------
		//now we have oriented the front three centers we will check the back center against one and then orient it, completing this step
		if (Centres [0,1] != Centres [3,0]) { //checks whether the back tip matches the right tip on the right face
			if (Centres [0,1] != Centres [3,1]) { 
				solutionSteps.Add(CurrentPyraminx.TurnSelector(3,1,2)); //performs B2dash
			}
			if (Centres [0,1] != Centres [3,2]) { 
				solutionSteps.Add(CurrentPyraminx.TurnSelector(3,0,2)); //performs B2
			}
		}
		//centres Finished :)

	}
	public void SolveBottomLayer(){
		//first thing we need to do is decide what bottom layer will be easiest to solve
		//-----------------------------------------------------------------
		chooseAndInsertSingleEdge();
		chooseAndInsertSingleEdge();
		chooseAndInsertSingleEdge();

	}

	public void SolveLastCorner(){
	}

	public void chooseAndInsertSingleEdge(){
		//taking note of which colour is on each face
		// 0=front,1=right,2=left,3=bottom
		// also note 0=R,1=G,2=B,3=Y,4=Unassigned
		int [] AssignedFaceColour = new int[4];
		int[] topCentre = CurrentPyraminx.GetTopCentre();
		int[] rightCentre = CurrentPyraminx.GetRightCentre();
		int[] leftCentre = CurrentPyraminx.GetLeftCentre();
		AssignedFaceColour[0] = rightCentre [0]; //front
		AssignedFaceColour[1] = rightCentre [1]; //right
		AssignedFaceColour[2] = leftCentre [2]; //left
		AssignedFaceColour[3] = rightCentre [2]; //bottom
		//in the future i will add a section which finds out which side would be best so solve as the bottom layer
		//collect top edges and see if any of them contain the bottom color
		//------------------------------------------------------------------
		//assign values to each case
		int[,] TopEdges = CurrentPyraminx.GetTopEdges(); 
		bool[,] PresenceArray = checkForPresenceOfBottomColour (TopEdges, AssignedFaceColour [3]); //creates a boolean of whether the bottom colour is there
		int[,] ValueArray = new int[3,2]; // will be 1 if good edge, 2 if perfect edge)
		int[] CCWchecker = new int[3] {2,0,1}; //cycles the indexes of faces for checking below
		int[] CWchecker = new int[3] {1,2,0}; //cycles the indexes of faces for checking below

		for (int i = 0; i < 3; i++) {
			if (PresenceArray[i,1]) { // checks if the left edge of the face is the bottom colour
				if (TopEdges[CWchecker[i],0] == AssignedFaceColour[i]){ //checks whether the other colour in the edge matches the right adjacent face
					ValueArray[i,1] = 2;
				}
				else{
					ValueArray [i, 1] = 1;
				}
			}
			else if (PresenceArray[i,0]){ // checks if the right edge of the face is the bottom colour
				if (TopEdges[CCWchecker[i],1] == AssignedFaceColour[i]){ //checks whether the other colour in the edge matches the left adjacent face
					ValueArray[i,0] = 2;
				}
				else{
					ValueArray[i,0] = 1;
				}
			}
		}

		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 2; j++) {
				Debug.Log(PresenceArray[i,j]);
			}
		}
		//--------------------------------------------------------------------
		//going through and performing the first perfect case (when value Array = 2)
		//if that doesnt yield any results it will try for imperfect, (when value Array = 1)
		//when a move is performed it will break out of everything
		for (int pass = 2; pass>0; pass--){
			bool break0 = false;
			for (int i = 0; i < 3; i++) { //cycling faces
				for (int j = 0;j<2;j++){ //cycling edges
					if (ValueArray[i,0] == 2){ //right edge perfect case

						//perform whole cube rotations to fix orientation
						//-------------------------------------------------
						moveFacetoFront(i); //move to the face looking at the edge
						//-----------------------------------------------------------------
						//perform algorythm
						SolutionStep[] steps = CurrentPyraminx.insertRightEdge();
						for (int a = 0; a < 4; a++) {
							solutionSteps.Add(steps[a]);
						}

						break0 = true;
						break;
					}



					else if (ValueArray[i,0] == 1){ //right edge imperfect case
						//perform whole cube rotations to fix orientation
						//-------------------------------------------------
						moveFacetoFront(System.Array.IndexOf(AssignedFaceColour,TopEdges[CCWchecker[i],1])); //move to the correct base - face
						//----------------------------------------------------------------
						topTriangletoFront(TopEdges[CWchecker[i],1]); // move the top triange to correct position
						//-----------------------------------------------------------------
						//perform algorythm
						SolutionStep[] steps = CurrentPyraminx.insertRightEdge();
						for (int a = 0; a < 4; a++) {
							solutionSteps.Add(steps[a]);
						}

						break0 = true;
						break;
					}


					if (ValueArray[i,1] == 2){ //left edge perfect case

						//perform whole cube rotations to fix orientation
						//-------------------------------------------------
						moveFacetoFront(i); //move to the face looking at the edge
						//-----------------------------------------------------------------
						//perform algorythm
						SolutionStep[] steps = CurrentPyraminx.insertLeftEdge();
						for (int a = 0; a < 4; a++) {
							solutionSteps.Add(steps[a]);
						}

						break0 = true;
						break;
					}



					else if (ValueArray[i,1] == 1){//left edge imperfect case
						//perform whole cube rotations to fix orientation
						//-------------------------------------------------
						moveFacetoFront(System.Array.IndexOf(AssignedFaceColour,TopEdges[CCWchecker[i],0])); //move to the correct base - face
						//----------------------------------------------------------------
						topTriangletoFront(TopEdges[CWchecker[i],0]); // move the top triange to correct position
						//-----------------------------------------------------------------
						//perform algorythm
						SolutionStep[] steps = CurrentPyraminx.insertLeftEdge();
						for (int a = 0; a < 4; a++) {
							solutionSteps.Add(steps[a]);
						}

						break0 = true;
						break;
					}
				}
				if (break0) break;
			}
			if (break0) break;
		}
	}
	private void moveFacetoFront(int currentfaceposition){
		if (currentfaceposition == 0){ //already correct
		}
		else if (currentfaceposition == 1){
			//U3dash
			CurrentPyraminx.U3();
			solutionSteps.Add(new SolutionStep(SolutionStep.MoveStep.U3,CurrentPyraminx));
		}
		else if (currentfaceposition == 2){
			CurrentPyraminx.U3dash();
			solutionSteps.Add(new SolutionStep(SolutionStep.MoveStep.U3D,CurrentPyraminx));
			//U3
		}
	}

	private void topTriangletoFront(int currentfaceposition){
		if (currentfaceposition == 0){ //already correct
		}
		else if (currentfaceposition == 1){
			//U2dash
			CurrentPyraminx.U2();
			solutionSteps.Add(new SolutionStep(SolutionStep.MoveStep.U2,CurrentPyraminx));
		}
		else if (currentfaceposition == 2){
			CurrentPyraminx.U2dash();
			solutionSteps.Add(new SolutionStep(SolutionStep.MoveStep.U2D,CurrentPyraminx));
			//U2
		}
	}

	private bool[,] checkForPresenceOfBottomColour(int [,] TopEdges, int BottomLayerColour){ //used in the solve bottom layer step to find out what parts are in the top edges
		bool[,] PresenceArray = new bool[3, 2];
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 2; j++) {
				if (TopEdges [i, j] == BottomLayerColour) {
					PresenceArray [i, j] = true;
				}
			}
		}
		return PresenceArray;
	}
	void fillRowOf2DArray(int [,] destinationArray, int row, int [] sourceArray){

		for (int i = 0; i < sourceArray.Length; i++) {
			destinationArray [row,i] = sourceArray [i];
		}

	}

}

// R1 R1' R2 R2' R3 R3' L1 L1' L2 L2' L3 L3' U1 U1' U2 U2' U3 U3' B1 B1' B2 B2' B3 B3'
public class SolutionStep {
	public enum MoveStep {R1, R1D, R2, R2D, R3, R3D, L1, L1D, L2, L2D, L3, L3D, U1, U1D, U2, U2D, U3, U3D, B1, B1D, B2, B2D, B3, B3D, I};

	public MoveStep moveStep;
	public int[,] newFront;
	public int[,] newLeft;
	public int[,] newRight;
	public int[,] newBottom;

	public SolutionStep() {

	}

	public SolutionStep(MoveStep moveStep) {
		this.moveStep = moveStep;
	}
		
	public SolutionStep(MoveStep moveStep, Pyraminx P) {
		this.moveStep = moveStep;
		newFront = P.getFrontColours ();
		newLeft = P.getLeftColours ();
		newRight = P.getRightColours ();
		newBottom = P.getBottomColours ();
	}

	public void SetMoveStep(MoveStep moveStep) {
		this.moveStep = moveStep;
	}
}
