namespace GameServer.Common {
    public enum RequestCode {
        None,   
        // 客户端请求
        Join,            // 客户端加入请求
        Sync,            // 数据同步
        Quit,            // 客户端退出请求
        
        // 服务端转发
        JoinEcho,        // 客户端加入请求回应
        Init,            // 服务器向客户端发送当前状态
        Error,           // 发生错误
        NewComeIn,       // 新玩家加入
        Disconnected,    // 玩家退出
        
    }
}