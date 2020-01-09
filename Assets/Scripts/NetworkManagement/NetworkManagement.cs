using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameServer;
using GameServer.Common;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;
public class NetworkManagement : MonoBehaviour {
    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public void Start() {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 建立房间，同时启动服务器与客户端
    /// </summary>
    public void StartHost() {
        StartServer();
        StartClient();
    }

    /// <summary>
    /// 启动服务器，此时没有玩家加入
    /// </summary>
    public void StartServer() {
        // 实例化服务器对象
        var server = Instantiate(serverPrefab, Vector3.up, Quaternion.identity);
        DontDestroyOnLoad(server);
    }

    /// <summary>
    /// 启动客户端
    /// </summary>
    public void StartClient() {
        var client = Instantiate(clientPrefab, Vector3.up, Quaternion.identity);
        DontDestroyOnLoad(client);
    }
    
    
}
