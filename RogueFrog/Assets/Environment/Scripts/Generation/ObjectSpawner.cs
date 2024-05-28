using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using RogueFrog.Algorithms;

// Class used to instantiate all the objects in the level
namespace RogueFrog.Environment.Scripts.Generation
{
    public class ObjectSpawner : MonoBehaviour
    {
        // Uses array of floor positions to instantiate floor tiles
        public static void SpawnFloor(HashSet<Vector2Int> floorPositions, GameObject parent, GameObject[] floor)
        {
            foreach (Vector2Int floorPosition in floorPositions)
            {
                Vector3 spawnPosition = new Vector3(floorPosition.x, 0.0f, floorPosition.y);
                GameObject floorObject = Instantiate(floor[Random.Range(0, floor.Length)], spawnPosition, Quaternion.identity, parent.transform);
                floorObject.AddComponent(typeof(BoxCollider));
            }
        }

        // Uses array of floor positions to instantiate ceiling tiles
        public static void SpawnCeiling(HashSet<Vector2Int> floorPositions, GameObject parent, GameObject[] ceiling)
        {
            foreach (Vector2Int floorPosition in floorPositions)
            {
                Vector3 spawnPosition = new Vector3(floorPosition.x, 2.2f, floorPosition.y);
                GameObject ceilingObject = Instantiate(ceiling[Random.Range(0, ceiling.Length)], spawnPosition, Quaternion.identity, parent.transform);
                ceilingObject.AddComponent(typeof(BoxCollider));
            }
        }

        // Uses array of walls positions to instantiate walls tiles
        public static void SpawnWalls(HashSet<Vector2Int> wallPositions, HashSet<Vector2Int> floorPositions, GameObject parent, GameObject[] walls)
        {
            foreach (Vector2Int wallPosition in wallPositions)
            {
                foreach (Vector2Int direction in Direction.CardinalDirectionsList)
                {
                    Vector2Int neighbourPosition = wallPosition + direction;
                    if (floorPositions.Contains(neighbourPosition))
                    {
                        // Calculate the correct position and rotation the wall
                        Vector3 spawnPosition = new Vector3(wallPosition.x + direction.x / 2.0f, 0, wallPosition.y + direction.y / 2.0f);
                        Vector3 forwardDirection = new Vector3(neighbourPosition.x - wallPosition.x, 0, neighbourPosition.y - wallPosition.y);

                        GameObject wallObject = Instantiate(walls[Random.Range(0, walls.Length)], spawnPosition, Quaternion.LookRotation(-forwardDirection, Vector3.up), parent.transform);
                        wallObject.AddComponent(typeof(MeshCollider));
                    }
                }
            }
        }

        // Uses array of column positions to instantiate column tiles
        public static void SpawnColumns(HashSet<Column> columnPositions, GameObject parent, GameObject[] columns)
        {
            foreach (Column columnPosition in columnPositions)
            {
                Vector3 spawnPosition = new Vector3(columnPosition.position.x, 0.0f, columnPosition.position.y);
                Vector3 forwardDirection = new Vector3(columnPosition.floorPosition.x - columnPosition.wallPosition.x, 0, columnPosition.floorPosition.y - columnPosition.wallPosition.y);

                GameObject columnObject;
                if (columnPosition.isCorner)
                {
                    columnObject = Instantiate(columns[Random.Range(1, columns.Length)], spawnPosition, Quaternion.LookRotation(forwardDirection, Vector3.up), parent.transform);
                    columnObject.AddComponent(typeof(BoxCollider));
                }
            }
        }

        // Uses dictionary of objects to instantiate them
        public static void SpawnRoomObjects(Dictionary<RoomObject, List<TransformData>> objectsPositions, GameObject parent)
        {
            foreach (KeyValuePair<RoomObject, List<TransformData>> roomObject in objectsPositions)
            {
                foreach (TransformData transformData in roomObject.Value)
                {
                    GameObject Object = Instantiate(roomObject.Key.Object, transformData.Position, transformData.Rotation, parent.transform);
                    Object.AddComponent(typeof(MeshCollider));
                    Object.GetComponent<MeshCollider>().convex = true;
                    Object.AddComponent(typeof(Rigidbody));
                }
            }
        }

        // Uses array of trap positions to instantiate trap tiles
        public static void SpawnTraps(HashSet<Vector2Int> trapPositions, GameObject parent, GameObject trap)
        {
            foreach (Vector2Int floorPosition in trapPositions)
            {
                Vector3 spawnPosition = new Vector3(floorPosition.x, 0.0f, floorPosition.y);
                GameObject trapObject = Instantiate(trap, spawnPosition, Quaternion.identity, parent.transform);
            }
        }

        // Uses array of health positions to instantiate health pickups
        public static void SpawnHealth(HashSet<Vector2Int> healthPositions, GameObject parent, GameObject healthPickup, AudioMixer mixer, AudioClip healAudioClip)
        {
            foreach (Vector2Int healthPosition in healthPositions)
            {
                Vector3 spawnPosition = new Vector3(healthPosition.x, 0, healthPosition.y);
                GameObject healthObject = Instantiate(healthPickup, spawnPosition, Quaternion.identity, parent.transform);
            }
        }

        // Spawns the card
        public static void SpawnCard(Vector2Int cardPosition, GameObject parent, GameObject card, AudioMixer mixer, AudioClip cardAudioClip)
        {
            Vector3 spawnPosition = new Vector3(cardPosition.x, 0, cardPosition.y);
            GameObject cardObject = Instantiate(card, spawnPosition, Quaternion.identity, parent.transform);
        }
    }
}
