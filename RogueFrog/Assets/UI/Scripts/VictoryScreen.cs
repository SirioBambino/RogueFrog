using RogueFrog.Characters.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

// Class that controls the victory screen behaviour
namespace RogueFrog.UI.Scripts
{
    public class VictoryScreen : MonoBehaviour
    {
        public GameObject victoryScreen;
        public GameObject image;
        public GameObject titleText;
        public GameObject menuButton;
        public GameObject tipText;
        public GameObject player;

        private PlayerInputs playerInputs;

        public static bool isPaused;

        private void Awake()
        {
            playerInputs = player.GetComponent<PlayerInputs>();
        }

        void Update()
        {
            if (playerInputs.insertCard)
            {
                playerInputs.insertCard = false;
                if (victoryScreen.activeSelf) player.GetComponent<PlayerInfo>().HasWon = true;
            }

            if (player.GetComponent<PlayerInfo>().HasWon)
            {
                image.SetActive(true);
                titleText.SetActive(true);
                menuButton.SetActive(true);
                tipText.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0.0f;
            }
            else
            {
                image.SetActive(false);
                titleText.SetActive(false);
                menuButton.SetActive(false);
                tipText.SetActive(true);
            }
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Menu");
        }
    }
}
