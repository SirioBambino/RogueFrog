using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RogueFrog.Algorithms
{
    // This class was adapted from this code https://github.com/malt-r/cyberspace-game/blob/317dfbcb51bc4e39c52978eb6dd5ab6adc101e2f/Assets/Generator/Scripts/Prim.cs
    public static class Prim
    {
        public static List<Edge> MinimumSpanningTree(List<Edge> edges, Vector2 start, bool returnRemaining = false)
        {
            HashSet<Vector2> openSet = new HashSet<Vector2>();
            HashSet<Vector2> closedSet = new HashSet<Vector2>();

            foreach (var edge in edges)
            {
                openSet.Add(edge.A);
                openSet.Add(edge.B);
            }

            closedSet.Add(start);

            List<Edge> results = new List<Edge>();

            while (openSet.Count > 0)
            {
                bool chosen = false;
                Edge chosenEdge = null;
                float minWeight = float.PositiveInfinity;

                foreach (var edge in edges)
                {
                    int closedVertices = 0;
                    if (!closedSet.Contains(edge.A)) closedVertices++;
                    if (!closedSet.Contains(edge.B)) closedVertices++;
                    if (closedVertices != 1) continue;

                    var length = Vector3.Distance(edge.A, edge.B);

                    if (length < minWeight)
                    {
                        chosenEdge = edge;
                        chosen = true;
                        minWeight = length;
                    }
                }

                if (!chosen) break;
                results.Add(chosenEdge);
                openSet.Remove(chosenEdge.A);
                openSet.Remove(chosenEdge.B);
                closedSet.Add(chosenEdge.A);
                closedSet.Add(chosenEdge.B);
            }

            if (returnRemaining)
            {
                List<Edge> remaining = new List<Edge>(edges);
                remaining = remaining.Except(results).ToList();
                return remaining;
            }
            else return results;
        }
    }
}