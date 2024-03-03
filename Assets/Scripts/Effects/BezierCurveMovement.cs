using UnityEngine;
public class BezierCurveMovement : MonoBehaviour
{
    public BezierCurvePath _path;
    public float _duration = 2f;

    private float t = 0f;

    void Update()
    {
        if (t <= 1f)
        {
            MoveOnBezierCurve();
        }
    }

    void MoveOnBezierCurve()
    {
        t += Time.deltaTime / _duration;

        // Ensure there are at least two control points
        if (_path.Length < 2)
        {
            Debug.LogError("There must be at least two control points.");
            return;
        }

        // Bezier curve equation
        Vector3 position = _path.CalculateBezierPoint(t);

        transform.position = position;
    }

}