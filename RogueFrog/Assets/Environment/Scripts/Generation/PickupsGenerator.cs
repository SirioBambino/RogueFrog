using System.Collections.Generic;
using UnityEngine;

namespace RogueFrog.Environment.Scripts.Generation
{
    public static class PickupsGenerator
    {
        public static BoundsInt furthestRoom;

        // Generate random health pickups positions in each room
        public static HashSet<Vector2Int> GenerateHealthPickupsPositions(HashSet<Vector2Int> floorPositions, List<BoundsInt> roomList, HashSet<Vector2Int> occupiedPositions)
        {
            HashSet<Vector2Int> healthPositions = new HashSet<Vector2Int>();

            // Iterate through all the free floor tiles in each room
            foreach (BoundsInt room in roomList)
            {
                for (int x = room.xMin; x < room.xMax; x++)
                {
                    for (int y = room.yMin; y < room.yMax; y++)
                    {
                        Vector2Int tile = new Vector2Int(x, y);
                        if (floorPositions.Contains(tile) && !occupiedPositions.Contains(tile))
                        {
                            // 1% chance that a health pickup will spawn
                            if (Random.Range(0.0f, 1.0f) > 0.99f)
                            {
                                healthPositions.Add(tile);
                                occupiedPositions.Add(tile);
                                goto End;
                            }
                        }
                    }
                }
                End:;
            }

            return healthPositions;
        }

        // Find the furthest room away from the start room and choose it's center to be the card's position
        public static Vector2Int GenerateCardPosition(HashSet<Vector2Int> floorPositions, List<BoundsInt> roomList, BoundsInt startRoom)
        {
            furthestRoom = startRoom;

            foreach (BoundsInt room in roomList)
            {
                if (Vector3.Distance(room.center, startRoom.center) > Vector3.Distance(furthestRoom.center, startRoom.center))
                    furthestRoom = room;
            }

            Vector2Int cardPosition = new Vector2Int(Mathf.RoundToInt(furthestRoom.center.x), Mathf.RoundToInt(furthestRoom.center.y));

            return cardPosition;
        }
    }
}