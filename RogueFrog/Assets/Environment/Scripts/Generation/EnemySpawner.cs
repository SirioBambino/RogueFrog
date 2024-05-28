using System.Collections.Generic;
using UnityEngine;

// Class that spawns a given amount of enemies
namespace RogueFrog.Environment.Scripts.Generation
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject enemy;
        public int spawnAmount;
        public bool respawnWhenDead;
        public float respawnTime;
        private float respawnTimer;
        private bool startTimer = false;

        private List<GameObject> aliveEnemies = new List<GameObject>();
        [SerializeField] private int aliveEnemiesCount => aliveEnemies.Count;

        private void Start()
        {
            while (aliveEnemiesCount < spawnAmount)
            {
                aliveEnemies.Add(Instantiate(enemy, transform.position, Quaternion.identity));
            }
            respawnTimer = respawnTime;
        }
    
        private void Update()
        {
            while (aliveEnemiesCount < spawnAmount && respawnWhenDead && respawnTimer <= 0.0f)
            {
                aliveEnemies.Add(Instantiate(enemy, transform.position, Quaternion.identity));
                respawnTimer = respawnTime;
                startTimer = false;
            }

            aliveEnemies.RemoveAll(enemy => enemy == null);
            if (aliveEnemiesCount < spawnAmount)
                startTimer = true;

            if (startTimer)
                respawnTimer -= Time.deltaTime;
        }
    }
}
