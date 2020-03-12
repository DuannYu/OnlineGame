using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    
    #region Deafult Values
    [Header("Default Menu Values")]
    [SerializeField] private float defaultBrightness;
    [SerializeField] private float defaultVolume;

    [Header("Levels To Load")]
    public string level;
    private string _levelToLoad;
    [SerializeField] private int menuNumber;
    #endregion

    #region Menu Dialogs
    [Header("Main Menu Components")]
    [SerializeField] private GameObject menuDefaultCanvas;
    [SerializeField] private GameObject generalSettingsCanvas;
    [SerializeField] private GameObject soundMenu;
    [SerializeField] private GameObject graphicsMenu;
    [SerializeField] private GameObject controlsMenu;
    #endregion

    #region Silder Linking
    [Header("Menu Sliders")]
    // TODO Brightness
//    [SerializeField] private Brightness brightnessEffect;
//    [SerializeField] private Slider brightnessSlider;
//    [SerializeField] private Text brightnessText;
//    [Space(10)]
    [SerializeField] private Text volumeText;
    [SerializeField] private Slider volumeSlider;
    #endregion

    public void Start() {
        menuNumber = 1;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (menuNumber == 2) {
                GoBackToMainMenu();
                ClickSound();
            }

            if (menuNumber == 3) {
                GoBackToSettingMenu();
                ClickSound();
            }
        }
    }

    public void MouseClick(String buttonType) {
        // Default Menu
        if (buttonType == "Start") {
            // TODO Load new Scene
            Debug.Log("Load Scene");
        }

        if (buttonType == "Settings") {
            menuDefaultCanvas.SetActive(false);
            generalSettingsCanvas.SetActive(true);
            menuNumber = 2;
        }

        if (buttonType == "Exit") {
            Application.Quit();
        }
        
        // General Setting Menu
        if (buttonType == "Sound") {
            generalSettingsCanvas.SetActive(false);
            soundMenu.SetActive(true);
            menuNumber = 3;
        }

        if (buttonType == "Graphics") {
            generalSettingsCanvas.SetActive(false);
            graphicsMenu.SetActive(true);
            menuNumber = 3;
        }

        if (buttonType == "Controls") {
            generalSettingsCanvas.SetActive(false);
            controlsMenu.SetActive(true);
            menuNumber = 3;
        }
    }
    
    
    
    private void ClickSound() {
        GetComponent<AudioSource>().Play();
    }

    public void GoBackToMainMenu() {
        menuDefaultCanvas.SetActive(true);
        generalSettingsCanvas.SetActive(false);
        menuNumber = 1;
    }

    public void GoBackToSettingMenu() {
        soundMenu.SetActive(false);
        graphicsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        generalSettingsCanvas.SetActive(true);
        menuNumber = 2;
    }

    #region Volume Sliders Click

    public void VolumeSlider() {
        Debug.Log("changed");
        AudioListener.volume = volumeSlider.value;
        volumeText.text = volumeSlider.value.ToString("0.0");
    }

    public void VolumeApply() {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        Debug.Log(PlayerPrefs.GetFloat("masterVolume"));
        GoBackToSettingMenu();
    }

    #endregion

    #region ResetButton

    public void ResetButton(String resetType) {
        if (resetType == "Sound") {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeText.text = defaultVolume.ToString("0.0");
        }
    }

    #endregion
}

