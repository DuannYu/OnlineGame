using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 飞行器-大厅界面控制
/// </summary>
public class AircraftUIControl : MonoBehaviour {

    public Button ownButton;
    private int _aircraftIndex = 0;
    private int _sceneIndex = 0;
    void Start() {
        ownButton.interactable  = false;
    }

    public void MouseClick(String buttonType) {
        // 左侧按钮功能集
        if (buttonType == "Aircraft") {
            SceneManager.LoadScene("Scenes/4in1/4in1Aircraft");
        }
        
        if (buttonType == "Universe") {
            SceneManager.LoadScene("Scenes/4in1/4in1Universe");
        }
        
        if (buttonType == "Land") {
            SceneManager.LoadScene("Scenes/4in1/4in1Land");
        }
        
        if (buttonType == "Sea") {
            SceneManager.LoadScene("Scenes/4in1/4in1Sea");
        }
        
        if (buttonType == "Bicycle") {
            SceneManager.LoadScene("Scenes/4in1/4in1Bicycle");
        }
        
        if (buttonType == "Exit") {
            Application.Quit();
        }
    }
    
    
}
