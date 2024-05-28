using UnityEngine;
using UnityEngine.SceneManagement;

// Class that controls the main menu behaviour
namespace RogueFrog.UI.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        public void StartGameA()
        {
            SceneManager.LoadScene("Game");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void StartGameB()
        {
            SceneManager.LoadScene("GameHandmade");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
