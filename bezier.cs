using UnityEngine;
//reference: http://paulbourke.net/geometry/bezier/

public class bezier : MonoBehaviour
{

    private int NI = 5;
    private int NJ = 4;
    private int resolutionI = 10 * NI;
    private int resolutionJ = 10 * NJ;
    private int i, j, ki, kj;
    private double mui, muj, bi, bj;
    private Random rnd1;
    private Vector3 xyz;
    static public void Main(String[] args)
    {
        for (i = 0; i <= NI; i++)
        {
            for (j = 0; j <= NJ; j++)
            {
                inp[i][j].x = i;
                inp[i][j].y = j;
                inp[i][j].z = (random() % 10000) / 5000.0 - 1;
            }
        }

    }

    private void Start()
    {
        rnd1 = new Random(1111);
        xyz = new Vector3();
    }

    private void Update()
    {

    }

    double BezierBlend(int k, double mu, int n)
    {
        int nn, kn, nkn;
        double blend = 1;

        nn = n;
        kn = k;
        nkn = n - k;

        while (nn >= 1)
        {
            blend *= nn;
            nn--;
            if (kn > 1)
            {
                blend /= (double)kn;
                kn--;
            }
            if (nkn > 1)
            {
                blend /= (double)nkn;
                nkn--;
            }
        }
        if (k > 0)
            blend *= pow(mu, (double)k);
        if (n - k > 0)
            blend *= pow(1 - mu, (double)(n - k));

        return (blend);
    }
}