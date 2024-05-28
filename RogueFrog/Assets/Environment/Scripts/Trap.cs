using UnityEngine;
using CharacterInfo = RogueFrog.Characters.Scripts.CharacterInfo;

// Class that handles the trap's logic
namespace RogueFrog.Environment.Scripts
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] private int damage = 30;
        [SerializeField] private float delayTime = 3.0f;
        Animation anim;

        // Repeat animation with a delay time
        void Start()
        {
            anim = GetComponent<Animation>();
            InvokeRepeating("Animate", delayTime, anim.GetClip("SpikeAnimation").length + delayTime);
        }

        private void Animate()
        {
            anim.Play();
        }

        // When colliding with a character deal damage
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<CharacterInfo>() != null)
            {
                other.GetComponent<CharacterInfo>().Health -= damage;
            }
        }
    }
}
