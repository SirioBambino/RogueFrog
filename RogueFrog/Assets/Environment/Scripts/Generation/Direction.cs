using System;
using System.Collections.Generic;
using UnityEngine;

namespace RogueFrog.Algorithms
{
    public static class Direction
    {
        public static readonly List<Vector2Int> CardinalDirectionsList = new List<Vector2Int>
        {
            new Vector2Int(0, 1),  //Up
            new Vector2Int(1, 0),  //Right
            new Vector2Int(0, -1), //Down
            new Vector2Int(-1, 0)  //Left
        };

        public static readonly List<Vector2Int> OrdinalDirectionsList = new List<Vector2Int>
        {
            new Vector2Int(1, 1),   //Up-Right
            new Vector2Int(1, -1),  //Down-Right
            new Vector2Int(-1, -1), //Down-Left
            new Vector2Int(-1, 1)   //Up-Left
        };

        public static readonly List<Vector2Int> FullDirectionsList = new List<Vector2Int>
        {
            new Vector2Int(0, 1),   //Up
            new Vector2Int(1, 1),   //Up-Right
            new Vector2Int(1, 0),   //Right
            new Vector2Int(1, -1),  //Down-Right
            new Vector2Int(0, -1),  //Down
            new Vector2Int(-1, -1), //Down-Left
            new Vector2Int(-1, 0),  //Left
            new Vector2Int(-1, 1)   //Up-Left
        };
    
        // Creates and returns a string which represents the neighbours of a tile in a clockwise direction
        public static string FindNeighbours(HashSet<Vector2Int> array, Vector2Int position, List<Vector2Int> directionList)
        {
            string neighbours = "";

            foreach (Vector2Int direction in directionList)
            {
                Vector2Int neighbourPosition = position + direction;

                if (array.Contains(neighbourPosition))
                    neighbours += "1";
                else
                    neighbours += "0";
            }

            return neighbours;
        }

        // Return a list of directions that point towards the end position
        public static List<Vector2Int> FindDirections(Vector2Int start, Vector2Int end)
        {
            if (start == end) throw new ArgumentException("Start and end position must be different");

            List<Vector2Int> directionList = new List<Vector2Int>();
            Vector2Int difference = end - start;

            if (difference.x > 0)
                directionList.Add(new Vector2Int(1, 0));
            else if (difference.x < 0)
                directionList.Add(new Vector2Int(-1, 0));

            if (difference.y > 0)
                directionList.Add(new Vector2Int(0, 1));
            else if (difference.y < 0)
                directionList.Add(new Vector2Int(0, -1));

            return directionList;
        }
    }
}