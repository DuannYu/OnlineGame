using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameServer;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class Clients : MonoBehaviour {
    
    public GameObject playerPrefab;
    public GameObject localPlayerPrefab;

    private string _msgStr = "";
    private int _instantiateCount = -1;
    private bool _msgFlag;
    private bool _instantiateFlag;
    private bool _receiveMsgFlag;
    private bool _destroyFlag;

    private GameObject _localPlayer;
    private string _posStr = "_0_0_0";
    private string _rotStr = "*0*0*0";
    private bool _positionFlag;
    private Vector3 _posLast;
    private Vector3 _rotationLast;
    private List<GameObject> _playersList;
    private bool _otherClientMoveFlag;

    private Socket _clientSocket;

    // Start is called before the first frame update
    void Start() {
        _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // 服务器IP地址
        var address = IPAddress.Parse("127.0.0.1");
        // 服务器端口
        var ipEndPoint = new IPEndPoint(address, 6666);
        try {
            _clientSocket.Connect(ipEndPoint);
            Debug.Log("客户端连接成功");

            // 启动接受消息线程
            Thread receiveThread = new Thread(ReceiveFun);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            // 启动消息处理线程
            Thread msgManageThread = new Thread(MsgManageFun);
            msgManageThread.IsBackground = true;
            msgManageThread.Start();
        }
        catch (System.Exception e) {
            Debug.Log(e.Message);
        }
        
        
        _playersList = new List<GameObject>();
        _posLast = new Vector3(0f, 0f, 0f);
        _rotationLast = new Vector3(0f, 0f, 0f);
        
        _localPlayer = Instantiate(localPlayerPrefab, Vector3.up, Quaternion.identity);
        
    }

    // Update is called once per frame
    void Update() {
        
        // 实例化只能在主线程
        if (_instantiateFlag)                //实例化只能在主线程
        {
            _instantiateFlag = false;
            if (_instantiateCount > 1)
            {
                for (int i = 1; i < _instantiateCount; i++)
                {
                    GameObject instance = PositionGetAndSet.instantiatePrefab(playerPrefab, i);     //实例化对象需要在主线程
                    _playersList.Add(instance);
                }
                _localPlayer.name = "player" + (_instantiateCount + 1).ToString();
                _playersList.Add(_localPlayer);
            }
            else
            {
                GameObject instance = PositionGetAndSet.instantiatePrefab(playerPrefab, _playersList.Count + 1);
                if (_playersList.Count == 0)
                {
                    _playersList.Add(_localPlayer);
                }
                _playersList.Add(instance);
            }
        }

        // 销毁对象，销毁物体只能在主线程
        if (_destroyFlag) {                
            _destroyFlag = false;
            string[] indexStr = _msgStr.Split('_');
            int index = int.Parse(indexStr[1]);
            GameObject objDestroyed = _playersList[index];
            _playersList.RemoveAt(index);
            Destroy(objDestroyed);
        }

        // 移动对象
        if (_otherClientMoveFlag) {
            _otherClientMoveFlag = false;
//            Debug.Log(_msgStr);
            string[] data = _msgStr.Split('|');
            Debug.Log("playlist：" + data[2] + "count " + _playersList.Count);
            PositionGetAndSet.SetPosition(_playersList[int.Parse(data[2])].transform, data[0]);
            PositionGetAndSet.SetRotation(_playersList[int.Parse(data[2])].transform, data[1]);
        }
        
        // 位置发生变化
        if (_posLast != _localPlayer.transform.position) {
            _posStr = PositionGetAndSet.GetPosition(_localPlayer.transform);
            _posLast = _localPlayer.transform.position;
            _positionFlag = true;
        }

        // 角度发生变化
        if (_rotationLast != _localPlayer.transform.localEulerAngles) {
            _rotStr = PositionGetAndSet.GetRotation(_localPlayer.transform);
            _rotationLast = _localPlayer.transform.localEulerAngles;
            _positionFlag = true;
        }
        
        // 发送位置信息
        if (_positionFlag) {
            _positionFlag = false;
//            Debug.Log(_posStr + "|" + _rotStr);
            SendMsg(_posStr + "|" + _rotStr);
        }
    }

    /// <summary>
    /// 接收服务器发来的消息
    /// </summary>
    private void ReceiveFun() {
        byte[] msgByte = new byte[1024];
        while (true) {
            try {
//                int msgLen = _clientSocket.Receive(msgByte);
                    
                // 消息置位要写在byte转换成str消息之后，否则消息处理线程可能会得到一个未转换的msg，也就是初始化的空串
//                _msgStr = Encoding.UTF8.GetString(msgByte, 0, msgLen);    
                _msgStr = Message.ReceiveMessage(_clientSocket);
                _msgFlag = true;
            }
            catch (System.Exception e) {
                Debug.Log(e.Message);
                break;
            }
        }
    }

    private void MsgManageFun() {
        while (true) {
            if (_msgFlag) {
//                lock (_msgLock) {
                    _msgFlag = false;
//                }
                
                // !clientList.count 玩家数量
                if (_msgStr != null && _msgStr[0] == '!') {
//                    Debug.Log(_msgStr);
                    _instantiateFlag = true;
                    var countStr = _msgStr.Substring(1, _msgStr.Length - 1).Split('|');
                    Debug.Log("玩家数量" + _instantiateCount);
                    _instantiateCount = int.Parse(countStr[0]);
                }
                
                // @newCreated
                else if (_msgStr != null && _msgStr[0] == '@') {
                    _instantiateFlag = true;
                    _instantiateCount = 1;
                }
                
                // #oneDestroyed_
                else if (_msgStr != null && _msgStr[0] == '#') {
                    _destroyFlag = true;
                    
                }
                
                // _position
                else if (_msgStr != null && _msgStr[0] == '_') {
                    _otherClientMoveFlag = true;                    
                }
                
                // 连接成功
                else if (_msgStr != "$ACK") {
                    _receiveMsgFlag = true;
                }
            }
        }
    }

    private void SendMsg(string message)
    {
        if (message != null)
        {
            //Debug.Log(msgStr);
            string[] msg = {message};
            _clientSocket.Send(Message.BuildDataPackage(1,2,3,4,5, msg));
        }
    }
}
