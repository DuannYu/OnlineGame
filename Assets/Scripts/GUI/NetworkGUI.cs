using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkManager))]
public class NetworkGUI : MonoBehaviour {

    public const int AircraftSumNumber = 2;
    public const int SceneSumNumber = 2;
    #region Menu Dialogs

    [Header("Main Menu Components")] 
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject menuDefaultCanvas;
    [SerializeField] private GameObject startPopoutCanvas;
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

    #region Arena Setting

    [Header("Menu Arena Setting")]
    [SerializeField] private Text aircraftText;
    [SerializeField] private Text sceneText;
    

    #endregion

    public CustomNetworkManager manager;
    
    private int _menuNumber;
    private int _aircraftIndex = 0;
    private int _sceneIndex = 0;
    private string[] _aircraftNameArray = new []{"Baron 58", "B 2"};
    private string[] _sceneNameArray = new []{"沙漠", "丘陵"};
    

    private void Start() {
        _menuNumber = 0;
        menuDefaultCanvas.SetActive(true);
        manager = GetComponent<CustomNetworkManager>();
    }

    private void Update() {
        // update text
        aircraftText.text = _aircraftNameArray[_aircraftIndex];
        sceneText.text = _sceneNameArray[_sceneIndex];
        
        // esc function
        if (Input.GetKeyDown(KeyCode.Escape) && !menuDefaultCanvas.activeInHierarchy) {
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
            // TODO popout canvas
            menuDefaultCanvas.SetActive(false);
            startPopoutCanvas.SetActive(true);
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
        
        // start popout canvas
        if (buttonType == "BackToLobby") {
            menuDefaultCanvas.SetActive(true);
            startPopoutCanvas.SetActive(false);
        }
        
        if (buttonType == "StartArena") {
            startPopoutCanvas.SetActive(false);
            manager.StartHost();
        }

        if (buttonType == "FlightPrev") {
            _aircraftIndex = Math.Abs(_aircraftIndex - 1) % AircraftSumNumber;
            manager.playerPrefabIndex = (short)_aircraftIndex;
        }
        
        if (buttonType == "FlightNext") {
            _aircraftIndex = (_aircraftIndex + 1) % AircraftSumNumber;
            manager.playerPrefabIndex = (short)_aircraftIndex;
        }
        
        if (buttonType == "ScenePrev") {
            _sceneIndex =  Math.Abs(_sceneIndex - 1) % SceneSumNumber;
            // TODO bind Scene and Index
        }
        
        if (buttonType == "SceneNext") {
            _sceneIndex = (_sceneIndex + 1) % SceneSumNumber;
            // TODO bind Scene and Index
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

    /// <summary>
    /// 对场景和机型进行初始化
    /// </summary>
    public void InitNetworkSetting() {
        aircraftText.text = _aircraftNameArray[0];
        sceneText.text = _sceneNameArray[0];
    }

}
