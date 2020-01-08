using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameServer;
using GameServer.Common;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkManagement : MonoBehaviour {
    
    private Socket _server;
    private List<Socket> _clientList;
    private List<string> _clientPosition;

    public void Start() {            
        _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _clientList = new List<Socket>();
        _clientPosition = new List<string>();
        
        _server.Bind(new IPEndPoint(IPAddress.Any, 12345));
        _server.Listen(10);
        Debug.Log("服务器启动成功...");

        var acceptThread = new Thread(AcceptFun);
        acceptThread.IsBackground = true;
        acceptThread.Start();
    }

    private void AcceptFun() {
        while (true) {
            Socket client = _server.Accept();
            IPEndPoint clientPoint = client.RemoteEndPoint as IPEndPoint;
            Debug.Log(clientPoint.Address + "[" + clientPoint.Port + "]:" + "连接成功...");

            _clientList.Add(client);
            _clientPosition.Add("_0_0_0*0*0*0");
            if (_clientList.Count > 1) {
                client.Send(Encoding.UTF8.GetBytes("!" + _clientList.Count));
                for (int i = 1; i < _clientList.Count; i++) {
                    client.Send(Encoding.UTF8.GetBytes("|" + _clientPosition[i - 1]));
                }
                Broadcast(client, "@newCreated");
            }

            Thread receiveThread = new Thread(()=>ReceiveFun(client));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
    }

    private void ReceiveFun(Socket obj)
    {
        Socket client = obj;
        IPEndPoint clientPoint = client.RemoteEndPoint as IPEndPoint;
        byte[] msgByte = new byte[1024];
        string msgStr = "";
        while (true)
        {
            try
            {
                int msgLen = client.Receive(msgByte);
                if (msgLen > 0)
                {
                    msgStr = Encoding.UTF8.GetString(msgByte, 0, msgLen);
                    if (msgStr[0] == '_')
                    {
                        int index = _clientList.FindIndex(item => item.Equals(client));
                        _clientPosition[index] = msgStr;
                        Console.WriteLine(msgStr);
                        Broadcast(client, msgStr + "|" + index.ToString());
                    }
                    else
                    {
                        Console.WriteLine(msgStr);
                        Broadcast(client, clientPoint.Address + "[" + clientPoint.Port + "]:" + msgStr);
                    }
                }
                client.Send(Encoding.UTF8.GetBytes("$ACK"));                    
            }
            catch (Exception e)
            {
                int index = _clientList.FindIndex(item => item.Equals(client));
                _clientPosition.RemoveAt(index);
                _clientList.Remove(client);
                Broadcast(client, "#oneDestroyed_" + (index + 1).ToString());
                Console.WriteLine(e.Message);
                break;
            }
        }           
    }

    private void Broadcast(Socket clientID, string msg)
    {
        foreach (var item in _clientList)
        {
            if (item != clientID)
            {
                item.Send(Encoding.UTF8.GetBytes(msg));
            }
        }
    }
}
