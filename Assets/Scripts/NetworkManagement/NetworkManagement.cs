using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameServer;
using UnityEngine;

public class NetworkManagement : MonoBehaviour{
    
    private Socket server;
    private List<Socket> _clientList;
    private List<string> clientPosition;
    public GameObject clientPrefab;
    public NetworkManagement()
    {
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _clientList = new List<Socket>();
        clientPosition = new List<string>();
    }

    public void Start() {
//        StartHost();
        StartClient();
    }

    public void StartHost() {
        server.Bind(new IPEndPoint(IPAddress.Any, 12345));
        server.Listen(10);
        Debug.Log("服务器启动成功...");

        Thread acceptThread = new Thread(AcceptFun);
        acceptThread.IsBackground = true;
        acceptThread.Start();
    }

    public void StartClient() {
        Instantiate(clientPrefab);
    }

    private void AcceptFun()
    {
        while (true)
        {
            Socket client = server.Accept();
            _clientList.Add(client);
            Thread receiveThread = new Thread(()=>ReceiveFun(client));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
    }

    private void ReceiveFun(Socket client)
    {
        byte[] msgByte = new byte[1024];
        string msgStr = "";
       
        while (true)
        {
            try
            {
                client.Receive(msgByte);
//                Debug.Log(BitConverter.ToSingle(msgByte, 0));
                BoardCast(client, msgByte);
//                client.Send(msgByte);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                break;
            }
        }           
    }

    /// <summary>
    /// 向其他客户端广播数据
    /// </summary>
    /// <param name="client"></param>
    /// <param name="message"></param>
    private void BoardCast(Socket client, byte[] message) {
        foreach (var otherClient in _clientList) {
            if (client != otherClient) {
                otherClient.Send(message);
            }
            
        }
    }

    private void OnApplicationQuit() {
        server.Close();
        foreach (var client in _clientList) {
            client.Close();
        }
    }
}
