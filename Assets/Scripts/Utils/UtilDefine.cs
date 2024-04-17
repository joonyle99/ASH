using UnityEngine;

/// <summary>
/// �� ���� ��� �ڷᱸ��
/// </summary>
public struct Line
{
    public Vector3 pointA;
    public Vector3 pointB;

    public Line(Vector3 a, Vector3 b)
    {
        pointA = a;
        pointB = b;
    }
}

public static class UtilDefine
{
    public static Vector3 PaddingVector = new Vector3(0.1f, 0f, 0f);

}