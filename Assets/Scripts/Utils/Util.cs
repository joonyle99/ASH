using UnityEngine;

namespace joonyle99
{
    /// <summary>
    /// �� ���� ��� �ڷᱸ�� (3����)
    /// </summary>
    public struct Line3D
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public Line3D(Vector3 a, Vector3 b)
        {
            pointA = a;
            pointB = b;
        }

        public static Vector3 Lerp(Line3D line, float t)
        {
            t = Mathf.Clamp01(t);

            var pointA = line.pointA;
            var pointB = line.pointB;

            return new Vector3(pointA.x + (pointB.x - pointA.x) * t, pointA.y + (pointB.y - pointA.y) * t,
                pointA.z + (pointB.z - pointA.z) * t);
        }
    }

    /// <summary>
    /// �� ���� ��� �ڷᱸ�� (2����)
    /// </summary>
    public struct Line2D
    {
        public Vector2 pointA;
        public Vector2 pointB;

        public Line2D(Vector2 a, Vector2 b)
        {
            pointA = a;
            pointB = b;
        }

        public static Vector2 Lerp(Line2D line, float t)
        {
            t = Mathf.Clamp01(t);

            var pointA = line.pointA;
            var pointB = line.pointB;

            return new Vector2(pointA.x + (pointB.x - pointA.x) * t, pointA.y + (pointB.y - pointA.y) * t);
        }
    }

    public static class Util
    {
        // �� ���̸� ��Ÿ���� ���
        private const float LINE_LENGTH = 1f;

        // ���� ������ ��Ÿ���� ���
        private static readonly Vector2 vec1 = new(-LINE_LENGTH, LINE_LENGTH);
        private static readonly Vector2 vec2 = new(LINE_LENGTH, -LINE_LENGTH);
        private static readonly Vector2 vec3 = new(-LINE_LENGTH, -LINE_LENGTH);
        private static readonly Vector2 vec4 = new(LINE_LENGTH, LINE_LENGTH);

        public static void DrawX(Vector2 center, float duration = 2f)
        {
            // ����׸� ���� Ÿ�� ��ġ�� 'X' ǥ��
            Debug.DrawLine(center + vec1, center + vec2, Color.red, duration);
            Debug.DrawLine(center + vec3, center + vec4, Color.red, duration);
        }
    }
}