using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RogueFrog.Algorithms
{
    public static class ProceduralGenerationAlgorithms
    {
        enum splitOrientation
        {
            Vertical,
            Horizontal
        }

        public static HashSet<Vector2Int> RandomWalk(Vector2Int startPosition, int iterations, int walkLength)
        {
            List<Vector2Int> directionList = new List<Vector2Int>(Direction.CardinalDirectionsList);
            HashSet<Vector2Int> fullWalk = new HashSet<Vector2Int>();

            for (int i = 0; i < iterations; i++)
            {
                // Create new walker path and add starting position to it
                HashSet<Vector2Int> walker = new HashSet<Vector2Int> { startPosition };

                // Keep track of current and next position
                Vector2Int currentPosition = startPosition;
                Vector2Int nextPosition;

                // Generate path by choosing a random neighbour, adding it to the path and moving the walker to that position
                // Repeat until walk length is reached
                for (int j = 0; j < walkLength; j++)
                {
                    nextPosition = currentPosition + directionList[UnityEngine.Random.Range(0, directionList.Count)];
                    walker.Add(nextPosition);
                    currentPosition = nextPosition;
                }

                //Add walker path to the full walk 
                fullWalk.UnionWith(walker);
            }
            return fullWalk;
        }

        public static HashSet<Vector2Int> RandomWalk(Vector2Int startPosition, Vector2Int endPosition, int weightTowardsEnd)
        {
            List<Vector2Int> directionList = new List<Vector2Int>(Direction.CardinalDirectionsList);
            HashSet<Vector2Int> fullWalk = new HashSet<Vector2Int>();

            // Create new walker path and add starting position to it
            HashSet<Vector2Int> walker = new HashSet<Vector2Int> { startPosition };

            // Keep track of current, previous and next position
            Vector2Int currentPosition = startPosition;
            Vector2Int previousPosition = currentPosition;
            Vector2Int newPosition;

            // Loop until end position is reached
            while (currentPosition != endPosition)
            {
                // Reset direction list
                directionList = new List<Vector2Int>(Direction.CardinalDirectionsList);

                // Get list of directions that point towards the end position, then add them to the direction list 
                // multiplied by the weight
                List<Vector2Int> keyDirectionList = Direction.FindDirections(currentPosition, (Vector2Int)endPosition);
                directionList.AddRange(Enumerable.Repeat(keyDirectionList, weightTowardsEnd).SelectMany(t => t).ToList());

                // Remove directions that would lead to the previous position
                directionList.RemoveAll(item => item == previousPosition - currentPosition);

                // Choose a random neighbour, adding it to the path and moving the walker to that position
                newPosition = currentPosition + directionList[UnityEngine.Random.Range(0, directionList.Count)];
                walker.Add(newPosition);
                previousPosition = currentPosition;
                currentPosition = newPosition;
            }

            //Add walker path to the full walk 
            fullWalk.UnionWith(walker);

            return fullWalk;
        }

        public static HashSet<Vector2Int> CellularAutomata(HashSet<Vector2Int> startArray, int arrayWidth, int arrayHeight, int iterations, int birthLimit, int deathLimit)
        {
            if (iterations == 0) return startArray;

            HashSet<Vector2Int> oldArray = new HashSet<Vector2Int>(startArray);
            HashSet<Vector2Int> newArray = new HashSet<Vector2Int>();

            // Repeat for as many iterations are given
            for (int i = 0; i < iterations; i++)
            {
                newArray = new HashSet<Vector2Int>();

                // Loop through the 2D array
                for (int x = 0; x < arrayWidth; x++)
                {
                    for (int y = 0; y < arrayHeight; y++)
                    {
                        // Get the current tile and check how many neighbours are alive
                        Vector2Int tile = new Vector2Int(x, y);
                        string neighbours = Direction.FindNeighbours(oldArray, tile, Direction.FullDirectionsList);
                        int aliveNeighbours = neighbours.Count(c => c == '1');

                        // If tile is alive and the number of alive neighbours is above the death limit keep it alive
                        if (oldArray.Contains(tile))
                        {
                            if (aliveNeighbours > deathLimit)
                                newArray.Add(tile);
                        }
                        // If tile is dead and the number of alive neighbours is above the birth limit turn it alive
                        else
                        {
                            if (aliveNeighbours > birthLimit)
                                newArray.Add(tile);
                        }
                    }
                }
                oldArray = newArray;
            }
            return newArray;
        }

        public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minimumWidth, int minimumHeight, int maximumWidth, int maximumHeight)
        {
            Queue<BoundsInt> roomQueue = new Queue<BoundsInt>();
            List<BoundsInt> roomList = new List<BoundsInt>();
            BoundsInt room;

            roomQueue.Enqueue(spaceToSplit);
            
            while (roomQueue.Count > 0)
            {
                room = roomQueue.Dequeue();

                float ratio = (float)room.size.x / (float)room.size.y;

                if (ratio > 1.2)
                    Split(splitOrientation.Vertical);
                else if (ratio < 0.8)
                    Split(splitOrientation.Horizontal);
                else
                    Split((splitOrientation)Random.Range(0, 1));
            }
            
            return roomList;
            
            void Split(splitOrientation orientation)
            {
                BoundsInt subRoomA = new BoundsInt(Vector3Int.zero, Vector3Int.zero);
                BoundsInt subRoomB = new BoundsInt(Vector3Int.zero, Vector3Int.zero);

                if (orientation == splitOrientation.Vertical)
                {
                    int splitAlongXEdge = Mathf.RoundToInt(Random.Range((float)(room.size.x * 0.3), (float)(room.size.x * 0.7)));

                    Vector3Int subRoomAPosition = room.min;
                    Vector3Int subRoomASize = new Vector3Int(splitAlongXEdge, room.size.y, room.size.z);
                        
                    subRoomA = new BoundsInt(subRoomAPosition, subRoomASize);
                        
                    Vector3Int subRoomBPosition = new Vector3Int(room.min.x + splitAlongXEdge, room.min.y, room.min.z);
                    Vector3Int subRoomBSize = new Vector3Int(room.size.x - splitAlongXEdge, room.size.y, room.size.z);
                        
                    subRoomB = new BoundsInt(subRoomBPosition, subRoomBSize);
                }
                else if (orientation == splitOrientation.Horizontal)
                {
                    int splitAlongYEdge = Mathf.RoundToInt(Random.Range((float)(room.size.y * 0.3), (float)(room.size.y * 0.7)));

                    Vector3Int subRoomAPosition = room.min;
                    Vector3Int subRoomASize = new Vector3Int(room.size.x, splitAlongYEdge, room.size.z);
                        
                    subRoomA = new BoundsInt(subRoomAPosition, subRoomASize);
                        
                    Vector3Int subRoomBPosition = new Vector3Int(room.min.x, room.min.y + splitAlongYEdge, room.min.z);
                    Vector3Int subRoomBSize = new Vector3Int(room.size.x, room.size.y - splitAlongYEdge, room.size.z);
                        
                    subRoomB = new BoundsInt(subRoomBPosition, subRoomBSize);
                }

                bool subRoomAIsLargerThanMinimum = subRoomA.size.y >= minimumHeight && subRoomA.size.x >= minimumWidth;
                bool subRoomBIsLargerThanMinimum = subRoomB.size.y >= minimumHeight && subRoomB.size.x >= minimumWidth;
                bool roomIsLargerThanMaximum = room.size.x > maximumWidth || room.size.y > maximumHeight;

                if (subRoomAIsLargerThanMinimum && subRoomBIsLargerThanMinimum)
                {
                    roomQueue.Enqueue(subRoomA);
                    roomQueue.Enqueue(subRoomB);
                }
                else if (roomIsLargerThanMaximum)
                {
                    roomQueue.Enqueue(room);
                }
                else
                {
                    roomList.Add(room);
                }
            }
        }
    }
}