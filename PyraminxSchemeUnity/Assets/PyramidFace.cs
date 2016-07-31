using UnityEngine;
using System.Collections;

public class PyramidFace : MonoBehaviour {

    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uv;
    private int[] triangles;
    private Mesh triangleMesh;

    private Color[] facePixels;
    private Texture2D faceTex;

    private int[,] faceColors = new int[3, 5] { { -1, -1, 0, -1, -1 }, { -1, 0, 0, 0, -1 }, { 0, 0, 0, 0, 0 } };

    private int triangleWidth = 170;
    private int triangleHeight = 148;

    private int faceWidth = 512;
    private int faceHeight = 444;

	private float vertexHeight = Mathf.Tan(60 * Mathf.PI / 180)*0.5f;
	public GameObject splitFace;

	private Vector3 vertexOffset = new Vector3(-0.5f, 0, 0);

    void Start () {
        CreateMesh();
        GetComponent<MeshFilter>().mesh = triangleMesh;
	}

	public void Initialise(int InitialColor) {
		
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				if (faceColors [i, j] != -1) {
					faceColors [i, j] = InitialColor;
				}
			}
		}

		CreateTexture();
		GetComponent<MeshRenderer>().material.SetTexture("_MainTex", faceTex);

	}

	public int[,] GetFaceColors() {
		return faceColors;
	}

	public void SetFaceColors(int[,] newFaceColors) {
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 5; j++) {
				if (faceColors [i, j] != -1) {
					if (newFaceColors [i, j] != faceColors [i, j]) {
						faceColors [i, j] = newFaceColors [i, j];
						SetTextureTriangle (j, i, false);
					}
				}
			}
		}
		faceTex.SetPixels (0, 0, 512, 444, facePixels);
		faceTex.Apply ();
	}

    void CreateTexture()
    {
        faceTex = new Texture2D(512, 444, TextureFormat.RGBA32, false);
        facePixels = new Color[512 * 444];

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                if (faceColors[r, c] != -1)
                {
                    bool upsideDown = (c + 1) % 2 == 0;
                    if (r == 1) upsideDown = !upsideDown;
                    Color[] pieceColors = TriColor.getColorTex(faceColors[r, c], upsideDown);

                    for (int w = 0; w < triangleWidth; w++)
                    {
                        for (int h = 0; h < triangleHeight; h++)
                        {
                            int pieceIndex = h * triangleWidth + w;
                            int writeIndex = ((2-r) * triangleHeight + h) * faceWidth + w + c * 85;

                            if (pieceColors[pieceIndex].a != 0)
                               facePixels[writeIndex] = pieceColors[pieceIndex];
                        }
                    }
                }
            }
        }

        faceTex.SetPixels(0, 0, 512, 444, facePixels);
        faceTex.Apply();
    }

	void SetTextureTriangle(int x, int y, bool updateTex)
    {
        if (faceColors[y, x] != -1)
        {
            bool upsideDown = (x + 1) % 2 == 0;
            if (y == 1) upsideDown = !upsideDown;
            Color[] pieceColors = TriColor.getColorTex(faceColors[y, x], upsideDown);

            for (int w = 0; w < triangleWidth; w++)
            {
                for (int h = 0; h < triangleHeight; h++)
                {
                    int pieceIndex = h * triangleWidth + w;
                    int writeIndex = ((2 - y) * triangleHeight + h) * faceWidth + w + x * 85;

                    if (pieceColors[pieceIndex].a != 0)
                        facePixels[writeIndex] = pieceColors[pieceIndex];
                }
            }
        } else
        {
            print("Invalid color position!");
        }

		if (updateTex) {
			faceTex.SetPixels (0, 0, 512, 444, facePixels);
			faceTex.Apply ();
		}
    }

    void CreateMesh()
    {
        vertices = new Vector3[3];
        normals = new Vector3[3];
        uv = new Vector2[3];
        triangles = new int[3];
        triangleMesh = new Mesh();

        vertices[0] = new Vector3(-0.5f, 0, 0);
        vertices[1] = new Vector3(0.5f, 0, 0);
		vertices[2] = new Vector3(0, 0, vertexHeight);

        normals[0] = new Vector3(0, 1, 0);
        normals[1] = new Vector3(0, 1, 0);
        normals[2] = new Vector3(0, 1, 0);

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0.5f, 1);

        triangles[0] = 2;
        triangles[1] = 1;
        triangles[2] = 0;

        triangleMesh.vertices = vertices;
        triangleMesh.normals = normals;
        triangleMesh.triangles = triangles;
        triangleMesh.uv = uv;
    }

	public void DisableMeshRenderer() {
		GetComponent<MeshRenderer> ().enabled = false;
	}

	public void EnableMeshRenderer() {
		GetComponent<MeshRenderer> ().enabled = true;
	}

	public SplitFace SplitWhole() {
		GameObject wholeFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);

		SplitFace wholeSplitFace = wholeFace.GetComponent<SplitFace> ();

		wholeSplitFace.Initialise (faceTex, vertices, uv, triangles);

		wholeSplitFace.transform.parent = transform;
		wholeSplitFace.transform.localPosition = Vector3.zero;
		wholeSplitFace.transform.localRotation = Quaternion.identity;

		return wholeSplitFace;
	}

	public SplitFace[] SplitTopSmall() {
		Vector3[] topVertices = new Vector3[3];
		topVertices [0] = new Vector3 (1.0f / 3.0f, 0, vertexHeight * 2.0f / 3.0f) + vertexOffset;
		topVertices [1] = new Vector3 (2.0f / 3.0f, 0, vertexHeight * 2.0f / 3.0f) + vertexOffset;
		topVertices [2] = new Vector3 (0.5f, 0, vertexHeight) + vertexOffset;

		Vector2[] topUV = new Vector2[3];
		topUV [0] = new Vector2 (1.0f / 3.0f, 2.0f / 3.0f);
		topUV [1] = new Vector2 (2.0f / 3.0f,  2.0f / 3.0f);
		topUV [2] = new Vector2 (0.5f, 1);

		int[] topTriangles = new int[3];
		topTriangles[0] = 2;
		topTriangles[1] = 1;
		topTriangles[2] = 0;

		Vector3[] bottomVertices = new Vector3[4];
		bottomVertices [0] = new Vector3 (-0, 0, 0) + vertexOffset;
		bottomVertices [1] = new Vector3 (1, 0, 0) + vertexOffset;
		bottomVertices [2] = new Vector3 (2.0f / 3.0f, 0, vertexHeight * 2.0f / 3.0f) + vertexOffset;
		bottomVertices [3] = new Vector3 (1.0f / 3.0f, 0, vertexHeight * 2.0f / 3.0f) + vertexOffset;

		Vector2[] bottomUV = new Vector2[4];
		bottomUV [0] = new Vector2 (0, 0);
		bottomUV [1] = new Vector2 (1, 0);
		bottomUV [2] = new Vector2 (2.0f / 3.0f,  2.0f / 3.0f);
		bottomUV [3] = new Vector2 (1.0f / 3.0f, 2.0f / 3.0f);

		int[] bottomTriangles = new int[6];
		bottomTriangles[0] = 2;
		bottomTriangles[1] = 1;
		bottomTriangles[2] = 0;
		bottomTriangles[3] = 0;
		bottomTriangles[4] = 3;
		bottomTriangles[5] = 2;

		GameObject topFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);
		GameObject bottomFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);

		SplitFace topSplitFace = topFace.GetComponent<SplitFace> ();
		SplitFace bottomSplitFace = bottomFace.GetComponent<SplitFace> ();

		topSplitFace.Initialise (faceTex, topVertices, topUV, topTriangles);
		bottomSplitFace.Initialise (faceTex, bottomVertices, bottomUV, bottomTriangles);

		topSplitFace.transform.parent = transform;
		topSplitFace.transform.localPosition = Vector3.zero;
		topSplitFace.transform.localRotation = Quaternion.identity;

		bottomSplitFace.transform.parent = transform;
		bottomSplitFace.transform.localPosition = Vector3.zero;
		bottomSplitFace.transform.localRotation = Quaternion.identity;

		return new SplitFace[] { topSplitFace, bottomSplitFace };
	}

	public SplitFace[] SplitTopLarge() {
		Vector3[] topVertices = new Vector3[3];
		topVertices [0] = new Vector3 (0.5f / 3.0f, 0, vertexHeight * 1.0f / 3.0f) + vertexOffset;
		topVertices [1] = new Vector3 (2.5f / 3.0f, 0, vertexHeight * 1.0f / 3.0f) + vertexOffset;
		topVertices [2] = new Vector3 (0.5f, 0, vertexHeight) + vertexOffset;

		Vector2[] topUV = new Vector2[3];
		topUV [0] = new Vector2 (0.5f / 3.0f, 1.0f / 3.0f);
		topUV [1] = new Vector2 (2.5f / 3.0f,  1.0f / 3.0f);
		topUV [2] = new Vector2 (0.5f, 1);

		int[] topTriangles = new int[3];
		topTriangles[0] = 2;
		topTriangles[1] = 1;
		topTriangles[2] = 0;

		Vector3[] bottomVertices = new Vector3[4];
		bottomVertices [0] = new Vector3 (-0, 0, 0) + vertexOffset;
		bottomVertices [1] = new Vector3 (1, 0, 0) + vertexOffset;
		bottomVertices [2] = new Vector3 (2.5f / 3.0f, 0, vertexHeight * 1.0f / 3.0f) + vertexOffset;
		bottomVertices [3] = new Vector3 (0.5f / 3.0f, 0, vertexHeight * 1.0f / 3.0f) + vertexOffset;

		Vector2[] bottomUV = new Vector2[4];
		bottomUV [0] = new Vector2 (0, 0);
		bottomUV [1] = new Vector2 (1, 0);
		bottomUV [2] = new Vector2 (2.5f / 3.0f,  1.0f / 3.0f);
		bottomUV [3] = new Vector2 (0.5f / 3.0f, 1.0f / 3.0f);

		int[] bottomTriangles = new int[6];
		bottomTriangles[0] = 2;
		bottomTriangles[1] = 1;
		bottomTriangles[2] = 0;
		bottomTriangles[3] = 0;
		bottomTriangles[4] = 3;
		bottomTriangles[5] = 2;

		GameObject topFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);
		GameObject bottomFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);

		SplitFace topSplitFace = topFace.GetComponent<SplitFace> ();
		SplitFace bottomSplitFace = bottomFace.GetComponent<SplitFace> ();

		topSplitFace.Initialise (faceTex, topVertices, topUV, topTriangles);
		bottomSplitFace.Initialise (faceTex, bottomVertices, bottomUV, bottomTriangles);

		topSplitFace.transform.parent = transform;
		topSplitFace.transform.localPosition = Vector3.zero;
		topSplitFace.transform.localRotation = Quaternion.identity;

		bottomSplitFace.transform.parent = transform;
		bottomSplitFace.transform.localPosition = Vector3.zero;
		bottomSplitFace.transform.localRotation = Quaternion.identity;

		return new SplitFace[] { topSplitFace, bottomSplitFace };
	}

	public SplitFace[] SplitLeftSmall() {
		Vector3[] topVertices = new Vector3[3];
		topVertices [0] = new Vector3 (0, 0, 0) + vertexOffset;
		topVertices [1] = new Vector3 (1.0f / 3.0f, 0, 0) + vertexOffset;
		topVertices [2] = new Vector3 (0.5f / 3.0f, 0, vertexHeight * 1.0f / 3.0f) + vertexOffset;

		Vector2[] topUV = new Vector2[3];
		topUV [0] = new Vector2 (0, 0);
		topUV [1] = new Vector2 (1.0f / 3.0f,  0);
		topUV [2] = new Vector2 (0.5f / 3.0f, 1.0f / 3.0f);

		int[] topTriangles = new int[3];
		topTriangles[0] = 2;
		topTriangles[1] = 1;
		topTriangles[2] = 0;

		Vector3[] bottomVertices = new Vector3[4];
		bottomVertices [0] = new Vector3 (1.0f / 3.0f, 0, 0) + vertexOffset;
		bottomVertices [1] = new Vector3 (1, 0, 0) + vertexOffset;
		bottomVertices [2] = new Vector3 (0.5f, 0, vertexHeight) + vertexOffset;
		bottomVertices [3] = new Vector3 (0.5f / 3.0f, 0, vertexHeight * 1.0f / 3.0f) + vertexOffset;

		Vector2[] bottomUV = new Vector2[4];
		bottomUV [0] = new Vector2 (1.0f / 3.0f, 0);
		bottomUV [1] = new Vector2 (1, 0);
		bottomUV [2] = new Vector2 (0.5f,  1);
		bottomUV [3] = new Vector2 (0.5f / 3.0f, 1.0f / 3.0f);

		int[] bottomTriangles = new int[6];
		bottomTriangles[0] = 2;
		bottomTriangles[1] = 1;
		bottomTriangles[2] = 0;
		bottomTriangles[3] = 0;
		bottomTriangles[4] = 3;
		bottomTriangles[5] = 2;

		GameObject topFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);
		GameObject bottomFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);

		SplitFace topSplitFace = topFace.GetComponent<SplitFace> ();
		SplitFace bottomSplitFace = bottomFace.GetComponent<SplitFace> ();

		topSplitFace.Initialise (faceTex, topVertices, topUV, topTriangles);
		bottomSplitFace.Initialise (faceTex, bottomVertices, bottomUV, bottomTriangles);

		topSplitFace.transform.parent = transform;
		topSplitFace.transform.localPosition = Vector3.zero;
		topSplitFace.transform.localRotation = Quaternion.identity;

		bottomSplitFace.transform.parent = transform;
		bottomSplitFace.transform.localPosition = Vector3.zero;
		bottomSplitFace.transform.localRotation = Quaternion.identity;

		return new SplitFace[] { topSplitFace, bottomSplitFace };
	}

	public SplitFace[] SplitLeftLarge() {
		Vector3[] topVertices = new Vector3[3];
		topVertices [0] = new Vector3 (0, 0, 0) + vertexOffset;
		topVertices [1] = new Vector3 (2.0f / 3.0f, 0, 0) + vertexOffset;
		topVertices [2] = new Vector3 (1.0f / 3.0f, 0, vertexHeight * 2.0f / 3.0f) + vertexOffset;

		Vector2[] topUV = new Vector2[3];
		topUV [0] = new Vector2 (0, 0);
		topUV [1] = new Vector2 (2.0f / 3.0f,  0);
		topUV [2] = new Vector2 (1.0f / 3.0f, 2.0f / 3.0f);

		int[] topTriangles = new int[3];
		topTriangles[0] = 2;
		topTriangles[1] = 1;
		topTriangles[2] = 0;

		Vector3[] bottomVertices = new Vector3[4];
		bottomVertices [0] = new Vector3 (2.0f / 3.0f, 0, 0) + vertexOffset;
		bottomVertices [1] = new Vector3 (1, 0, 0) + vertexOffset;
		bottomVertices [2] = new Vector3 (0.5f, 0, vertexHeight) + vertexOffset;
		bottomVertices [3] = new Vector3 (1.0f / 3.0f, 0, vertexHeight * 2.0f / 3.0f) + vertexOffset;

		Vector2[] bottomUV = new Vector2[4];
		bottomUV [0] = new Vector2 (2.0f / 3.0f, 0);
		bottomUV [1] = new Vector2 (1, 0);
		bottomUV [2] = new Vector2 (0.5f,  1);
		bottomUV [3] = new Vector2 (1.0f / 3.0f, 2.0f / 3.0f);

		int[] bottomTriangles = new int[6];
		bottomTriangles[0] = 2;
		bottomTriangles[1] = 1;
		bottomTriangles[2] = 0;
		bottomTriangles[3] = 0;
		bottomTriangles[4] = 3;
		bottomTriangles[5] = 2;

		GameObject topFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);
		GameObject bottomFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);

		SplitFace topSplitFace = topFace.GetComponent<SplitFace> ();
		SplitFace bottomSplitFace = bottomFace.GetComponent<SplitFace> ();

		topSplitFace.Initialise (faceTex, topVertices, topUV, topTriangles);
		bottomSplitFace.Initialise (faceTex, bottomVertices, bottomUV, bottomTriangles);

		topSplitFace.transform.parent = transform;
		topSplitFace.transform.localPosition = Vector3.zero;
		topSplitFace.transform.localRotation = Quaternion.identity;

		bottomSplitFace.transform.parent = transform;
		bottomSplitFace.transform.localPosition = Vector3.zero;
		bottomSplitFace.transform.localRotation = Quaternion.identity;

		return new SplitFace[] { topSplitFace, bottomSplitFace };
	}

	public SplitFace[] SplitRightSmall() {
		Vector3[] topVertices = new Vector3[3];
		topVertices [0] = new Vector3 (2.0f / 3.0f, 0, 0) + vertexOffset;
		topVertices [1] = new Vector3 (1, 0, 0) + vertexOffset;
		topVertices [2] = new Vector3 (2.5f / 3.0f, 0, vertexHeight * 1.0f / 3.0f) + vertexOffset;

		Vector2[] topUV = new Vector2[3];
		topUV [0] = new Vector2 (2.0f / 3.0f, 0);
		topUV [1] = new Vector2 (1,  0);
		topUV [2] = new Vector2 (2.5f / 3.0f, 1.0f / 3.0f);

		int[] topTriangles = new int[3];
		topTriangles[0] = 2;
		topTriangles[1] = 1;
		topTriangles[2] = 0;

		Vector3[] bottomVertices = new Vector3[4];
		bottomVertices [0] = new Vector3 (0, 0, 0) + vertexOffset;
		bottomVertices [1] = new Vector3 (2.0f / 3.0f, 0, 0) + vertexOffset;
		bottomVertices [2] = new Vector3 (2.5f / 3.0f, 0, vertexHeight * 1.0f / 3.0f) + vertexOffset;
		bottomVertices [3] = new Vector3 (0.5f, 0, vertexHeight) + vertexOffset;

		Vector2[] bottomUV = new Vector2[4];
		bottomUV [0] = new Vector2 (0, 0);
		bottomUV [1] = new Vector2 (2.0f / 3.0f, 0);
		bottomUV [2] = new Vector2 (2.5f / 3.0f, 1.0f / 3.0f);
		bottomUV [3] = new Vector2 (0.5f, 1);

		int[] bottomTriangles = new int[6];
		bottomTriangles[0] = 2;
		bottomTriangles[1] = 1;
		bottomTriangles[2] = 0;
		bottomTriangles[3] = 0;
		bottomTriangles[4] = 3;
		bottomTriangles[5] = 2;

		GameObject topFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);
		GameObject bottomFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);

		SplitFace topSplitFace = topFace.GetComponent<SplitFace> ();
		SplitFace bottomSplitFace = bottomFace.GetComponent<SplitFace> ();

		topSplitFace.Initialise (faceTex, topVertices, topUV, topTriangles);
		bottomSplitFace.Initialise (faceTex, bottomVertices, bottomUV, bottomTriangles);

		topSplitFace.transform.parent = transform;
		topSplitFace.transform.localPosition = Vector3.zero;
		topSplitFace.transform.localRotation = Quaternion.identity;

		bottomSplitFace.transform.parent = transform;
		bottomSplitFace.transform.localPosition = Vector3.zero;
		bottomSplitFace.transform.localRotation = Quaternion.identity;

		return new SplitFace[] { topSplitFace, bottomSplitFace };
	}

	public SplitFace[] SplitRightLarge() {
		Vector3[] topVertices = new Vector3[3];
		topVertices [0] = new Vector3 (1.0f / 3.0f, 0, 0) + vertexOffset;
		topVertices [1] = new Vector3 (1, 0, 0) + vertexOffset;
		topVertices [2] = new Vector3 (2.0f / 3.0f, 0, vertexHeight * 2.0f / 3.0f) + vertexOffset;

		Vector2[] topUV = new Vector2[3];
		topUV [0] = new Vector2 (1.0f / 3.0f, 0);
		topUV [1] = new Vector2 (1,  0);
		topUV [2] = new Vector2 (2.0f / 3.0f, 2.0f / 3.0f);

		int[] topTriangles = new int[3];
		topTriangles[0] = 2;
		topTriangles[1] = 1;
		topTriangles[2] = 0;

		Vector3[] bottomVertices = new Vector3[4];
		bottomVertices [0] = new Vector3 (0, 0, 0) + vertexOffset;
		bottomVertices [1] = new Vector3 (1.0f / 3.0f, 0, 0) + vertexOffset;
		bottomVertices [2] = new Vector3 (2.0f / 3.0f, 0, vertexHeight * 2.0f / 3.0f) + vertexOffset;
		bottomVertices [3] = new Vector3 (0.5f, 0, vertexHeight) + vertexOffset;

		Vector2[] bottomUV = new Vector2[4];
		bottomUV [0] = new Vector2 (0, 0);
		bottomUV [1] = new Vector2 (1.0f / 3.0f, 0);
		bottomUV [2] = new Vector2 (2.0f / 3.0f, 2.0f / 3.0f);
		bottomUV [3] = new Vector2 (0.5f, 1);

		int[] bottomTriangles = new int[6];
		bottomTriangles[0] = 2;
		bottomTriangles[1] = 1;
		bottomTriangles[2] = 0;
		bottomTriangles[3] = 0;
		bottomTriangles[4] = 3;
		bottomTriangles[5] = 2;

		GameObject topFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);
		GameObject bottomFace = (GameObject)Instantiate (splitFace, Vector3.zero, Quaternion.identity);

		SplitFace topSplitFace = topFace.GetComponent<SplitFace> ();
		SplitFace bottomSplitFace = bottomFace.GetComponent<SplitFace> ();

		topSplitFace.Initialise (faceTex, topVertices, topUV, topTriangles);
		bottomSplitFace.Initialise (faceTex, bottomVertices, bottomUV, bottomTriangles);

		topSplitFace.transform.parent = transform;
		topSplitFace.transform.localPosition = Vector3.zero;
		topSplitFace.transform.localRotation = Quaternion.identity;

		bottomSplitFace.transform.parent = transform;
		bottomSplitFace.transform.localPosition = Vector3.zero;
		bottomSplitFace.transform.localRotation = Quaternion.identity;

		return new SplitFace[] { topSplitFace, bottomSplitFace };
	}
	void Update () {

	}

	void OnDestroy() {
		Destroy (GetComponent<MeshRenderer> ());
		Destroy (GetComponent<MeshFilter> ());
		Destroy (gameObject);
	}
}
