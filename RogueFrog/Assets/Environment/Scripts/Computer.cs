using RogueFrog.Characters.Scripts;
using UnityEngine;

// Class that handles the computer's logic
namespace RogueFrog.Environment.Scripts
{
    public class Computer : MonoBehaviour
    {
        public GameObject victoryScreen;

        void Start()
        {
            victoryScreen.SetActive(false);
        }

        // When colliding with player and they have a card make victory screen active
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerInfo>() && other.GetComponent<PlayerInfo>().HasCard) victoryScreen.SetActive(true);
        }
    }
}
