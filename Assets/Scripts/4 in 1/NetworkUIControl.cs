using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 下拉菜单控制脚本
/// </summary>
public class NetworkUIControl : MonoBehaviour {
    // 总场景数与总机型数
   
    private int _aircraftIndex = 0;
    private int _sceneIndex = 0;
    private bool _isHost;
    
    public CustomNetworkManager manager;
    
    public GameObject obg;
    // Start is called before the first frame update
    void Start() {
        Init();
        GameObject.Find("AircraftSelectDropdown").GetComponent<Dropdown>().onValueChanged.AddListener(ModelSelectResponse);
        GameObject.Find("SceneSelectDropdown").GetComponent<Dropdown>().onValueChanged.AddListener(SceneSelectResponse);
        GameObject.Find("NetworkDropdown").GetComponent<Dropdown>().onValueChanged.AddListener(NetworkSelectResponse);
    }

    // Update is called once per frame
    void Update() {
        
    }
    
    public void MouseClick(String buttonType) {
        // 水平按钮功能集
        if (buttonType == "Setting") {
            // TODO 设置响应函数
        }
        
        if (buttonType == "Start") {
            obg.SetActive(false);
            if (_isHost)
                manager.StartHost();
            else {
                manager.StartClient();
            }
        }
        
    }
    /// <summary>
    /// 机型选择回调
    /// </summary>
    /// <param name="value"></param>
    public void ModelSelectResponse(int value) {
        switch (value) {
            case 0:
                manager.playerPrefabIndex = 0;
                break;
            case 1:
                manager.playerPrefabIndex = 1;
                break;
            case 2:
                manager.playerPrefabIndex = 2;
                break;

        }
    }

    public void SceneSelectResponse(int value) {
        switch (value) {
            case 1:
                manager.onlineScene = "Scenes/4in1/Maps/map0";
                break;
            case 2:
                manager.onlineScene = "Scenes/4in1/Maps/map1";
                break;
        }
    }
    
    public void NetworkSelectResponse(int value) {
        switch (value) {
            case 1:
                _isHost = true;
                break;
            case 2:
                _isHost = false;
                break;
        }
    }

    /// <summary>
    /// 初始化默认设置
    /// </summary>
    public void Init() {
        obg.SetActive(true);
        manager = GetComponent<CustomNetworkManager>();
        
        // 默认地图
        manager.onlineScene = "Scenes/4in1/Maps/map0";
        
        // 默认飞行器
        manager.playerPrefabIndex = 0;
        
        // 默认以主机方式加入
        _isHost = true;
    }
    
}
