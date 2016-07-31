using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class TestScript : MonoBehaviour {

    [DllImport("PyraminxSchemeCV.Windows")]
    private static extern void StartCapture();

    [DllImport("PyraminxSchemeCV.Windows")]
    private static extern void EndCapture();

    [DllImport("PyraminxSchemeCV.Windows")]
    private static extern void extResetColourCounts();

    [DllImport("PyraminxSchemeCV.Windows")]
    private static extern bool GetColours(int expectedOrientation, int[] colours);

    WebCamTexture webcamTexture;

    void Start () {
        extResetColourCounts();
        StartCapture();
    }
	
	// Update is called once per frame
	void Update () {
        int[] col = new int[9];
        bool result = GetColours(0, col);
        print(result);

        string test = "";
        foreach (int c in col)
        {
            test += " " + c;
        }
        print(test);
    }

    void OnApplicationQuit() {
        EndCapture();
    }
}
