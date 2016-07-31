using UnityEngine;
using System.Collections;

public class TriColorGameObj : MonoBehaviour {
    public Texture2D upTriTex;
    public Texture2D downTriTex;

	void Start () {
        TriColor.genColorTex(upTriTex.GetPixels(), 0, false);
        TriColor.genColorTex(upTriTex.GetPixels(), 1, false);
        TriColor.genColorTex(upTriTex.GetPixels(), 2, false);
        TriColor.genColorTex(upTriTex.GetPixels(), 3, false);
		TriColor.genColorTex(upTriTex.GetPixels(), 4, false);

        TriColor.genColorTex(downTriTex.GetPixels(), 0, true);
        TriColor.genColorTex(downTriTex.GetPixels(), 1, true);
        TriColor.genColorTex(downTriTex.GetPixels(), 2, true);
        TriColor.genColorTex(downTriTex.GetPixels(), 3, true);
		TriColor.genColorTex(downTriTex.GetPixels(), 4, true);
    }
}


public class TriColor
{
    public static Color red = new Color(1, 0, 0);
    public static Color green = new Color(0, 1, 0);
    public static Color blue = new Color(0, 0, 1);
    public static Color yellow = new Color(1, 1, 0);
    public static Color error = new Color(1, 1, 1);

    public static Color[] redTriTex;
    public static Color[] greenTriTex;
    public static Color[] blueTriTex;
    public static Color[] yellowTriTex;
	public static Color[] errorTriTex;

    public static Color[] redTriDTex;
    public static Color[] greenTriDTex;
    public static Color[] blueTriDTex;
    public static Color[] yellowTriDTex;
	public static Color[] errorTriDTex;
    

    public static Color getColor(int colorNum)
    {
        switch (colorNum)
        {
            case 0: return red;
            case 1: return green;
            case 2: return blue;
            case 3: return yellow;
			case 4: return error;
        }

        return error;
    }

    public static Color[] getColorTex(int colorNum, bool upsideDown)
    {
        if (upsideDown)
        {
            switch (colorNum)
            {
                case 0: return redTriDTex;
                case 1: return greenTriDTex;
                case 2: return blueTriDTex;
                case 3: return yellowTriDTex;
				case 4: return errorTriDTex; 
            }
        }
        else {
            switch (colorNum)
            {
                case 0: return redTriTex;
                case 1: return greenTriTex;
                case 2: return blueTriTex;
                case 3: return yellowTriTex;
				case 4: return errorTriTex; 
            }
        }
        return errorTriTex;
    }

    public static void genColorTex(Color[] inputTex, int colorNum, bool upsideDown)
    {
        if (!upsideDown)
        {
            switch (colorNum)
            {
                case 0: redTriTex = new Color[inputTex.Length]; break;
                case 1: greenTriTex = new Color[inputTex.Length]; break;
                case 2: blueTriTex = new Color[inputTex.Length]; break;
                case 3: yellowTriTex = new Color[inputTex.Length]; break;
				case 4: errorTriTex = new Color[inputTex.Length]; break;
            }
        }
        else {
            switch (colorNum)
            {
                case 0: redTriDTex = new Color[inputTex.Length]; break;
                case 1: greenTriDTex = new Color[inputTex.Length]; break;
                case 2: blueTriDTex = new Color[inputTex.Length]; break;
                case 3: yellowTriDTex = new Color[inputTex.Length]; break;
				case 4: errorTriDTex = new Color[inputTex.Length]; break;
            }
        }

        Color[] colorTex = getColorTex(colorNum, upsideDown);
        Color triColor = getColor(colorNum);
        for (int i = 0; i < inputTex.Length; i++)
        {
            if (inputTex[i].Equals(Color.white) && inputTex[i].a > 0)
            {
                colorTex[i] = triColor;
            }
            else
            {
                colorTex[i] = inputTex[i];
            }
        }
    }
}
