using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGrid : MonoBehaviour
{
	private MeshFilter _mfltr;
	private MeshRenderer _mrend;
	private Mesh _mesh;
	private int _gridX, _gridY;

	private Vector3[] _verts;


    // Start is called before the first frame update
    void Start()
    {
		_mfltr = gameObject.AddComponent<MeshFilter>();
		_mrend = gameObject.AddComponent<MeshRenderer>();

		_gridX = 5;
		_gridY = 3;

		GenerateGrid();

	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		if(_verts != null)
		{
			for (int i = 0; i < _verts.Length; i++)
			{
				Gizmos.DrawSphere(_verts[i], 0.1f);
			}
		}

	}


	private void GenerateGrid()
	{
		// Set up a mesh
		_mesh = new Mesh();
		_mfltr.mesh = _mesh;
		_mesh.name = "Procedural Grid";


		_verts = new Vector3[_gridX * _gridY];

		// Populate vertices
		int vertCount = 0;
		for(int i=0; i< _gridY; i++)
		{
			for(int j=0; j< _gridX; j++)
			{
				_verts[vertCount] = new Vector3(j, i);
				vertCount++;
			}
		}
		_mesh.vertices = _verts;

		// Populate triangles
		int[] triangles = new int[_gridX * _gridY * 6];

		// triangle is filled in clockwise order
		//(0, 1)
		// 2. & 5.-------6. (1, 1)
		// |\           |
		// | \          |
		// |  \         |
		// |   \        |
		// |    \       |
		// |     \      |
		// |      \     |
		// |       \    |
		// |        \   |
		// |         \  |
		// |          \ |
		// |           \|
		// 1. ----------\3. & 4.
		// (0, 0)         (1, 0)

		/*
		// Works but too many variables to keep track of
		int ti = 0;
		int vi = 0;
		for (int i=0; i<_gridY; i++)
		{
			for(int j=0; j<_gridX; j++)
			{
				triangles[ti] = vi;
				triangles[ti + 1] = triangles[ti + 4] =  vi + _gridX + 1;
				triangles[ti + 2] = triangles[ti + 3] =  vi + 1;
				triangles[ti + 5] = vi + _gridX + 2;
				vi++;
				ti += 6;
				//Debug.Log("ti: " + ti + ", vi: " + vi);
			}
			vi++;
		}
		*/

		int ti = 0;
		for (int i = 0; i < _gridY - 1; i++)
		{
			for (int j = 0; j < _gridX - 1; j++)
			{
				triangles[ti + 0] = j + i * _gridX;
				triangles[ti + 1] = j + i * _gridX + _gridX;
				triangles[ti + 2] = j + i * _gridX + 1;
				triangles[ti + 3] = j + i * _gridX + 1;
				triangles[ti + 4] = j + i * _gridX + _gridX;
				triangles[ti + 5] = j + i * _gridX + _gridX + 1;
				/*
				_statusMsg += "" + triangles[ti + 0].ToString() + ", "
								+ triangles[ti + 1].ToString() + ", "
								+ triangles[ti + 2].ToString() + ", "
								+ triangles[ti + 3].ToString() + ", "
								+ triangles[ti + 4].ToString() + ", "
								+ triangles[ti + 5].ToString() + "\n";
				*/
				ti += 6;
			}
		}
		_mesh.triangles = triangles;

		// Calculate normals
		_mesh.RecalculateNormals();

		// Calculate UV values
		// There will be as many uv values as there are vertices
		Vector2[] uv = new Vector2[_verts.Length];
		int uvCount = 0;
		for (int i = 0; i < _gridY; i++)
		{
			for (int j = 0; j < _gridX; j++)
			{
				uv[uvCount] = new Vector2((float) j / _gridX, (float) i / _gridY);
				uvCount++;
			}
		}
		_mesh.uv = uv;


		// Apply a material property to the mesh
		//_mrend.material.color = new Color(1.0f, 0.5f, 0.0f);
		_mrend.material = Resources.Load("myGridMaterial", typeof(Material)) as Material;



	}


	// Update is called once per frame
	void Update()
    {
        
    }
}
