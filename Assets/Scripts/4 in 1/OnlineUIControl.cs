using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class OnlineUIControl : MonoBehaviour {
    public CustomNetworkManager manager;

    public GameObject onlineCanvas;
    public GameObject OfflineCanvas;

    private bool isShow;
    // Start is called before the first frame update
    void Start() {
        manager = GetComponent<CustomNetworkManager>();
        OfflineCanvas.SetActive(true);
        onlineCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !OfflineCanvas.activeInHierarchy) {
            isShow = !isShow;
            onlineCanvas.SetActive(isShow);
        }
    }
    
    public void MouseClick(String buttonType) {
        // 仿真界面按钮功能集
        if (buttonType == "Setting") {
            // TODO 设置响应函数
        }

        
        if (buttonType == "Continue") {
            // TODO
            isShow = !isShow;
            onlineCanvas.SetActive(isShow);
        }
        
        if (buttonType == "BackToLobby") {
            onlineCanvas.SetActive(false);
            OfflineCanvas.SetActive(true);
            Disconnected();
        }

        if (buttonType == "Exit") {
            Application.Quit();
        }
    }
    
    public void Disconnected() {

        if (NetworkServer.active || manager.IsClientConnected()) {
            manager.StopHost();
        }
        else {
            manager.StopClient();
        }
    }
}
