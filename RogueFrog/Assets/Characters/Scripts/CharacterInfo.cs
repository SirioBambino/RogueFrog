using UnityEngine;
using UnityEngine.Audio;

// Class that holds basic information about characters
namespace RogueFrog.Characters.Scripts
{
    public class CharacterInfo : MonoBehaviour
    {
        [Header("Basic Stats")]
        private float currentHealth;
        public float MaxHealth = 100.0f;

        private int currentAmmoInClip;
        public int MaxAmmoInClip = 100;

        public float MovementSpeed;
        public float AttackSpeed;
        public int AttackDamage;

        [SerializeField] private float invunerabilityTime;
        private float invunerabilityTimer = 0.0f;

        [Header("Audio")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioClip damageAudioClip;

        public float Health
        {
            get { return currentHealth; }

            set 
            {
                // Only remove health if invunerability is not active
                if (currentHealth > value)
                {
                    if (invunerabilityTimer <= 0)
                    {
                        currentHealth = Mathf.Clamp(value, 0.0f, MaxHealth);
                        invunerabilityTimer = invunerabilityTime;

                        // Play damage audio clip when health is removed
                        mixer.GetFloat("MainVolume", out float volume);
                        AudioSource.PlayClipAtPoint(damageAudioClip, transform.position, Mathf.Pow(10, (volume / 20.0f)) * 0.5f);
                    }
                }
                else
                    currentHealth = Mathf.Clamp(value, 0.0f, MaxHealth);
            }
        }

        public float Ammo
        {
            get { return currentAmmoInClip; }

            set { currentAmmoInClip = (int)Mathf.Clamp(value, 0, MaxAmmoInClip); }
        }

        private void Update()
        {
            if (invunerabilityTimer > 0) invunerabilityTimer -= Time.deltaTime;
        }
    }
}
