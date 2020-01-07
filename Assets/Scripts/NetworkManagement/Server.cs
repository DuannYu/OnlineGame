using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameServer {
    public class Server {
       
        private Socket server;
        private List<Socket> _clientList;
        private List<string> clientPosition;
        public Server()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientList = new List<Socket>();
            clientPosition = new List<string>();
        }

        public void Start()
        {
            server.Bind(new IPEndPoint(IPAddress.Any, 12345));
            server.Listen(10);
            Console.WriteLine("服务器启动成功...");
            
            // 异步接收客户端
            server.BeginAccept(AcceptCallBack, null);
//            Thread acceptThread = new Thread(AcceptFun);
//            acceptThread.IsBackground = true;
//            acceptThread.Start();
        }

        private void AcceptCallBack(IAsyncResult asyncResult) {
            Socket clientSocket = server.EndAccept(asyncResult);
//            Client client = new Client(clientSocket, this);
//            client.Start();
        
            _clientList.Add(clientSocket);
        }

        private void AcceptFun()
        {
            while (true)
            {
                Socket client = server.Accept();
                IPEndPoint clientPoint = client.RemoteEndPoint as IPEndPoint;
                Console.WriteLine(clientPoint.Address + "[" + clientPoint.Port + "]:" + "连接成功...");

                _clientList.Add(client);
                clientPosition.Add("_0_0_0*0*0*0");
                if (_clientList.Count > 1)
                {
                    client.Send(Encoding.UTF8.GetBytes("!" + _clientList.Count.ToString()));
                    for (int i = 1; i < _clientList.Count; i++)
                    {
                        client.Send(Encoding.UTF8.GetBytes("|" + clientPosition[i - 1]));
                    }
                    Broadcast(client, "@newCreated");
                }

                Thread receiveThread = new Thread(ReceiveFun);
                receiveThread.IsBackground = true;
                receiveThread.Start(client);
            }
        }

        private void ReceiveFun(Object obj)
        {
            Socket client = obj as Socket;
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
                            clientPosition[index] = msgStr;
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
                    clientPosition.RemoveAt(index);
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
}
