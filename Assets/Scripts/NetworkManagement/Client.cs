using System;
using System.Net.Sockets;
using UnityEngine;
/*
 * Client类
 * 负责与客户端数据同步收发 
 */

namespace GameServer {
    public class Client {
        private bool _isLocal;    // 是否为本地玩家
        private Socket _clientSocket;
        private Server _server;
        private Message _message = new Message();
        byte[] dataBuffer = new byte[1024];
        


        public Client(Socket clientSocket) {
            _clientSocket = clientSocket;
        }

        public void Start() {
//             gameObject = GameObject.Find("Cube");
            _clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, null);
        }

        /// <summary>
        /// Receive异步回调函数
        /// </summary>
        /// <param name="asyncResult"></param>
        private void ReceiveCallBack(IAsyncResult asyncResult) {
            try {
                int count = _clientSocket.EndReceive(asyncResult);
                // 断开连接
                if (count == 0) {
                    Close();
                }
                Debug.Log("x: " + BitConverter.ToSingle(dataBuffer, 0)
                          + " y: " + BitConverter.ToSingle(dataBuffer, 4)
                          + " z: " + BitConverter.ToSingle(dataBuffer, 8));
                Start();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                Close();
            }
        }

        private void OnProcessMessage(string data) {

        }

        /// <summary>
        /// 包装数据
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="data"></param>
        public void Send(string data) {
            byte[] bytes = Message.PackData(data);
            _clientSocket.Send(bytes);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        private void Close() {
            if (_clientSocket != null) {
                _clientSocket.Close();
            }
//            _server.RemoveClient(this);
        }

        public bool IsLocal => _isLocal;

        public byte[] DataBuffer => dataBuffer;
    }
}
