using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkManager))]
public class NetworkGUI : MonoBehaviour {
    
    public NetworkManager manager;
    
    #region Menu Dialogs

    [Header("Main Menu Components")] 
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject menuDefaultCanvas;
    [SerializeField] private GameObject generalSettingsCanvas;
    [SerializeField] private GameObject inGameMenuCanvas;
    [SerializeField] private GameObject soundMenu;
    [SerializeField] private GameObject graphicsMenu;
    [SerializeField] private GameObject controlsMenu;
    #endregion

    #region Silder Linking
    [Header("Menu Sliders")]
    [SerializeField] private Brightness brightnessEffect;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Text brightnessText;
    [Space(10)]
    [SerializeField] private Text volumeText;
    [SerializeField] private Slider volumeSlider;
    #endregion

    private int _menuNumber;
    

    private void Start() {
        _menuNumber = 0;
        manager = GetComponent<NetworkManager>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !menuDefaultCanvas.activeInHierarchy) {
            Debug.Log(_menuNumber);
            switch (_menuNumber) {
                case 0:
                    inGameMenuCanvas.SetActive(true);
                    _menuNumber = 1;
                    break;
                case 1:
                    inGameMenuCanvas.SetActive(false);
                    _menuNumber = 0;
                    break;
                case 2:
                    GoBackToMainMenu();
                    break;
                case 3:
                    GoBackToSettingMenu();
                    break;
            }
        }
    }

    public void MouseClick(string buttonType) {
        // menu default canvas
        if (buttonType == "Start") {
            menuDefaultCanvas.SetActive(false);
            manager.StartHost();
        }

        if (buttonType == "Join") {
            manager.StartClient();
        }

        if (buttonType == "BackToTheme") {
            Disconnected();
            SceneManager.LoadScene("Scenes/MainTheme");
        }
        
        // in-game menu canvas
        if (buttonType == "Continue") {
            inGameMenuCanvas.SetActive(false);
        }

        if (buttonType == "Settings") {
            inGameMenuCanvas.SetActive(false);
            generalSettingsCanvas.SetActive(true);
            _menuNumber = 2;
        }

        if (buttonType == "Disconnected") {
            Disconnected();
            inGameMenuCanvas.SetActive(false);
            menuDefaultCanvas.SetActive(true);
        }
        
        // general setting canvas
        if (buttonType == "Sound") {
            generalSettingsCanvas.SetActive(false);
            soundMenu.SetActive(true);
            _menuNumber = 3;
        }

        if (buttonType == "Graphics") {
            generalSettingsCanvas.SetActive(false);
            graphicsMenu.SetActive(true);
            _menuNumber = 3;
        }

        if (buttonType == "Controls") {
            generalSettingsCanvas.SetActive(false);
            controlsMenu.SetActive(true);
            _menuNumber = 3;
        }
    }

    public void Disconnected() {
        if (NetworkServer.active || manager.IsClientConnected()) {
            manager.StopHost();
        }
        else {
            manager.StopClient();
        }
        _menuNumber = 0;
    }
    
    public void GoBackToMainMenu() {
        inGameMenuCanvas.SetActive(true);
        generalSettingsCanvas.SetActive(false);
        _menuNumber = 1;
    }

    public void GoBackToSettingMenu() {
        soundMenu.SetActive(false);
        graphicsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        generalSettingsCanvas.SetActive(true);
        _menuNumber = 2;
    }

}
