using RogueFrog.Characters.Scripts;
using UnityEngine;
using UnityEngine.UI;
using CharacterInfo = RogueFrog.Characters.Scripts.CharacterInfo;

// Class that controls the HUD screen behaviour
namespace RogueFrog.UI.Scripts
{
    public class HUDScreen : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Slider healthBar;
        [SerializeField] private Image crosshair;

        private PlayerInputs playerInputs;
        private CharacterInfo characterInfo;

        private void Awake()
        {
            playerInputs = player.GetComponent<PlayerInputs>();
            characterInfo = player.GetComponent<CharacterInfo>();
        }

        private void Start()
        {
            healthBar.value = characterInfo.Health;
            healthBar.maxValue = characterInfo.MaxHealth;
        }

        private void Update()
        {
            healthBar.value = characterInfo.Health;

            if (playerInputs.aim)
                crosshair.enabled = true;
            else
                crosshair.enabled = false;
        }
    }
}
