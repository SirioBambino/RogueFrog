using System.Collections.Generic;
using UnityEngine;

// Class that generates random trap positions across the level
namespace RogueFrog.Environment.Scripts.Generation
{
    public static class TrapGenerator
    {
        public static HashSet<Vector2Int> GenerateTrapPositions(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> occupiedPositions, float trapChance)
        {
            HashSet<Vector2Int> trapPositions = new HashSet<Vector2Int>();

            // Iterate through all the free floor tiles
            foreach (Vector2Int floorPosition in floorPositions)
            {
                if (!occupiedPositions.Contains(floorPosition))
                {
                    // Use trap chance to randomly add traps
                    float chanceToSpawn = Random.Range(0f, 1f);
                    if (chanceToSpawn < trapChance)
                    {
                        trapPositions.Add(floorPosition);
                        occupiedPositions.Add(floorPosition);
                    }
                }
            }

            return trapPositions;
        }
    }
}
