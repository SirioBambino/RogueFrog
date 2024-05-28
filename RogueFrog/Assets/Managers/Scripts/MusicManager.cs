using UnityEngine;

// Singleton class that plays background music with the game is started and works across scenes
namespace RogueFrog.Managers.Scripts
{
    public class MusicManager : MonoBehaviour
    {
        private AudioSource audioSource;
        private static MusicManager musicInstance;

        void Awake()
        {
            DontDestroyOnLoad(this);
            audioSource = GetComponent<AudioSource>();

            if (musicInstance == null) musicInstance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}