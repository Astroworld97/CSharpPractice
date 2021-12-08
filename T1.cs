using Random = UnityEngine.Random;
using System;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task1 : MonoBehaviour
{
	// Surface Grid Variables
	#region
	private MeshFilter _SurfMeshFilter;
	private MeshRenderer _SurfMeshRenderer;
	private Mesh _SurfMesh;
	private int _surfGridX, _surfGridZ;
	private Vector3[] _SurfVerts;
	#endregion

	// Control Points / Bezier Curve Variables
	#region 
	// Bezier Curve Variables

	private LineRenderer _BezierRenderer;
	private int _controlGridX, _controlGridZ;
	private Vector3[,] _ControlPoints;
	private Vector3[] _XControlPoints, _ZControlPoints;
	//	private int _numXPoints = 5;
	//	private int _numZPoints = 4;
	private Vector3[,] _ypos;

	private float BI;
	private float BJ;

	// Control Point Variables
	private List<GameObject> _ControlPointCubes;
	private LineRenderer _ControlPolygonRenderer;

	#endregion

	// Start is called before the first frame update
	void Start()
	{


		// Surface Grid Stuff
		#region 
		_SurfMeshFilter = gameObject.AddComponent<MeshFilter>();
		_SurfMeshRenderer = gameObject.AddComponent<MeshRenderer>();
		_surfGridX = 25;
		_surfGridZ = 25;
		#endregion

		// Bezier Curve Stuff
		#region 
		_ypos = new Vector3[_surfGridX, _surfGridZ];


		_controlGridX = 5;
		_controlGridZ = 4;
		_ControlPoints = new Vector3[_controlGridX, _controlGridZ];
		_ControlPointCubes = new List<GameObject>();
		_XControlPoints = new Vector3[_controlGridX];
		_ZControlPoints = new Vector3[_controlGridZ];
		_SurfVerts = new Vector3[_surfGridX * _surfGridZ];

		GenerateControlPoints();
		CalculateYPos();
		GenerateGrid();
		//_BezierRenderer = new GameObject().AddComponent<LineRenderer>();


		#endregion


	}

	private void OnDrawGizmos()
	{
		if (_SurfVerts != null)
		{
			for (int i = 0; i < _SurfVerts.Length; i++)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawSphere(_SurfVerts[i], 0.1f);
			}
		}

		if (_ControlPoints != null)
		{
			for (int x = 0; x < _controlGridX; x++)
			{

				for (int j = 0; j < _controlGridZ; j++)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawCube(_ControlPoints[x, j], new Vector3(1f, 1f, 1f));
				}
			}
		}

	}

	private void CalculateYPos()
	{


		for (int i = 0; i < _surfGridX; i++)
		{
			float s = i / (float)_surfGridX;
			for (int j = 0; j < _surfGridZ; j++)
			{
				float t = j / (float)_surfGridZ;



				for (int ki = 0; ki < _controlGridX; ki++)
				{
					BI = BezierBlend(ki, s, _controlGridX);

					for (int kj = 0; kj < _controlGridZ; kj++)
					{
						BJ = BezierBlend(kj, t, _controlGridZ);
						float y = BI * BJ * _ControlPoints[ki, kj].y;
						_ypos[i, j] += new Vector3(0f, y, 0f);
					}
				}
			}
		}

		#region 
		// for (int z = 0; z < _surfGridZ; z++)
		// {
		//     for (int x = 0; x < _surfGridX; x++)
		//     {

		//         float t = x / (float)_surfGridX;
		//         float s = z/(float)_surfGridZ;
		//         Vector3[] _XControlPoints = GetXControlPoints(_ControlPoints, x, 0);
		//         Vector3[] _ZControlPoints = GetZControlPoints(_ControlPoints, 0, z);
		//         _ypos[x, z] = CalculateBezierUsingDeCasteljau(t, _XControlPoints);
		//         _ypos[x, z] = CalculateBezierUsingDeCasteljau(s, _ZControlPoints);

		//     }
		// }



		// int p = 0;
		// for (int i = 0; i < _surfGridZ; i++)
		// {
		//     for (int j = 0; j < _surfGridX; j++)
		//     {
		//         float t = j / (float)_surfGridX;


		//         _ypos[p] = CalculateBezierUsingDeCasteljau(t, _ControlPoints);
		//         p++;
		//     }
		// }
		#endregion
	}

	private float BezierBlend(int k, float w, int n)
	{
		int nn = n;
		int kn = k;
		int nkn = n - k;

		float blend = 1;
		while (nn >= 1)
		{
			blend *= nn;
			nn--;
			if (kn > 1)
			{
				blend /= (float)kn;
				kn--;
			}
			if (nkn > 1)
			{
				blend /= (float)nkn;
				nkn--;
			}
		}
		if (k > 0)
		{
			blend *= Mathf.Pow(w, (float)k);
		}
		if (n - k > 0)
		{
			blend *= Mathf.Pow(1 - w, (float)(n - k));
		}
		return blend;
	}

	private void GenerateGrid()
	{
		_SurfMesh = new Mesh();
		_SurfMeshFilter.mesh = _SurfMesh;
		_SurfMesh.name = "Surface Grid";

		_SurfVerts = new Vector3[_surfGridX * _surfGridZ];

		int vertcount = 0;

		for (int i = 0; i < _surfGridZ; i++)
		{
			for (int j = 0; j < _surfGridX; j++)
			{
				_SurfVerts[vertcount] = new Vector3(j, _ypos[j, i].y, i);
				vertcount++;
			}
		}

		_SurfMesh.vertices = _SurfVerts;

		int[] triangles = new int[_surfGridX * _surfGridZ * 6];

		int ti = 0;
		for (int i = 0; i < _surfGridZ - 1; i++)
		{
			for (int j = 0; j < _surfGridX - 1; j++)
			{
				triangles[ti + 0] = j + i * _surfGridX;
				triangles[ti + 1] = j + i * _surfGridX + _surfGridX;
				triangles[ti + 2] = j + i * _surfGridX + 1;
				triangles[ti + 3] = j + i * _surfGridX + 1;
				triangles[ti + 4] = j + i * _surfGridX + _surfGridX;
				triangles[ti + 5] = j + i * _surfGridX + _surfGridX + 1;

				ti += 6;
			}
		}

		_SurfMesh.triangles = triangles;

		_SurfMesh.RecalculateNormals();

		Vector2[] uv = new Vector2[_SurfVerts.Length];

		int uvCount = 0;
		for (int i = 0; i < _surfGridZ; i++)
		{
			for (int j = 0; j < _surfGridX; j++)
			{
				uv[uvCount] = new Vector2((float)j / _surfGridX, (float)i / _surfGridZ);
				uvCount++;
			}
		}
		_SurfMesh.uv = uv;

		//_meshRenderer.material.color = new Color(1.0f, 0.5f, 0.0f);
		_SurfMeshRenderer.material = Resources.Load("myGridMaterial", typeof(Material)) as Material;

	}

	private void GenerateControlPoints()
	{
		int controlPointCount = 0;

		for (int z = 0; z < _controlGridZ; z++)
		{
			for (int x = 0; x < _controlGridX; x++)
			{
				_ControlPoints[x, z] = new Vector3(x * (_surfGridX / (_controlGridX - 1)), Random.Range(-10f, 10f), z * (_surfGridZ / (_controlGridZ - 1)));
				// _ControlPoints[controlPointCount] = new Vector3(x * (_surfGridX / (_controlGridX - 1)), z, z * (_surfGridZ / (_controlGridZ - 1)));
				controlPointCount++;
			}
		}
		// controlPointCount = 0;
		// for (int z = 0; z < _controlGridZ; z++)
		// {
		// 	for (int x = 0; x < _controlGridX; x++)
		// 	{
		// 		GameObject tempGO = Instantiate(Resources.Load("ControlPointCube"), _ControlPoints[x, z], Quaternion.identity) as GameObject;
		// 		tempGO.GetComponent<MeshRenderer>().material.color = Color.red;
		// 		_ControlPointCubes.Add(tempGO);
		// 		controlPointCount++;
		// 	}
		// }



		// Debug.Log(controlPointCount);
	}

	// private Vector3[] GetXControlPoints(Vector3[,] CP, int x, int z)
	// {
	//     // Debug.Log("X = " + x);
	//     // Debug.Log("Z = " + z);

	//     Vector3[] xCP = new Vector3[_controlGridX];

	//     for (x = 0; x < _controlGridX; x++)
	//     {
	//         xCP[x] = CP[x, z];
	//         Debug.Log(CP[x, z]);
	//     }
	//     return xCP;
	// }

	// private Vector3[] GetZControlPoints(Vector3[,] CP, int x, int z)
	// {
	//     Vector3[] zCP = new Vector3[_controlGridZ];
	//     for (z = 0; z < _controlGridZ; z++)
	//     {
	//         zCP[z] = CP[x, z];
	//     }
	//     return zCP;
	// }


	// private Vector3 CalculateBezierUsingDeCasteljau(float t, Vector3[] p)
	// {
	//     // Compute point on a Bezier Curve using deCasteljau algorithm
	//     // takes t values and the control points as inputs
	//     // Outputs a Vector3 point

	//     Vector3[] B = new Vector3[p.Length];

	//     for (int i = 0; i < p.Length; i++)
	//     {
	//         B[i] = p[i];
	//     }
	//     for (int k = 1; k < p.Length; k++)
	//         for (int i = 0; i < p.Length - k; i++)
	//         {
	//             B[i] = (1f - t) * B[i] + t * B[i + 1];
	//         }
	//     Debug.Log(B[0]);
	//     return B[0];
	// }



	// Update is called once per frame

}
