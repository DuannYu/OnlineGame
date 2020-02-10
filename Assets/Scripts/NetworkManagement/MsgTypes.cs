using UnityEngine.Networking;

public class MsgTypes
{
    public const short PlayerPrefab = MsgType.Highest + 1;
 
    public class PlayerPrefabMsg : MessageBase
    {
        public short controllerID;    
        public short prefabIndex;
    }
}