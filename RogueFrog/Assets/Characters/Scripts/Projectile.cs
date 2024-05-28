using RogueFrog.Managers.Scripts;
using UnityEngine;
using UnityEngine.Audio;

// Class that handles the projectile's logic
namespace RogueFrog.Characters.Scripts
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private float speed;

        [Header("Audio")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioClip impactAudioClip;

        private Rigidbody bulletRigidbody;

        public int Damage;
    
        private void Awake()
        {
            bulletRigidbody = GetComponent<Rigidbody>();
        }

        // On start move forward
        private void Start()
        {
            bulletRigidbody.velocity = transform.forward * speed;
        }

        // When colliding with another object deal damage and destroy itself
        private void OnTriggerEnter(Collider other)
        {
            // Execute only if other is not a trigger or another projectile
            if (!other.isTrigger && !other.CompareTag("Projectile"))
            {
                // If character is hit deal damage
                if (other.GetComponent<CharacterInfo>())
                    other.GetComponent<CharacterInfo>().Health -= Damage;

                // Instantiate different particles effects
                if (gameObject.layer == 6)
                    ParticlesManager.instance.SpawnProjectileHitRed(transform.position);
                else
                {
                    if (other.GetComponent<EnemyInfo>())
                        ParticlesManager.instance.SpawnProjectileHitRed(transform.position);
                    else
                        ParticlesManager.instance.SpawnProjectileHitWhite(transform.position);
                }

                // Play sound clip on collsiion
                mixer.GetFloat("MainVolume", out float volume);
                AudioSource.PlayClipAtPoint(impactAudioClip, transform.position, Mathf.Pow(10, (volume / 20.0f)) * 0.5f);

                Destroy(gameObject);
            }
        }
    }
}
