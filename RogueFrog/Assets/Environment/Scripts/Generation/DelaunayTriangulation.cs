using System;
using System.Collections.Generic;
using UnityEngine;

namespace RogueFrog.Algorithms
{
    // This class was adapted from this code https://github.com/isaiah497/The-Golden-Alpaca/blob/cc7ec40620615030f11b25d38f3eb8c07735579d/Assets/Scripts/MapGeneration/Delaunay2D.cs
    public class DelaunayTriangulation
    {
        public class Triangle : IEquatable<Triangle>
        {
            public Vector3 A { get; set; }
            public Vector3 B { get; set; }
            public Vector3 C { get; set; }
            public bool IsBad { get; set; }

            public Triangle(Vector3 a, Vector3 b, Vector3 c)
            {
                A = a;
                B = b;
                C = c;
            }

            public bool ContainsVertex(Vector3 v)
            {
                return Vector3.Distance(v, A) < 0.01f
                       || Vector3.Distance(v, B) < 0.01f
                       || Vector3.Distance(v, C) < 0.01f;
            }

            public bool CircumCircleContains(Vector3 v)
            {
                Vector3 a = A;
                Vector3 b = B;
                Vector3 c = C;

                float ab = a.sqrMagnitude;
                float cd = b.sqrMagnitude;
                float ef = c.sqrMagnitude;

                float circumX = (ab * (c.y - b.y) + cd * (a.y - c.y) + ef * (b.y - a.y)) / (a.x * (c.y - b.y) + b.x * (a.y - c.y) + c.x * (b.y - a.y));
                float circumY = (ab * (c.x - b.x) + cd * (a.x - c.x) + ef * (b.x - a.x)) / (a.y * (c.x - b.x) + b.y * (a.x - c.x) + c.y * (b.x - a.x));

                Vector3 circum = new Vector3(circumX / 2, circumY / 2);
                float circumRadius = Vector3.SqrMagnitude(a - circum);
                float dist = Vector3.SqrMagnitude(v - circum);
                return dist <= circumRadius;
            }

            public static bool operator ==(Triangle left, Triangle right)
            {
                return (left.A == right.A || left.A == right.B || left.A == right.C)
                       && (left.B == right.A || left.B == right.B || left.B == right.C)
                       && (left.C == right.A || left.C == right.B || left.C == right.C);
            }

            public static bool operator !=(Triangle left, Triangle right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj is Triangle t)
                {
                    return this == t;
                }

                return false;
            }

            public bool Equals(Triangle t)
            {
                return this == t;
            }

            public override int GetHashCode()
            {
                return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
            }
        }

        public List<Vector3> Vertices { get; private set; }
        public List<Edge> Edges { get; private set; }
        public List<Triangle> Triangles { get; private set; }

        DelaunayTriangulation()
        {
            Edges = new List<Edge>();
            Triangles = new List<Triangle>();
        }

        public static DelaunayTriangulation Triangulate(List<Vector3> vertices)
        {
            DelaunayTriangulation delaunay = new DelaunayTriangulation();
            delaunay.Vertices = new List<Vector3>(vertices);
            delaunay.Triangulate();

            return delaunay;
        }

        void Triangulate()
        {
            float minX = Vertices[0].x;
            float minY = Vertices[0].y;
            float maxX = minX;
            float maxY = minY;

            foreach (var vertex in Vertices)
            {
                if (vertex.x < minX) minX = vertex.x;
                if (vertex.x > maxX) maxX = vertex.x;
                if (vertex.y < minY) minY = vertex.y;
                if (vertex.y > maxY) maxY = vertex.y;
            }

            float dx = maxX - minX;
            float dy = maxY - minY;
            float deltaMax = Mathf.Max(dx, dy) * 2;

            Vector3 p1 = new Vector3(minX - 1, minY - 1, 0);
            Vector3 p2 = new Vector3(minX - 1, maxY + deltaMax, 0);
            Vector3 p3 = new Vector3(maxX + deltaMax, minY - 1, 0);

            Triangles.Add(new Triangle(p1, p2, p3));

            foreach (var vertex in Vertices)
            {
                List<Edge> polygon = new List<Edge>();

                foreach (var t in Triangles)
                {
                    if (t.CircumCircleContains(vertex))
                    {
                        t.IsBad = true;
                        polygon.Add(new Edge(t.A, t.B));
                        polygon.Add(new Edge(t.B, t.C));
                        polygon.Add(new Edge(t.C, t.A));
                    }
                }

                Triangles.RemoveAll((Triangle t) => t.IsBad);

                for (int i = 0; i < polygon.Count; i++)
                {
                    for (int j = i + 1; j < polygon.Count; j++)
                    {
                        if (Edge.AlmostEqual(polygon[i], polygon[j]))
                        {
                            polygon[i].IsBad = true;
                            polygon[j].IsBad = true;
                        }
                    }
                }

                polygon.RemoveAll((Edge e) => e.IsBad);

                foreach (var edge in polygon)
                {
                    Triangles.Add(new Triangle(edge.A, edge.B, vertex));
                }
            }

            Triangles.RemoveAll((Triangle t) => t.ContainsVertex(p1) || t.ContainsVertex(p2) || t.ContainsVertex(p3));

            HashSet<Edge> edgeSet = new HashSet<Edge>();

            foreach (var t in Triangles)
            {
                var ab = new Edge(t.A, t.B);
                var bc = new Edge(t.B, t.C);
                var ca = new Edge(t.C, t.A);

                if (edgeSet.Add(ab))
                {
                    Edges.Add(ab);
                }

                if (edgeSet.Add(bc))
                {
                    Edges.Add(bc);
                }

                if (edgeSet.Add(ca))
                {
                    Edges.Add(ca);
                }
            }
        }
    }
}