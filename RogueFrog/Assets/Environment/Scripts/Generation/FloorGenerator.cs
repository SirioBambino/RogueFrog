using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RogueFrog.Algorithms;

namespace RogueFrog.Environment.Scripts.Generation
{
    public static class FloorGenerator
    {
        public static List<BoundsInt> finalRoomList;

        public static List<Edge> minimumSpanningTree;

        public static HashSet<Vector2Int> GenerateFloor(Vector3Int startPosition, LevelParametersSO parameters)
        {
            finalRoomList = new List<BoundsInt>();

            // Create a list of rooms using binary space partitioning
            List<BoundsInt> baseRoomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning
            (new BoundsInt(startPosition, new Vector3Int(parameters.levelSize.x, parameters.levelSize.y, 0)), 
                parameters.minimumRoomSize.x, parameters.minimumRoomSize.y, parameters.maximumRoomSize.x, parameters.maximumRoomSize.y);

            int maximumRooms = (parameters.maximumRooms > baseRoomsList.Count) ? baseRoomsList.Count : parameters.maximumRooms;

            // Randomly select a number of rooms from the list and add it to a hashset
            for (int i = 0; i < maximumRooms; i++)
            {
                var randomRoom = baseRoomsList[Random.Range(0, baseRoomsList.Count)];
                finalRoomList.Add(randomRoom);
                baseRoomsList.Remove(randomRoom);
            }

            // Get the center of each room and add it to a list
            List<Vector2Int> roomCenters = new List<Vector2Int>();
            foreach (BoundsInt room in finalRoomList)
                roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));

            // Generate floor positions for the rooms
            HashSet<Vector2Int> floorPositions = GenerateRandomWalkRooms(finalRoomList, parameters);

            // Generate floor positions for the corridors and join them with the previous list
            HashSet<Vector2Int> corridors = ConnectRooms(roomCenters, parameters.addCorridorChance, parameters);
            floorPositions.UnionWith(corridors);

            // Smooth level out using a cellular automata algorithm
            floorPositions = ProceduralGenerationAlgorithms.CellularAutomata(floorPositions, parameters.levelSize.x, parameters.levelSize.y, parameters.steps, parameters.birthLimit, parameters.deathLimit);

            // Sharpen the edges of the level
            if (parameters.sharpen)
                SharpenMap(floorPositions);

            return floorPositions;
        }

        // Create rooms using the random walk algorithm
        private static HashSet<Vector2Int> GenerateRandomWalkRooms(List<BoundsInt> roomsList, LevelParametersSO parameters)
        {
            HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
            foreach (BoundsInt room in roomsList)
            {
                // Get the center of the room and use it as the starting point of random walk algorithm
                Vector2Int roomCenter = new Vector2Int(Mathf.RoundToInt(room.center.x), Mathf.RoundToInt(room.center.y));
                HashSet<Vector2Int> roomFloor = ProceduralGenerationAlgorithms.RandomWalk(roomCenter, parameters.iterations, parameters.walkLength);

                // Make sure the generated room is within the bounds
                foreach (Vector2Int position in roomFloor)
                {
                    if (position.x >= (room.xMin) && position.x <= (room.xMax) &&
                        position.y >= (room.yMin) && position.y <= (room.yMax))
                    {
                        floor.Add(position);
                    }
                }
            }
            return floor;
        }

        // Use Delaunay's triangulation and minimum spanning tree to connect rooms with corridors
        private static HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters, float addCorridorChance, LevelParametersSO parameters)
        {
            List<Vector3> points = new List<Vector3>();
            HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

            // Get the center of each room and add it to a list in Vector3 format
            foreach (Vector2Int point in roomCenters)
            {
                points.Add(new Vector3(point.x, point.y, 0));
            }

            // Create triangulation using the center points of each room
            DelaunayTriangulation roomTriangulation = DelaunayTriangulation.Triangulate(points);

            // Add every edge in triangulation to a list
            List<Edge> edges = new List<Edge>();
            foreach (Edge edge in roomTriangulation.Edges)
                edges.Add(edge);

            // Create minimum spanning tree from triangulation
            minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].A);

            // Get the edges that aren't part of the mst
            List<Edge> remaining = Prim.MinimumSpanningTree(edges, edges[0].A, true);

            // Sorts the left over edges by length in ascending order
            remaining = remaining.OrderBy(Edge => Vector3.Distance(Edge.A, Edge.B)).ToList();

            // Randomly adds a few edges back to the mst starting from the shortest ones
            int count = 0;
            foreach (Edge edge in remaining)
            {
                if (Random.Range(0.0f, 1.0f) < addCorridorChance)
                {
                    minimumSpanningTree.Add(remaining[count]);
                    ++count;
                }
            }

            // Create a corridor for each edge
            foreach (Edge edge in minimumSpanningTree)
            {
                HashSet<Vector2Int> newCorridor = new HashSet<Vector2Int>();

                // Choose between normal and random walk corridors
                if (parameters.randomWalkCorridors)
                {
                    newCorridor = CreateRandomWalkCorridor(new Vector2Int(Mathf.RoundToInt(edge.A.x), Mathf.RoundToInt(edge.A.y)),
                        new Vector2Int(Mathf.RoundToInt(edge.B.x), Mathf.RoundToInt(edge.B.y)), parameters);
                }
                else
                {
                    newCorridor = CreateCorridor(new Vector2Int(Mathf.RoundToInt(edge.A.x), Mathf.RoundToInt(edge.A.y)),
                        new Vector2Int(Mathf.RoundToInt(edge.B.x), Mathf.RoundToInt(edge.B.y)));
                }

                corridors.UnionWith(newCorridor);
            }

            return corridors;
        }

        // Create a corridor going from one point to another using a random walk
        public static HashSet<Vector2Int> CreateRandomWalkCorridor(Vector2Int start, Vector2Int destination, LevelParametersSO parameters)
        {
            // Create corridor using random walk algorithm
            HashSet<Vector2Int> corridor = ProceduralGenerationAlgorithms.RandomWalk(start, destination, 3);

            // Make corridor bigger using cellular automata algorithm
            corridor = ProceduralGenerationAlgorithms.CellularAutomata(corridor, parameters.levelSize.x, parameters.levelSize.y, 1, 2, 0);

            return corridor;
        }

        // Create a corridor going from one point to another in straight lines
        private static HashSet<Vector2Int> CreateCorridor(Vector2Int start, Vector2Int destination)
        {
            HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
            Vector2Int position = start;
            corridor.Add(position);

            // If y value is not equal move towards it
            while (position.y != destination.y)
            {
                if (destination.y > position.y) position += Vector2Int.up;
                else if (destination.y < position.y) position += Vector2Int.down;

                corridor.Add(position);

                // Makes the corridor wider by adding tiles around it
                foreach (Vector2Int direction in Direction.FullDirectionsList)
                    corridor.Add(position + direction);
            }
            // If x value is not equal move towards it
            while (position.x != destination.x)
            {
                if (destination.x > position.x) position += Vector2Int.right;
                else if (destination.x < position.x) position += Vector2Int.left;

                corridor.Add(position);

                // Makes the corridor wider by adding tiles around it
                foreach (Vector2Int direction in Direction.FullDirectionsList)
                    corridor.Add(position + direction);
            }
            return corridor;
        }

        // Makes the map's edges more blocky
        private static void SharpenMap(HashSet<Vector2Int> floorPositions)
        {
            foreach (Vector2Int floor in floorPositions.ToList())
            {
                string neighbours = Direction.FindNeighbours(floorPositions, floor, Direction.FullDirectionsList);

                switch (neighbours)
                {
                    case "11110001":
                        if (Random.value >= 0.5)
                            floorPositions.Add(floor + Vector2Int.down);
                        else
                            floorPositions.Add(floor + Vector2Int.left);
                        break;
                    case "01111100":
                        if (Random.value >= 0.5)
                            floorPositions.Add(floor + Vector2Int.up);
                        else
                            floorPositions.Add(floor + Vector2Int.left);
                        break;
                    case "00011111":
                        if (Random.value >= 0.5)
                            floorPositions.Add(floor + Vector2Int.up);
                        else
                            floorPositions.Add(floor + Vector2Int.right);
                        break;
                    case "11000111":
                        if (Random.value >= 0.5)
                            floorPositions.Add(floor + Vector2Int.down);
                        else
                            floorPositions.Add(floor + Vector2Int.right);
                        break;
                }
            }
        }
    }
}
