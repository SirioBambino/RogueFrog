using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RogueFrog.Algorithms;

namespace RogueFrog.Environment.Scripts.Generation
{
    public struct TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public TransformData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }

    public static class RoomObjectsGenerator
    {
        private static Dictionary<RoomObject, List<TransformData>> objectsPositions;
        private static HashSet<Vector2Int> floorPositions, wallPositions;

        public static HashSet<Vector2Int> OccupiedPositions { get; set; }
        public static HashSet<Vector2Int> EdgeFloorPositions { get; set; }
        public static HashSet<Vector2Int> InnerFloorPositions { get; set; }

        public static Dictionary<RoomObject, List<TransformData>> GenerateRoomObjectsPositions(HashSet<Vector2Int> FloorPositions, HashSet<Vector2Int> WallPositions, RoomObjectSO[] roomObjects, Vector2Int cardPosition)
        {
            objectsPositions = new Dictionary<RoomObject, List<TransformData>>();

            floorPositions = FloorPositions;
            wallPositions = WallPositions;
            OccupiedPositions = new HashSet<Vector2Int>() { cardPosition };

            // Find inner and edge floor positions
            EdgeFloorPositions = FindEdgeFloor(floorPositions, wallPositions);
            InnerFloorPositions = new HashSet<Vector2Int>(floorPositions.Except(EdgeFloorPositions));

            foreach (RoomObjectSO roomObject in roomObjects)
            {
                // If object placement is edge iterate through all edge floor positions
                if (roomObject.Edge)
                {
                    foreach (Vector2Int floorPosition in EdgeFloorPositions)
                    {
                        FindRoomObjectPosition(floorPosition, roomObject, false);
                    }
                }

                // If object placement is inner iterate through all inner floor positions
                if (roomObject.Inner)
                {
                    foreach (Vector2Int floorPosition in InnerFloorPositions)
                    {
                        FindRoomObjectPosition(floorPosition, roomObject, true);
                    }
                }
            }
            return objectsPositions;
        }

        // Find edge floor positions by checking which floor positions are adjacent to walls
        private static HashSet<Vector2Int> FindEdgeFloor(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> wallPositions)
        {
            HashSet<Vector2Int> edgeFloorPositions = new HashSet<Vector2Int>();

            foreach (Vector2Int wallPosition in wallPositions)
            {
                foreach (Vector2Int direction in Direction.CardinalDirectionsList)
                {
                    Vector2Int neighbourPosition = wallPosition + direction;
                    if (floorPositions.Contains(neighbourPosition))
                    {
                        edgeFloorPositions.Add(neighbourPosition);
                    }
                }
            }

            return edgeFloorPositions;
        }

        // Check if either tile to the side is valid so that the object can be placed
        private static Vector3 FindBigObjectPosition(Vector2Int floorPosition, Vector2Int rightPosition, Vector2Int leftPosition, Vector2Int direction, Vector3 offset, bool isInner)
        {
            Vector3 spawnPosition = Vector3.zero;

            bool isRightValid, isLeftValid;

            if (isInner)
            {
                isRightValid = floorPositions.Contains(rightPosition) && !OccupiedPositions.Contains(rightPosition) && !wallPositions.Contains(rightPosition + direction);
                isLeftValid = floorPositions.Contains(leftPosition) && !OccupiedPositions.Contains(leftPosition) && !wallPositions.Contains(leftPosition + direction);
            }
            else
            {
                isRightValid = floorPositions.Contains(rightPosition) && !OccupiedPositions.Contains(rightPosition) && wallPositions.Contains(rightPosition + direction);
                isLeftValid = floorPositions.Contains(leftPosition) && !OccupiedPositions.Contains(leftPosition) && wallPositions.Contains(leftPosition + direction);
            }


            if (isRightValid)
            {
                spawnPosition = new Vector3(rightPosition.x, 0, rightPosition.y) - offset;
                OccupiedPositions.Add(floorPosition);
                OccupiedPositions.Add(rightPosition);
            }
            else if (isLeftValid)
            {
                spawnPosition = new Vector3(leftPosition.x, 0, leftPosition.y) + offset;
                OccupiedPositions.Add(floorPosition);
                OccupiedPositions.Add(leftPosition);
            }

            return spawnPosition;
        }

        private static void FindRoomObjectPosition(Vector2Int floorPosition, RoomObjectSO roomObject, bool isInner)
        {
            // If position is not occupied use object frequency and a random number to calculate chance to spawn object
            if (!OccupiedPositions.Contains(floorPosition))
            {
                float chanceToSpawn = Random.Range(0f, 1f);
                if (chanceToSpawn < roomObject.Frequency * 0.1f)
                {
                    // Iterate through cardinal directions to see which which side the wall is located and get the accurate rotation for the object
                    foreach (Vector2Int direction in Direction.CardinalDirectionsList)
                    {
                        Vector2Int neighbourPosition = floorPosition + direction;
                        if (!floorPositions.Contains(neighbourPosition) || isInner)
                        {
                            Vector3 spawnPosition = Vector3.zero;
                            Vector3 forwardDirection = new Vector3(floorPosition.x - neighbourPosition.x, 0, floorPosition.y - neighbourPosition.y);

                            // If object is 2 tiles big check if either tile to the side is valid so that the object can be placed
                            if (roomObject.Size == 2)
                            {
                                // If wall is on the z axis
                                if (direction.x == 0)
                                {
                                    Vector2Int rightPosition = new Vector2Int(neighbourPosition.x + 1, neighbourPosition.y - direction.y);
                                    Vector2Int leftPosition = new Vector2Int(neighbourPosition.x - 1, neighbourPosition.y - direction.y);

                                    spawnPosition = FindBigObjectPosition(floorPosition, rightPosition, leftPosition, direction, new Vector3(0.5f, 0, 0), isInner);
                                }
                                // If wall is on the x axis
                                else
                                {
                                    Vector2Int rightPosition = new Vector2Int(neighbourPosition.x - direction.x, neighbourPosition.y + 1);
                                    Vector2Int leftPosition = new Vector2Int(neighbourPosition.x - direction.x, neighbourPosition.y - 1);

                                    spawnPosition = FindBigObjectPosition(floorPosition, rightPosition, leftPosition, direction, new Vector3(0, 0, 0.5f), isInner);
                                }
                            }
                            else
                            {
                                spawnPosition = new Vector3(floorPosition.x, 0, floorPosition.y);
                                OccupiedPositions.Add(floorPosition);
                            }

                            // If no valid spawn position found, break out the loop
                            if (spawnPosition == Vector3.zero) break;

                            // Apply modifiers to each sub object of room object
                            foreach (RoomObject subObject in roomObject.Objects)
                            {
                                Vector3 positionnModifier = Vector3.zero;
                                if (subObject.RandomPosition)
                                    positionnModifier = roomObject.GetRandomVector(1, subObject.lockXPosition, subObject.lockYPosition, subObject.lockZPosition);
                                else
                                {
                                    switch (direction)
                                    {
                                        case Vector2Int v when v.Equals(Vector2Int.up):
                                            positionnModifier = new Vector3(subObject.Position.x, subObject.Position.y, subObject.Position.z);
                                            break;
                                        case Vector2Int v when v.Equals(Vector2Int.right):
                                            positionnModifier = new Vector3(subObject.Position.z, subObject.Position.y, subObject.Position.x);
                                            break;
                                        case Vector2Int v when v.Equals(Vector2Int.down):
                                            positionnModifier = new Vector3(subObject.Position.x, subObject.Position.y, -subObject.Position.z);
                                            break;
                                        case Vector2Int v when v.Equals(Vector2Int.left):
                                            positionnModifier = new Vector3(-subObject.Position.z, subObject.Position.y, subObject.Position.x);
                                            break;
                                    }
                                }

                                Vector3 rotationModifier = Vector3.zero;
                                if (subObject.RandomRotation)
                                    rotationModifier += roomObject.GetRandomVector(360, subObject.lockXRotation, subObject.lockYRotation, subObject.lockZRotation);
                                else
                                    rotationModifier += subObject.Rotation;

                                // Calculate final sub object position and rotation
                                Vector3 objectPosition = spawnPosition + positionnModifier;
                                Quaternion objectRotation = Quaternion.LookRotation(-forwardDirection, Vector3.up) * Quaternion.Euler(rotationModifier.x, rotationModifier.y, rotationModifier.z);

                                // If sub object is in dictionary add new transform data to the list otherwise add sub object and create list
                                if (objectsPositions.ContainsKey(subObject))
                                {
                                    objectsPositions[subObject].Add(new TransformData(objectPosition, objectRotation));
                                }
                                else
                                {
                                    objectsPositions.Add(subObject, new List<TransformData> { new TransformData(objectPosition, objectRotation) });
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}