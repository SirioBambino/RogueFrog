using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Class that instantiates and stores all active particle effects, then disposes of them when they are finished
namespace RogueFrog.Managers.Scripts
{
    public class ParticlesManager : MonoBehaviour
    {
        public static ParticlesManager instance;

        [SerializeField] private GameObject projectileHitWhite;
        [SerializeField] private GameObject projectileHitRed;
        [SerializeField] private GameObject enemyDeath;

        private List<GameObject> aliveParticles = new List<GameObject>();

        private void Awake()
        {
            if (instance == null) instance = this;
        }

        // Iterate through all active particles effects and if they are finished destroy them
        private void FixedUpdate()
        {
            if (aliveParticles.Count > 0)
            {
                foreach (GameObject particle in aliveParticles.ToList())
                {
                    if (!particle.GetComponent<ParticleSystem>().IsAlive())
                    {
                        aliveParticles.Remove(particle);
                        Destroy(particle);
                    }
                }
            }
        }

        public void SpawnProjectileHitWhite(Vector3 position)
        {
            GameObject particle = Instantiate(projectileHitWhite, position, Quaternion.Euler(-90, 0, 0));
            aliveParticles.Add(particle);
        }

        public void SpawnProjectileHitRed(Vector3 position)
        {
            GameObject particle = Instantiate(projectileHitRed, position, Quaternion.Euler(-90, 0, 0));
            aliveParticles.Add(particle);
        }

        public void SpawnEnemyDeath(Vector3 position)
        {
            GameObject particle = Instantiate(enemyDeath, position, Quaternion.Euler(-90, 0, 0));
            aliveParticles.Add(particle);
        }
    }
}
