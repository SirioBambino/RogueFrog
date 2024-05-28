using System;
using System.Collections.Generic;
using UnityEngine;
using RogueFrog.Algorithms;

namespace RogueFrog.Environment.Scripts.Generation
{
    public static class WallGenerator
    {
        //Returns an hashset of positions next to floor tiles which are empty
        public static HashSet<Vector2Int> FindWallPositions(HashSet<Vector2Int> floorPositions)
        {
            HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

            foreach (Vector2Int position in floorPositions)
            {
                // Go through each neighbour and if it's empty add it to the list
                foreach (Vector2Int direction in Direction.CardinalDirectionsList)
                {
                    Vector2Int neighbourPosition = position + direction;
                    if (floorPositions.Contains(neighbourPosition) == false)
                        wallPositions.Add(neighbourPosition);
                }
            }
            return wallPositions;
        }

    
        public static HashSet<Column> FindColumnPositions(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> wallPositions)
        {
            HashSet<Column> columnPositions = new HashSet<Column>();

            foreach (Vector2Int wallPosition in wallPositions)
            {
                foreach (Vector2Int direction in Direction.CardinalDirectionsList)
                {
                    Vector2Int neighbourPosition = wallPosition + direction;

                    if (!floorPositions.Contains(neighbourPosition)) continue;

                    Vector2 columnAPosition, columnBPosition;
                    bool columnAIsCorner, columnBIsCorner;

                    // If wall is along x axis
                    if (direction.x == 0)
                    {
                        columnAPosition = new Vector2(neighbourPosition.x + 0.5f, neighbourPosition.y - direction.y / 2.0f);
                        columnBPosition = new Vector2(neighbourPosition.x - 0.5f, neighbourPosition.y - direction.y / 2.0f);

                        columnAIsCorner = IsCorner(new Vector2Int(1, 0), new Vector2Int(1, direction.y));
                        columnBIsCorner = IsCorner(new Vector2Int(-1, 0), new Vector2Int(-1, direction.y));
                    }
                    // If wall is along y axis
                    else
                    {
                        columnAPosition = new Vector2(neighbourPosition.x - direction.x / 2.0f, neighbourPosition.y + 0.5f);
                        columnBPosition = new Vector2(neighbourPosition.x - direction.x / 2.0f, neighbourPosition.y - 0.5f);

                        columnAIsCorner = IsCorner(new Vector2Int(0, 1), new Vector2Int(direction.x, 1));
                        columnBIsCorner = IsCorner(new Vector2Int(0, -1), new Vector2Int(direction.x, -1));
                    }
                    // Create columns and add them to hashset
                    Column columnA = new Column(columnAPosition, wallPosition, neighbourPosition, columnAIsCorner);
                    Column columnB = new Column(columnBPosition, wallPosition, neighbourPosition, columnBIsCorner);

                    columnPositions.Add(columnA);
                    columnPositions.Add(columnB);
                }

                bool IsCorner(Vector2Int wallOffset, Vector2Int floorOffset)
                {
                    Vector2Int columnBAdjacentWall = wallPosition + wallOffset;
                    Vector2Int columnBAdjacentFloor = wallPosition + floorOffset;
                    return !(wallPositions.Contains(columnBAdjacentWall) && floorPositions.Contains(columnBAdjacentFloor));
                }
            }
            return columnPositions;
        }
    }

// Struct that holds information about a column
    public struct Column : IEquatable<Column>
    {
        public Vector2 position { get; private set; }
        public Vector2Int wallPosition { get; private set; }
        public Vector2Int floorPosition { get; private set; }
        public bool isCorner { get; private set; }

        public Column(Vector2 position, Vector2Int wallPosition, Vector2Int floorPosition, bool isCorner)
        {
            this.position = position;
            this.wallPosition = wallPosition;
            this.floorPosition = floorPosition;
            this.isCorner = isCorner;
        }

        public bool Equals(Column other)
        {
            return position == other.position;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }
}