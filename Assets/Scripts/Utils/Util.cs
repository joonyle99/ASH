using UnityEngine;

namespace joonyle99
{
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