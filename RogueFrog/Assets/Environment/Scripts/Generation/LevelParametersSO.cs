using UnityEngine;

namespace RogueFrog.Environment.Scripts.Generation
{
    [CreateAssetMenu(fileName = "LevelParameters_", menuName = "LevelGenerationData")]

    public class LevelParametersSO : ScriptableObject
    {
        [Header("Seed")]
        public string seed = "12345";
        public bool useRandomSeed = false;

        [Header("Level Parameters")]
        public Vector2Int minimumRoomSize = new Vector2Int(9, 9);
        public Vector2Int maximumRoomSize = new Vector2Int(25, 25);
        public Vector2Int levelSize = new Vector2Int(50, 50);
        public int levelScale = 4;
        public int maximumRooms = 7;

        [Header("Room Parameters")]
        public int iterations = 200;
        public int walkLength = 10;

        [Header("Level Smoothing")]
        public int steps = 2;
        public int birthLimit = 6;
        public int deathLimit = 3;

        [HideInInspector] public float addCorridorChance = 0.3f;
        [HideInInspector] public bool sharpen = true;
        [HideInInspector] public bool randomWalkCorridors = false;
    }
}
