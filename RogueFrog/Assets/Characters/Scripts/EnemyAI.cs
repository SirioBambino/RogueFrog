using RogueFrog.Managers.Scripts;
using RogueFrog.UI.Scripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

// This class was adapted and changed from this video tutorial https://www.youtube.com/watch?v=UjkSFoLxesw&list=PLqzUG5Dfyi_YrSg4C-PqABgstM7_g1FLn&index=1
// Around 70% of code is new, the rest is the same as the original

namespace RogueFrog.Characters.Scripts
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(EnemyInfo))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private LayerMask floorMask, playerMask;
        [SerializeField] private bool playerInSightRange, playerInAttackRange;

        // Patrolling
        private Vector3 startPosition;
        public Vector3 walkPoint;
        public bool walkPointSet = false;

        // Attacking
        [SerializeField] private Transform projectilePrefab;
        [SerializeField] private Transform spawnProjectilePosition;
        private bool alreadyAttacked;
    
        // Flying
        private float flyingOffset;
        private float flyingSpeed;

        // Falling
        [SerializeField] private AnimationCurve fallCurve;
        private bool isFalling;
        private float fallCurvePoint;
        private float fallTime;

        // Audio
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioClip deathAudioClip;

        private NavMeshAgent agent;
        private Animator animator;
        private EnemyInfo enemyInfo;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            enemyInfo = GetComponent<EnemyInfo>();

            startPosition = transform.position;

            agent.speed = enemyInfo.MovementSpeed;

            enemyInfo.Health = enemyInfo.MaxHealth;
            enemyInfo.Ammo = enemyInfo.MaxAmmoInClip;

            // Change the speed and offset of sine wave randomly to get variation
            flyingOffset = Random.Range(-5.0f, 5.0f);
            flyingSpeed = Random.Range(0.5f, 1.5f);
        }

        private void Update()
        {
            if (!PauseMenu.isPaused && !player.GetComponent<PlayerInfo>().HasWon)
            {
                // If enemy is alive
                if (enemyInfo.Health > 0)
                {
                    // Use sine wave to change offset to make enemy fly up and down
                    agent.baseOffset = Mathf.Sin(Time.time * flyingSpeed + flyingOffset) * 2.0f + 2.0f;

                    // Check if player is in sight or attack range and perform appropriate action
                    playerInSightRange = Physics.CheckSphere(transform.position, enemyInfo.SightRange, playerMask);
                    playerInAttackRange = Physics.CheckSphere(transform.position, enemyInfo.AttackRange, playerMask);

                    if (!playerInSightRange && !playerInAttackRange) Patrol();
                    if (playerInSightRange && !playerInAttackRange) Chase();
                    if (playerInSightRange && playerInAttackRange) Attack();

                }
                // If enemy is dead
                else
                {
                    agent.isStopped = true;
                    animator.SetBool("IsDead", true);

                    // Use animation curve to reduce the offset until enemy reaches the ground
                    if (isFalling)
                    {
                        fallCurvePoint = fallCurve.Evaluate(fallTime * 0.1f);
                        fallTime += Time.deltaTime;
                        agent.baseOffset = Mathf.Lerp(agent.baseOffset, -0.35f, fallCurvePoint);
                    }
                }
            }
        }

        // Walk to random point and once reached repeat
        private void Patrol()
        {
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet)
                agent.SetDestination(walkPoint);

            Vector2 distanceToWalkPoint = new Vector2(transform.position.x - walkPoint.x, transform.position.z - walkPoint.z);

            if (distanceToWalkPoint.magnitude < 2.0f)
                walkPointSet = false;
        }

        // Get random point on XZ plane and make sure it's inside the level
        private void SearchWalkPoint()
        {
            float randomX = Random.Range(-enemyInfo.PatrolRange, enemyInfo.PatrolRange);
            float randomZ = Random.Range(-enemyInfo.PatrolRange, enemyInfo.PatrolRange);

            walkPoint = new Vector3(startPosition.x + randomX, transform.position.y, startPosition.z + randomZ);

            // Use raycast to check if point is above floor
            if (Physics.Raycast(walkPoint, -transform.up, 10.0f, floorMask))
                walkPointSet = true;
        }

        // Move towards the player
        private void Chase()
        {
            agent.SetDestination(player.position);
            var directionVector = (player.position - transform.position - new Vector3(0, 1, 0)).normalized;
            transform.forward = directionVector;
        }

        // Stop the enemy, look towards the player and shoot projectile
        private void Attack()
        {
            agent.SetDestination(transform.position);

            var directionVector = (player.position - transform.position - new Vector3(0, 1, 0)).normalized;

            transform.forward = directionVector;

            // If enemy isn't attacking play attack animation
            if (!alreadyAttacked)
            {
                animator.SetBool("IsShooting", true);
                animator.SetFloat("AttackSpeed", 1/enemyInfo.AttackSpeed);
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), enemyInfo.AttackSpeed);
            }
        }

        private void ResetAttack()
        {
            alreadyAttacked = false;
            animator.SetBool("IsShooting", false);
        }

        // Instantiate projectile towards forward direction and assign it the enemy's attack damage
        private void ShootProjectile(AnimationEvent animationEvent)
        {
            Transform projectile = Instantiate(projectilePrefab, spawnProjectilePosition.position, Quaternion.LookRotation(transform.forward, transform.up));
            projectile.gameObject.GetComponent<Projectile>().Damage = enemyInfo.AttackDamage;
        }

        private void FallToGround(AnimationEvent animationEvent)
        {
            fallTime = 0;
            isFalling = true;
        }

        private void Death(AnimationEvent animationEvent)
        {
            Invoke(nameof(DestroyEnemy), 1f);
        }

        // Play death audio clip, spawn particles and destroy gameobject 
        private void DestroyEnemy()
        {
            mixer.GetFloat("MainVolume", out float volume);
            AudioSource.PlayClipAtPoint(deathAudioClip, transform.position, Mathf.Pow(10, (volume / 20.0f)));

            ParticlesManager.instance.SpawnEnemyDeath(transform.position + Vector3.up * 0.5f);
            Destroy(gameObject);
        }
    }
}
