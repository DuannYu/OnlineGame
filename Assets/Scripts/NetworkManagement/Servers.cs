using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameServer;
using UnityEngine;

public class Servers : MonoBehaviour {
    
   private Socket _server;
    private List<Socket> _clientList;
    private List<string> _clientPosition;

    public void Start() {            
        _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _clientList = new List<Socket>();
        _clientPosition = new List<string>();
        
        _server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666));
        _server.Listen(10);
        Debug.Log("服务器启动成功...");

        var acceptThread = new Thread(AcceptFun);
        acceptThread.IsBackground = true;
        acceptThread.Start();
    }

    /// <summary>
    /// 当有新玩家加入时，服务端调用此函数
    /// </summary>
    private void AcceptFun() {
        while (true)
        {
            Socket client = _server.Accept();
            IPEndPoint clientPoint = client.RemoteEndPoint as IPEndPoint;
            Console.WriteLine(clientPoint.Address + "[" + clientPoint.Port + "]:" + "连接成功...");

            _clientList.Add(client);
            _clientPosition.Add("_0_0_0*0*0*0");
            if (_clientList.Count > 1)
            {
                client.Send(Message.BuildDataPackage(1,2,3,4,5, new string[] {"!" + _clientList.Count}));
                for (int i = 1; i < _clientList.Count; i++)
                {
                    client.Send(Message.BuildDataPackage(1,2,3,4,5, new string[] {"|" + _clientPosition[i - 1]}));
                }
                Broadcast(client, "@newCreated");
            }

            Thread receiveThread = new Thread(()=>ReceiveFun(client));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
    }

    /// <summary>
    /// 接收来自客户端的消息
    /// </summary>
    /// <param name="clientSocket">客户端套接字</param>
    private void ReceiveFun(Socket clientSocket) {
        IPEndPoint clientPoint = clientSocket.RemoteEndPoint as IPEndPoint;
        byte[] msgByte = new byte[1024];
        string msgStr = "";
        while (true) {
            try {
//                int msgLen = clientSocket.Receive(msgByte);
                msgStr = Message.ReceiveMessage(clientSocket);
                if (msgStr != null) {
//                    msgStr = Encoding.UTF8.GetString(msgByte, 0, msgLen);
                    // 收到客户端发来的位置信息
                    if (msgStr[0] == '_') {
                        int index = _clientList.FindIndex(item => item.Equals(clientSocket));
                        
                        _clientPosition[index] = msgStr;
                        // 将位置信息广播给其他客户端
                        Broadcast(clientSocket, msgStr + "|" + index);
                    }
                    else {
                        // 直接广播消息
                        Broadcast(clientSocket, clientPoint.Address + "[" + clientPoint.Port + "]:" + msgStr);
                    }
                }
                clientSocket.Send(Message.BuildDataPackage(1,2,3,4,5, 
                    new string[] {"$ACK"}));                    
            }
            catch (Exception e) {
                int index = _clientList.FindIndex(item => item.Equals(clientSocket));
                _clientPosition.RemoveAt(index);
                _clientList.Remove(clientSocket);
                // 将改玩家退出消息广播
                Broadcast(clientSocket, "#oneDestroyed_" + (index + 1));
                Debug.Log(e.Message);
                break;
            }
        }           
    }

    private void Broadcast(Socket clientID, string msg) {
        Debug.Log("服务器广播" + msg);
        foreach (var item in _clientList) {
            if (item != clientID) {
                item.Send(Message.BuildDataPackage(1,2,3,4,5, new string[] {msg}));
            }
        }
    }
}
