using UnityEngine;

namespace RogueFrog.Environment.Scripts.Generation
{
    [CreateAssetMenu(fileName = "", menuName = "RoomObject")]

    public class RoomObjectSO : ScriptableObject
    {
        [Header("Object Data")]
        public RoomObject[] Objects;
        public int Size = 1;
        [Range(0.0f, 1.0f)]
        public float Frequency;

        [Header("Object Placement")]
        public bool Edge;
        public bool Inner;

        public Vector3 GetRandomVector(float limit, bool lockX = false, bool lockY = false, bool lockZ = false)
        {
            float x = lockX ? 0 : Random.Range(0, limit);
            float y = lockY ? 0 : Random.Range(0, limit);
            float z = lockZ ? 0 : Random.Range(0, limit);

            return new Vector3(x, y, z);
        }
    }

    [System.Serializable]
    public class RoomObject
    {
        public GameObject Object;

        public bool hasPhysics;

        [Header("Object Modifier")]
        public Vector3 Position;
        public bool RandomPosition;
        public bool lockXPosition, lockYPosition, lockZPosition;

        public Vector3 Rotation;
        public bool RandomRotation;
        public bool lockXRotation, lockYRotation, lockZRotation;
    }
}