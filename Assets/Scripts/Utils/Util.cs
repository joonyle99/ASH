using UnityEngine;

namespace joonyle99
{
    /// <summary>
    /// 두 점을 담는 자료구조
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
        // 선 길이를 나타내는 상수
        private const float LINE_LENGTH = 1f;

        // 선의 방향을 나타내는 상수
        private static readonly Vector2 vec1 = new(-LINE_LENGTH, LINE_LENGTH);
        private static readonly Vector2 vec2 = new(LINE_LENGTH, -LINE_LENGTH);
        private static readonly Vector2 vec3 = new(-LINE_LENGTH, -LINE_LENGTH);
        private static readonly Vector2 vec4 = new(LINE_LENGTH, LINE_LENGTH);

        public static void DrawX(Vector2 center, float duration = 2f)
        {
            // 디버그를 위한 타겟 위치에 'X' 표시
            Debug.DrawLine(center + vec1, center + vec2, Color.red, duration);
            Debug.DrawLine(center + vec3, center + vec4, Color.red, duration);
        }
    }
}