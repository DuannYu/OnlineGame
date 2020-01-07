using System;
using System.Linq;
using System.Text;

namespace GameServer {
    public class Message {
        private byte[] _data = new byte[1024];
        // 读取数据的位置 在数组里存取多少字节
        private int _startIndex = 0;

        /// <summary>
        /// 将字符串转换为字符数组
        /// </summary>
        /// <param name="data">需要转换的字节</param>
        public static byte[] GetBytes(string data) {
            // 数据内容
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int dataLength = dataBytes.Length;
            // 数据长度
            byte[] lengthBytes = BitConverter.GetBytes(dataLength);
            // 将两个字符连接
            byte[] newBytes = lengthBytes.Concat(dataBytes).ToArray();

            return newBytes;
        }

        /// <summary>
        /// 解析读取数据
        /// |-------|------------|-------------|------|
        /// | Count | ActionCode | RequestCode | Data |
        /// |-------|------------|-------------|------|
        /// </summary>
        public void ReadMessage(int newDataAmount, Action<string> processDataCallBack) {
            _startIndex += newDataAmount;
            while (true) {
                if (_startIndex <= 4) return;
                // 获取数据长度
                int count = BitConverter.ToInt32(_data, 0);

                if (_startIndex - 4 >= count) {
    //                    string s = Encoding.UTF8.GetString(_data, 4, count);
    //                    Console.Write("解析出的数据为：" + s);
                    // 先解析RequestCode
    //                    RequestCode requestCode = (RequestCode)BitConverter.ToInt32(_data, 4);
    //                    ActionCode actionCode = (ActionCode)BitConverter.ToInt32(_data, 8);
                    string s = Encoding.UTF8.GetString(_data, 12, count-8);
                    processDataCallBack(s);
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
        public static byte[] PackData(string data) {
            byte[] result = Encoding.UTF8.GetBytes(data);
            return result;
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