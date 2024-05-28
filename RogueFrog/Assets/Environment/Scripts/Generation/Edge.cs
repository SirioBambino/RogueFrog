using UnityEngine;

namespace RogueFrog.Algorithms
{
    // This class was adapted from this code https://github.com/isaiah497/The-Golden-Alpaca/blob/cc7ec40620615030f11b25d38f3eb8c07735579d/Assets/Scripts/MapGeneration/Delaunay2D.cs
    public class Edge
    {
        public Vector3 A { get; set; }
        public Vector3 B { get; set; }
        public bool IsBad { get; set; }

        public Edge(Vector3 a, Vector3 b)
        {
            A = a;
            B = b;
        }

        public static bool operator ==(Edge left, Edge right)
        {
            return (left.A == right.A || left.A == right.B)
                   && (left.B == right.A || left.B == right.B);
        }

        public static bool operator !=(Edge left, Edge right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Edge e)
            {
                return this == e;
            }

            return false;
        }

        public bool Equals(Edge e)
        {
            return this == e;
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode();
        }

        static bool AlmostEqual(float x, float y)
        {
            return Mathf.Abs(x - y) <= float.Epsilon * Mathf.Abs(x + y) * 2
                   || Mathf.Abs(x - y) < float.MinValue;
        }

        static bool AlmostEqual(Vector3 left, Vector3 right)
        {
            return AlmostEqual(left.x, right.x) && AlmostEqual(left.y, right.y);
        }

        public static bool AlmostEqual(Edge left, Edge right)
        {
            return AlmostEqual(left.A, right.A) && AlmostEqual(left.B, right.B)
                   || AlmostEqual(left.A, right.B) && AlmostEqual(left.B, right.A);
        }
    }
}