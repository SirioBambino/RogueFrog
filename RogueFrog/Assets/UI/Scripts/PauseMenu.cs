using RogueFrog.Characters.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

// Class that controls the pause menu behaviour
namespace RogueFrog.UI.Scripts
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pauseMenu;
        public GameObject settingsMenu;
        public GameObject victoryScreen;
        public GameObject playerUI;
        public GameObject player;

        private PlayerInputs playerInputs;

        public static bool isPaused;

        private void Awake()
        {
            playerInputs = player.GetComponent<PlayerInputs>();
        }

        private void Start()
        {
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false);
        }

        private void Update()
        {
            if (playerInputs.pause && player.GetComponent<PlayerInfo>().Health > 0 && !player.GetComponent<PlayerInfo>().HasWon)
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }

        public void PauseGame()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseMenu.SetActive(true);
            settingsMenu.SetActive(false);
            Time.timeScale = 0.0f;
            isPaused = true;
            victoryScreen.SetActive(false);
            playerUI.SetActive(!isPaused);
            playerInputs.pause = false;
        }

        public void ResumeGame()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false);
            Time.timeScale = 1.0f;
            isPaused = false;
            playerUI.SetActive(!isPaused);
            playerInputs.pause = false;
        }

        public void ReturnToMenu()
        {
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false);
            Time.timeScale = 1.0f;
            isPaused = false;
            SceneManager.LoadScene("Menu");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
