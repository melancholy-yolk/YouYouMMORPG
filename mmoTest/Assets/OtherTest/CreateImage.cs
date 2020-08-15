using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateImage : MonoBehaviour 
{

	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	public Texture texture;
	public int triCount = 10;

	void Start () {
		meshFilter = this.GetComponent<MeshFilter>();
		meshRenderer = this.GetComponent<MeshRenderer>();
		CreateAImage();
	}

	void CreateAImage()
	{
		VertexHelper vp = new VertexHelper();
		vp.Clear();

		vp.AddVert(new Vector3(0, 0, 0), Color.red, new Vector2(0, 0));
		vp.AddVert(new Vector3(0, 1, 0), Color.green, new Vector2(0, 1));
		vp.AddVert(new Vector3(1, 1, 0), Color.blue, new Vector2(1, 1));
		vp.AddVert(new Vector3(1, 0, 0), Color.cyan, new Vector2(1, 0));

		vp.AddTriangle(0, 1, 2);
		vp.AddTriangle(2, 3, 0);

		Mesh mesh = new Mesh();
		vp.FillMesh(mesh);
		meshFilter.mesh = mesh;

		// meshRenderer.material.color = Color.red;
		meshRenderer.material.mainTexture = texture;
	}

    //void CreateCircle()
    //{
    //    VertexHelper toFill = new VertexHelper();
    //    toFill.Clear();
    //    toFill.AddVert(new Vector3(0, 0, 0), Color.white, new Vector2(0, 0));
    //    float theta = 2 * Mathf.PI / triCount;
    //}

}
