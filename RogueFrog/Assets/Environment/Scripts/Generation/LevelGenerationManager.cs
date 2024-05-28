using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Audio;

namespace RogueFrog.Environment.Scripts.Generation
{
    public class LevelGenerationManager : MonoBehaviour
    {
        public static GameObject level;

        [SerializeField] private GameObject[] floorPrefabs;
        [SerializeField] private GameObject[] ceilingPrefabs;
        [SerializeField] private GameObject[] wallPrefabs;
        [SerializeField] private GameObject[] columnPrefabs;
        [SerializeField] private RoomObjectSO[] roomObjectsPrefabs;
        [SerializeField] private GameObject[] shelfPropsPrefabs;
        [SerializeField] private GameObject trapPrefab;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject healthPrefab;
        [SerializeField] private GameObject cardPrefab;

        [Space(10)]

        [SerializeField] private GameObject victoryScreen;

        [Space(10)]

        [SerializeField] private LevelParametersSO levelParameters;

        [SerializeField] private GameObject player;
        [SerializeField] private GameObject enemy;
        private BoundsInt startRoom;
        public static Vector3 spawnPoint;

        private Vector3Int startPosition = Vector3Int.zero;
        private GameObject floor, ceiling, walls, columns, roomObjects, traps, healthPickups;
        private static HashSet<Vector2Int> floorPositions, wallPositions, occupiedPositions;
        private HashSet<Vector2Int> trapPositions, healthPositions;
        private Vector2Int cardPosition;
        private HashSet<Column> columnPositions;
        private Dictionary<RoomObject, List<TransformData>> roomObjectsPositions;
        private List<BoundsInt> roomList;

        private NavMeshSurface navMesh;
        [SerializeField] private LayerMask navMeshLayers;

        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioClip healAudioClip, cardAudioClip;


        private void Start()
        {
            InitialiseGeneration();
        }

        // Sets the seed for the random number generator using the hashcode of a string
        public void SetSeed()
        {
            if (levelParameters.useRandomSeed)
            {
                levelParameters.seed = Random.Range(0, 99999).ToString();
                Random.InitState(levelParameters.seed.GetHashCode());
            }
            else
            {
                Random.InitState(levelParameters.seed.GetHashCode());
            }
        }

        public void InitialiseGeneration()
        {
            // Destroys previous level before generating new one
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }

            // Set seed before starting level generation
            SetSeed();

            // Reset the scale
            transform.localScale = Vector3.one;

            level = gameObject;

            GenerateLevel();

            // Scale the level and move the player to the correct spawn
            transform.localScale *= levelParameters.levelScale;
        
            navMesh.layerMask = navMeshLayers;
            navMesh.BuildNavMesh();

            Time.timeScale = 1.0f;

            SetPlayerSpawn();
        }

        private void GenerateLevel()
        {
            // Generate the floor, wall, columns positions
            floorPositions = FloorGenerator.GenerateFloor(startPosition, levelParameters);
            wallPositions = WallGenerator.FindWallPositions(floorPositions);
            columnPositions = WallGenerator.FindColumnPositions(floorPositions, wallPositions);
        
            // Get the list of rooms choose a random one and make it the start room
            roomList = new List<BoundsInt>(FloorGenerator.finalRoomList);
            startRoom = roomList[Random.Range(0, roomList.Count)];
        
            // Generate the card, room objects, traps and health positions
            cardPosition = PickupsGenerator.GenerateCardPosition(floorPositions, roomList, startRoom);
            roomObjectsPositions = RoomObjectsGenerator.GenerateRoomObjectsPositions(floorPositions, wallPositions, roomObjectsPrefabs, cardPosition);
            occupiedPositions = new HashSet<Vector2Int>(RoomObjectsGenerator.OccupiedPositions);
            trapPositions = TrapGenerator.GenerateTrapPositions(RoomObjectsGenerator.InnerFloorPositions, occupiedPositions, 0.075f);
            healthPositions = PickupsGenerator.GenerateHealthPickupsPositions(floorPositions, roomList, occupiedPositions);

            // Spawn the gameObjects at the correct positions and set them to the correct layer
            ceiling = new GameObject("Ceiling");
            ObjectSpawner.SpawnCeiling(floorPositions, ceiling, ceilingPrefabs);

            foreach (Transform child in ceiling.transform)
            {
                child.gameObject.layer = 8;
                child.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            floorPositions = new HashSet<Vector2Int>(floorPositions.Except(trapPositions));

            floor = new GameObject("Floor");
            ObjectSpawner.SpawnFloor(floorPositions, floor, floorPrefabs);

            foreach (Transform child in floor.transform)
            {
                child.gameObject.layer = 7;
                child.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            walls = new GameObject("Walls");
            ObjectSpawner.SpawnWalls(floorPositions, wallPositions, walls, wallPrefabs);

            foreach (Transform child in walls.transform)
            {
                child.gameObject.layer = 9;
                child.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            columns = new GameObject("Columns");
            ObjectSpawner.SpawnColumns(columnPositions, columns, columnPrefabs);

            foreach (Transform child in columns.transform)
            {
                child.gameObject.layer = 9;
                child.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            roomObjects = new GameObject("Objects");
            ObjectSpawner.SpawnRoomObjects(roomObjectsPositions, roomObjects);

            foreach (Transform child in roomObjects.transform)
            {
                child.gameObject.layer = 10;
            }

            traps = new GameObject("Traps");
            ObjectSpawner.SpawnTraps(trapPositions, traps, trapPrefab);

            foreach (Transform child in traps.transform)
            {
                child.gameObject.layer = 7;
            }

            healthPickups = new GameObject("Health");
            ObjectSpawner.SpawnHealth(healthPositions, healthPickups, healthPrefab, mixer, healAudioClip);

            ObjectSpawner.SpawnCard(cardPosition, level, cardPrefab, mixer, cardAudioClip);

            // Run physics simulation
            RunSimulation();

            foreach (Transform child in roomObjects.transform)
            {
                Destroy(child.gameObject.GetComponent<Rigidbody>());
            }

            // Add collider and computer class to all screens
            foreach (Transform child in roomObjects.transform)
            {
                if (child.name == "ScreenSmall(Clone)" || child.name == "ScreenTall(Clone)")
                {
                    CapsuleCollider collider = child.gameObject.AddComponent<CapsuleCollider>();
                    collider.radius = 1.5f;
                    collider.isTrigger = true;

                    Computer computer = child.gameObject.AddComponent<Computer>();
                    computer.victoryScreen = victoryScreen;
                }
            }

            // Populate shelves
            ShelfPopulator.PopulateShelves(roomObjects, shelfPropsPrefabs, levelParameters);

            // Spawn enemies
            SetEnemySpawners();

            floor.AddComponent(typeof(NavMeshSurface));
            navMesh = floor.GetComponent<NavMeshSurface>();

            // Set all objects as children of this game object
            floor.transform.parent = level.transform;
            ceiling.transform.parent = level.transform;
            walls.transform.parent = level.transform;
            columns.transform.parent = level.transform;
            roomObjects.transform.parent = level.transform;
            traps.transform.parent = level.transform;
            healthPickups.transform.parent = level.transform;
        }

        // Pick a random unoccupied tile in the start room and set it as the spawn point
        private void SetPlayerSpawn()
        {
            for (int x = startRoom.xMin; x < startRoom.xMax; x++)
            {
                for (int y = startRoom.yMin; y < startRoom.yMax; y++)
                {
                    Vector2Int tile = new Vector2Int(x, y);
                    if (floorPositions.Contains(tile) && !occupiedPositions.Contains(tile))
                    {
                        spawnPoint = new Vector3(tile.x, 0, tile.y) * levelParameters.levelScale;
                        if (Random.Range(0.0f, 1.0f) > 0.95f) return;
                    }
                }
            }
        }

        private void RunSimulation()
        {
            // Disable auto physics simulation
            Physics.autoSimulation = false;

            // Run simulation for fixed amount of iterations
            for (int i = 0; i < 100; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
            }

            // Enable auto physics simulation
            Physics.autoSimulation = true;
        }

        // Position enemy spawner in each room except the start and set its parameters
        private void SetEnemySpawners()
        {
            foreach (BoundsInt room in roomList)
            {
                if (room != startRoom)
                {
                    GameObject spawner = new GameObject("Enemy Spawner");
                    spawner.transform.parent = level.transform;
                    spawner.transform.position = new Vector3(room.center.x, 0, room.center.y);

                    EnemySpawner spawnerComponent = spawner.AddComponent<EnemySpawner>();
                    spawnerComponent.enemy = enemyPrefab;
                    spawnerComponent.spawnAmount = Random.Range(3, 5);
                    spawnerComponent.respawnWhenDead = false;
                }
            }
        }
    }
}