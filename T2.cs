using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task2 : MonoBehaviour
{
	private LineRenderer bezierLineRend;
	private LineRenderer bSplineLineRend;
	private int numberControlPoints = 4;
	private int numberPoints = 25;
	private Vector3[] controlPoints;
	private float[] BSplinePoints;

	//	private float[] BSplineCalcPoints;
	private List<GameObject> controlPointSpheres;

	private LineRenderer controlBezierPolygonLineRend;
	private LineRenderer controlBSplinePolygonLineRend;

	private Vector3[] pos;
	private Vector3[] posB;
	private float[] posBPoints;

	private int t = 4;
	private int n = 3;
	private int[] knots;


	// Start is called before the first frame update
	void Start()
	{
		pos = new Vector3[numberPoints];

		posB = new Vector3[numberPoints];
		knots = new int[n + t + 1];


		posBPoints = new float[numberPoints * 3];
		bezierLineRend = new GameObject().AddComponent<LineRenderer>();
		bezierLineRend.name = "Bezier Line";
		bezierLineRend.startWidth = 0.1f;
		bezierLineRend.endWidth = 0.1f;
		bezierLineRend.positionCount = numberPoints + 1;
		bezierLineRend.material.color = Color.yellow;

		controlBezierPolygonLineRend = new GameObject().AddComponent<LineRenderer>();
		controlBezierPolygonLineRend.startWidth = 0.01f;
		controlBezierPolygonLineRend.endWidth = 0.01f;
		controlBezierPolygonLineRend.material.color = Color.white;



		bSplineLineRend = new GameObject().AddComponent<LineRenderer>();
		bSplineLineRend.name = "B-Spline Line";
		bSplineLineRend.startWidth = 0.1f;
		bSplineLineRend.endWidth = 0.1f;
		bSplineLineRend.positionCount = numberPoints + 1;
		bSplineLineRend.material.color = Color.cyan;

		controlBSplinePolygonLineRend = new GameObject().AddComponent<LineRenderer>();
		controlBSplinePolygonLineRend.startWidth = 0.01f;
		controlBSplinePolygonLineRend.endWidth = 0.01f;
		controlBSplinePolygonLineRend.material.color = Color.white;

		controlPoints = new Vector3[numberControlPoints];
		BSplinePoints = new float[numberControlPoints * 3];
		controlPointSpheres = new List<GameObject>();
		CreateControlPoints();
		DrawBezier();
		DrawBSpline();
	}
	private void CreateControlPoints()
	{
		//int j = 0;
		controlPoints[0] = new Vector3(-0.5f, -0.5f, 0f);
		controlPoints[1] = new Vector3(-0.35f, 0.6f, 0f);
		controlPoints[2] = new Vector3(0.3f, -0.4f, 0f);
		controlPoints[3] = new Vector3(0.8f, 0.5f, 0f);

		for (int i = 0; i < controlPoints.Length; i++)
		{
			// BSplinePoints[j] = controlPoints[i].x;
			// BSplinePoints[j + 1] = controlPoints[i].y + 1;
			// BSplinePoints[j + 2] = controlPoints[i].z;
			//j += 3;
			GameObject tempGO = Instantiate(Resources.Load("ControlPointSphere"), controlPoints[i], Quaternion.identity) as GameObject;
			tempGO.GetComponent<MeshRenderer>().material.color = Color.red;
			controlPointSpheres.Add(tempGO);

		}
		controlBezierPolygonLineRend.positionCount = numberControlPoints + 1;
		controlBezierPolygonLineRend.SetPositions(controlPoints);
		controlBezierPolygonLineRend.SetPosition(numberControlPoints, controlPoints[numberControlPoints - 1]);

		controlBSplinePolygonLineRend.positionCount = numberControlPoints + 1;
		controlBSplinePolygonLineRend.SetPositions(controlPoints);
		controlBSplinePolygonLineRend.SetPosition(numberControlPoints, controlPoints[numberControlPoints - 1]);
	}

	private void DrawBSpline()
	{

		knots = SplineKnots(knots, n, t);
		SplineCurve(controlPoints, n, knots, t, posB, numberPoints);
		// for (int i = 0; i < posB.Length; i++)
		// {

		// 	Debug.Log("x" + i + "= " + posB[i].x);
		// 	Debug.Log("y" + i + "= " + posB[i].y);
		// 	Debug.Log("z" + i + "= " + posB[i].z);
		// }

		bSplineLineRend.SetPositions(posB);
		bSplineLineRend.SetPosition(numberPoints, controlPoints[numberControlPoints - 1]);

	}



	private Vector3 SplinePoint(int[] u, int n, int t, float v, Vector3[] control, Vector3 output)
	{
		int k;
		float b;

		output.x = 0;
		output.y = 0;
		output.z = 0;

		for (k = 0; k <= n; k++)
		{
			b = SplineBlend(k, t, u, v);
			output.x += control[k].x * b;
			output.y += control[k].y * b;
			output.z += control[k].z * b;
			// Debug.Log("b" + k + "= " + b);
			// Debug.Log("Point x" + k + "= " + output.x);
			// Debug.Log("Point y" + k + "= " + output.y);
			// Debug.Log("Point z" + k + "= " + output.z);
		}
		return output;
	}
	private float SplineBlend(int k, int t, int[] u, float v)
	{
		float value;
		if (t == 1)
		{
			if ((u[k] <= v) && (v < u[k + 1]))
			{
				value = 1;
			}
			else
			{
				value = 0;
			}
		}
		else
		{
			if ((u[k + t - 1] == u[k]) && (u[k + t] == u[k + 1]))
			{
				value = 0;
			}
			else if (u[k + t - 1] == u[k])
			{
				value = (u[k + t] - v) / (u[k + t] - u[k + 1]) * SplineBlend(k + 1, t - 1, u, v);
			}
			else if (u[k + t] == u[k + 1])
			{
				value = (v - u[k]) / (u[k + t - 1] - u[k]) * SplineBlend(k, t - 1, u, v);
			}
			else
			{
				value = (v - u[k]) / (u[k + t - 1] - u[k]) * SplineBlend(k, t - 1, u, v) + (u[k + t] - v) / (u[k + t] - u[k + 1]) * SplineBlend(k + 1, t - 1, u, v);
			}
		}
		return value;
	}
	private int[] SplineKnots(int[] u, int n, int t)
	{
		int j;

		for (j = 0; j <= n + t; j++)
		{
			if (j < t)
			{
				u[j] = 0;
			}
			else if (j <= n)
			{
				u[j] = j - t + 1;
			}
			else if (j > n)
			{
				u[j] = n - t + 2;
			}
		}
		return u;
	}
	private void SplineCurve(Vector3[] input, int n, int[] knots, int t, Vector3[] output, int res)
	{
		int i;
		float interval, increment;
		interval = 0;
		increment = (n - t + 2) / (float)(res - 1);
		for (i = 0; i < res; i++)
		{
			output[i] = SplinePoint(knots, n, t, interval, input, output[i]);
			// Debug.Log("Curve x" + i + "= " + output[i].x);
			// Debug.Log("Curve y" + i + "= " + output[i].y);
			// Debug.Log("Curve z" + i + "= " + output[i].z);

			interval += increment;
		}
		output[res - 1] = input[n];
	}

	// private void DrawBSpline()
	// {
	// 	BSpline(numberControlPoints, 3, numberPoints, BSplinePoints, posBPoints);
	// 	int j = 0;
	// 	for (int i = 0; i < posB.Length; i++)
	// 	{
	// 		posB[i].x = posBPoints[j];
	// 		posB[i].y = posBPoints[j + 1];
	// 		posB[i].z = posBPoints[j + 2];
	// 		j += 3;
	// 	}


	// 	bSplineLineRend.SetPositions(posB);
	// 	bSplineLineRend.SetPosition(numberPoints, controlPoints[numberControlPoints - 1]);
	// }

	private void DrawBezier()
	{
		for (int i = 0; i < numberPoints; i++)
		{
			float q = i / (float)numberPoints;
			pos[i] = CalculateBezier(q, controlPoints);
		}
		bezierLineRend.SetPositions(pos);
		bezierLineRend.SetPosition(numberPoints, controlPoints[numberControlPoints - 1]);
	}

	private Vector3 CalculateBezier(float t, Vector3[] p)
	{
		// Vector3 term1, term2, term3, term4;
		// term1 = Mathf.Pow((1 - t), 3f) * p[0];
		// term2 = 3f * Mathf.Pow((1 - t), 2f) * t * p[1];
		// term3 = 3f * Mathf.Pow(t, 2f) * (1 - t) * p[2];
		// term4 = Mathf.Pow(t, 3f) * p[3];
		// return term1 + term2 + term3 + term4;

		Vector3[] B = new Vector3[numberControlPoints];

		for (int i = 0; i < numberControlPoints; i++)
		{
			B[i] = p[i];
		}
		for (int k = 1; k < numberControlPoints; k++)
			for (int i = 0; i < numberControlPoints - k; i++)
			{
				B[i] = (1f - t) * B[i] + t * B[i + 1];
			}
		// Debug.Log(B[0]);
		return B[0];
	}

	// private void BSplineBasis(int c, float t, int npts, int[] x, float[] n)
	// {
	// 	int nplusc;
	// 	int i, k;
	// 	float d, e;
	// 	float[] temp = new float[36];

	// 	nplusc = npts + c;
	// 	for (i = 0; i < nplusc; i++)
	// 	{
	// 		if ((t >= x[i]) && (t < x[i + 1]))
	// 		{
	// 			temp[i] = 1;
	// 		}
	// 		else
	// 		{
	// 			temp[i] = 0;
	// 		}
	// 	}
	// 	for (k = 1; k < c; k++)
	// 	{
	// 		for (i = 0; i <= nplusc - k; i++)
	// 		{
	// 			if (temp[i] != 0)
	// 			{
	// 				d = ((t - x[i]) * temp[i]) / (x[i + k - 1] - x[i]);
	// 			}
	// 			else
	// 			{
	// 				d = 0;
	// 			}
	// 			if (temp[i + 1] != 0)
	// 			{
	// 				e = ((x[i + k] - t) * temp[i + 1]) / (x[i + k] - x[i + 1]);
	// 			}
	// 			else
	// 			{
	// 				e = 0;
	// 			}
	// 			temp[i] = d + e;
	// 		}
	// 	}
	// 	if (t == (float)x[nplusc])
	// 	{
	// 		temp[npts] = 1;
	// 	}
	// 	for (i = 0; i < npts; i++)
	// 	{
	// 		n[i] = temp[i];
	// 	}
	// }

	// private void Knot(int n, int c, int[] x)
	// {
	// 	int nplusc, nplus2, i;
	// 	nplusc = n + c;
	// 	nplus2 = n + 2;

	// 	x[0] = 0;
	// 	for (i = 1; i < nplusc; i++)
	// 	{
	// 		if ((i > c) && (i < nplus2))
	// 		{
	// 			x[i] = x[i - 1] + 1;
	// 		}
	// 		else
	// 		{
	// 			x[i] = x[i - 1];
	// 		}
	// 	}
	// }

	// private void BSpline(int npts, int k, int p1, float[] b, float[] p)
	// {
	// 	int i, j, icount, jcount;
	// 	int i1;
	// 	int[] x = new int[30];
	// 	int nplusc;

	// 	float step;
	// 	float t;
	// 	float[] nbasis = new float[20];
	// 	float temp;

	// 	nplusc = npts + k;

	// 	for (i = 0; i < npts; i++)
	// 	{
	// 		nbasis[i] = 0;
	// 	}
	// 	for (i = 0; i < nplusc; i++)
	// 	{
	// 		x[i] = 0;
	// 	}

	// 	Knot(npts, k, x);

	// 	for (i = 0; i < nplusc; i++)
	// 	{
	// 		Debug.Log("The knot vector is: " + x[i]);
	// 	}

	// 	icount = 0;
	// 	t = 0;
	// 	step = ((float)x[nplusc]) / ((float)(p1 - 1));
	// 	for (i1 = 0; i1 < p1; i1++)
	// 	{
	// 		if ((float)x[nplusc] - t < 5e-6)
	// 		{
	// 			t = (float)x[nplusc];
	// 		}
	// 		BSplineBasis(k, t, npts, x, nbasis);

	// 		for (j = 0; j < 3; j++)
	// 		{
	// 			jcount = j;
	// 			p[icount] = 0;
	// 			for (i = 0; i < npts; i++)
	// 			{
	// 				temp = nbasis[i] * b[jcount];
	// 				p[icount + j] = p[icount + j] + temp;
	// 				jcount += 3;
	// 			}
	// 		}
	// 		icount += 3;
	// 		t += step;
	// 	}


	// }

}
