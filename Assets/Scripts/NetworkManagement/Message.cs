using System;
using System.Linq;
using System.Text;
using GameServer.Common;
using UnityEngine;

namespace GameServer {
    public class Message {
        private byte[] _data = new byte[1024];
        // 读取数据的位置 在数组里存取多少字节
        private int _startIndex = 0;
        

        /// <summary>
        /// 解析读取数据
        /// |-------|-------------|------|
        /// | Count | RequestCode | Data |
        /// |-------|-------------|------|
        /// </summary>
        public void ReadMessage(int newDataAmount, Action<RequestCode, ActionCode, string> processDataCallBack) {
            _startIndex += newDataAmount;
            while (true) {
                if (_startIndex <= 4) return;
                // 获取数据长度
                int count = BitConverter.ToInt32(_data, 0);

                if (_startIndex - 4 >= count) {
                   // 先解析RequestCode
                    RequestCode requestCode = (RequestCode)BitConverter.ToInt32(_data, 4);
//                    ActionCode actionCode = (ActionCode)BitConverter.ToInt32(_data, 8);
                    string s = Encoding.UTF8.GetString(_data, 8, count-4);
                    // ????
                    Array.Copy(_data, count+4, _data, 0, _startIndex-4-count);
                    _startIndex -= (count + 4);
                }
                else {
                    return;
                }
            }

        }

        /// <summary>
        /// 组装数据
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="data"></param>
        public byte[] PackData(RequestCode requestCode, Vector3 position) {
            // TODO 当前只能处理一个客户端请求
            
            // 1. 强制转换RequestCode
            byte[] requestCodeBytes = BitConverter.GetBytes((int)requestCode);
            
            // 2. 将位置转换为bytes类型
            byte[] x = BitConverter.GetBytes(position.x);
            byte[] y = BitConverter.GetBytes(position.y);
            byte[] z = BitConverter.GetBytes(position.z);
            
            // 3. 长度为12字节
            var positionBytes = x.Concat(y).Concat(z).ToArray();
            
            // 4. 固定长度为16字节
            int dataAmount = requestCodeBytes.Length + positionBytes.Length;
            byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
            
            // 打包好的字符串长度为20字节
            return dataAmountBytes.Concat(requestCodeBytes).Concat(positionBytes).ToArray();
        }
        
        public byte[] Data {
            get { return _data; }
        }

        public int StartIndex => _startIndex;

        public int RemainSize {
            get { return _data.Length - _startIndex; }
        }
        
    }
}