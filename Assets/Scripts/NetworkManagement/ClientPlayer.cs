using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameServer;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

/*
 * ClientPlayer类
 * 客户端专用类，向服务器发送数据
 */

public class ClientPlayer : MonoBehaviour {
    private Socket _clientSocket;
    private Server _server;
    private bool _isLocal;
    private string msgStr = "";
    public GameObject prefab;
    private GameObject player;
    private Message _message = new Message();
    
    byte[] dataBuffer = new byte[1024];

    private void Start() {
        Connect();
        
        Thread receiveThread = new Thread(ReceiveMessage);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void Update() {
        SendMessage(GameObject.Find("Cube").transform.position);

        Debug.Log(BitConverter.ToSingle(dataBuffer, 0));
        player.transform.position = new Vector3(BitConverter.ToSingle(dataBuffer, 0), 
            BitConverter.ToSingle(dataBuffer, 4), 
            BitConverter.ToSingle(dataBuffer, 8));
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    public void Connect() {
        _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // 服务器IP地址
        var address = IPAddress.Parse("127.0.0.1");
        // 服务器端口
        var ipEndPoint = new IPEndPoint(address, 12345);
        try {
            _clientSocket.Connect(ipEndPoint);
            Debug.Log("客户端连接成功");
            player = Instantiate(prefab, Vector3.up, Quaternion.identity);
        }
        catch (Exception e) {
            Debug.Log(e);
            throw;
        }
    }

    // TODO 完善代码注释
    /// <summary>
    /// 向服务器发送数据
    /// </summary>
    public void SendMessage(Vector3 position) {
        byte[] x = BitConverter.GetBytes(position.x);
        byte[] y = BitConverter.GetBytes(position.y);
        byte[] z = BitConverter.GetBytes(position.z);
        byte[] message = x.Concat(y).Concat(z).ToArray();
        _clientSocket.Send(message);
    }

    /// <summary>
    /// 接收服务器端数据
    /// </summary>
    public void ReceiveMessage() {
        while (true)
        {
            try
            {
                int msgLen = _clientSocket.Receive(dataBuffer);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                break;
            }
        }
    }
}
