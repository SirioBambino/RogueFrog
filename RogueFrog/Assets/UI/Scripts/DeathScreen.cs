using UnityEngine;
using CharacterInfo = RogueFrog.Characters.Scripts.CharacterInfo;

// Class that controls the death screen behaviour
namespace RogueFrog.UI.Scripts
{
    public class DeathScreen : MonoBehaviour
    {
        public GameObject player;
        public GameObject deathScreen;

        void Start()
        {
            deathScreen.SetActive(false);
        }

        void Update()
        {
            if (player.GetComponent<CharacterInfo>().Health <= 0)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                deathScreen.SetActive(true);
            }
            else
            {
                deathScreen.SetActive(false);
            }
        }
    }
}
