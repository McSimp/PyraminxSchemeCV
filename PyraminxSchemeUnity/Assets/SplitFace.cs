using UnityEngine;
using System.Collections;

public class SplitFace : MonoBehaviour {

	private Vector3[] vertices;
	private Vector3[] normals;
	private Vector2[] uv;
	private int[] triangles;
	private Mesh mesh;

	public void Initialise(Texture2D SplitTexture, Vector3[] vertices, Vector2[] uv, int[] triangles) {
		this.vertices = vertices;
		this.uv = uv;
		this.triangles = triangles;

		CreateMesh();
		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshRenderer>().material.SetTexture("_MainTex", SplitTexture);
	}
		
	void CreateMesh()
	{
		normals = new Vector3[vertices.Length];
		mesh = new Mesh();

		for (int i = 0; i < vertices.Length; i++) {
			normals [i] = new Vector3 (0, 1, 0);
		}

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.triangles = triangles;
		mesh.uv = uv;
	}

	void OnDestroy() {
		Destroy (GetComponent<MeshRenderer> ());
		Destroy (GetComponent<MeshFilter> ());
		Destroy (this.gameObject);
	}
}
