using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurvePath : MonoBehaviour
{
    [SerializeField] Transform[] _controlPoints;

    public Transform[] ControlPoints => _controlPoints;
    public int Length => _controlPoints.Length;

    private void Reset()
    {
        _controlPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            _controlPoints[i] = transform.GetChild(i);
    }
    public Vector3 CalculateBezierPoint(float t)
    {
        t = Mathf.Clamp01(t);
        int order = _controlPoints.Length - 1;

        Vector3 position = Vector3.zero;
        for (int i = 0; i < _controlPoints.Length; i++)
        {
            float binomial = BinomialCoefficient(order, i);
            float term = binomial * Mathf.Pow(1 - t, order - i) * Mathf.Pow(t, i);
            position += term * _controlPoints[i].position;
        }

        return position;
    }


    float BinomialCoefficient(int n, int k)
    {
        return Factorial(n) / (Factorial(k) * Factorial(n - k));
    }

    float Factorial(int n)
    {
        if (n <= 1)
            return 1;
        else
            return n * Factorial(n - 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (float f = 0; f < 1; f += 0.01f)
            Gizmos.DrawLine(CalculateBezierPoint(f), CalculateBezierPoint(f + 0.01f));

        for(int i=0; i<_controlPoints.Length; i++)
        {
            Gizmos.DrawSphere(_controlPoints[i].position, 0.1f);
        }
    }
}
