using UnityEngine;

namespace joonyle99
{
    /// <summary>
    /// 두 점을 담는 자료구조 (3차원)
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
    /// 두 점을 담는 자료구조 (2차원)
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
        // 선 길이를 나타내는 상수
        private const float LINE_LENGTH = 1f;

        // 선의 방향을 나타내는 상수
        private static readonly Vector2 vec1 = new(-LINE_LENGTH, LINE_LENGTH);
        private static readonly Vector2 vec2 = new(LINE_LENGTH, -LINE_LENGTH);
        private static readonly Vector2 vec3 = new(-LINE_LENGTH, -LINE_LENGTH);
        private static readonly Vector2 vec4 = new(LINE_LENGTH, LINE_LENGTH);

        public static void DebugDrawX(Vector2 center, float duration = 2f)
        {
            // 디버그를 위한 타겟 위치에 'X' 표시
            Debug.DrawLine(center + vec1, center + vec2, Color.red, duration);
            Debug.DrawLine(center + vec3, center + vec4, Color.red, duration);
        }

        public static void GizmosDrawVerticalLine(Vector3 origin)
        {
            Gizmos.DrawLine(new Vector3(origin.x, origin.y + LINE_LENGTH, origin.z), new Vector3(origin.x, origin.y - LINE_LENGTH, origin.z));
        }

        // Extension Methods
        public static Vector2 ToVector2(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }
        public static Vector2 ToVector2XZ(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }
        public static Vector3 ToVector3(this Vector2 vec, float z = 0)
        {
            return new Vector3(vec.x, vec.y, z);
        }
        public static Vector3 ToVector3XZ(this Vector2 vec, float y = 0)
        {
            return new Vector3(vec.x, y, vec.y);
        }
    }
}