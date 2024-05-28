using RogueFrog.Characters.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Class that controls the settings menu behaviour
namespace RogueFrog.UI.Scripts
{
    public class SettingsMenu : MonoBehaviour
    {
        //public TMP_Dropdown resolutionDropdown;
        private Resolution[] resolutions;

        //public Toggle toggle;

        public Slider volumeSlider;
        public AudioMixer mixer;

        public PlayerSensSO playerSens;

        public Slider lookSensSlider;
        public TMP_Text lookSensValue;

        public Slider aimSensSlider;
        public TMP_Text aimSensValue;

        private void Start()
        {
            /*resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = String.Format("{0} x {1} @ {2}Hz", resolutions[i].width, resolutions[i].height, resolutions[i].refreshRate);
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        toggle.isOn = Screen.fullScreen;*/

            mixer.GetFloat("MainVolume", out float sliderValue);

            volumeSlider.value = Mathf.Pow(10, (sliderValue / 20.0f));
            lookSensSlider.value = playerSens.lookSensitivity;
            aimSensSlider.value = playerSens.aimSensitivity;
            lookSensValue.text = lookSensSlider.value.ToString("0.0");
            aimSensValue.text = aimSensSlider.value.ToString("0.0");
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetVolume(float sliderValue)
        {
            mixer.SetFloat("MainVolume", Mathf.Log10(sliderValue) * 20); 
        }

        public void SetLookSensitivity(float sliderValue)
        {
            playerSens.lookSensitivity = sliderValue;
            lookSensValue.text = sliderValue.ToString("0.0");
        }

        public void SetAimSensitivity(float sliderValue)
        {
            playerSens.aimSensitivity = sliderValue;
            aimSensValue.text = sliderValue.ToString("0.0");
        }
    }
}
