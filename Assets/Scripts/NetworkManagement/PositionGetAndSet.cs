using UnityEngine;

namespace GameServer {
    public class PositionGetAndSet {
        static public string GetPosition(Transform t)
    {
        float x = (float)(Mathf.Round(t.position.x * 100)) / 100;
        float y = (float)(Mathf.Round(t.position.y * 100)) / 100;
        float z = (float)(Mathf.Round(t.position.z * 100)) / 100;
        //Debug.Log(x.ToString() + "_" + y.ToString() + "_" + z.ToString());
        return ("_" + x.ToString() + "_" + y.ToString() + "_" + z.ToString());      //第一个符号为标志，"_"代表位置，"*"代表角度
    }

    static public void SetPosition(Transform t, string pos)
    {
        float x, y, z;
        string[] posXYZ = pos.Split('_');
        bool flagX = float.TryParse(posXYZ[1], out x);
        bool flagY = float.TryParse(posXYZ[2], out y);
        bool flagZ = float.TryParse(posXYZ[3], out z);
        //Debug.Log(x.ToString());
        if (flagX && flagY && flagZ)
        {
            t.position = new Vector3(x, y, z);
        }
        else
        {
            Debug.Log("数据转换失败！");
        }
    }

    static public string GetRotation(Transform t)
    {
        float x = (float)(Mathf.Round(t.localEulerAngles.x * 100)) / 100;
        float y = (float)(Mathf.Round(t.localEulerAngles.y * 100)) / 100;
        float z = (float)(Mathf.Round(t.localEulerAngles.z * 100)) / 100;
        return ("*" + x.ToString() + "*" + y.ToString() + "*" + z.ToString());
    }

    static public void SetRotation(Transform t, string angle)
    {
        float x, y, z;
        string[] rotationXYZ = angle.Split('*');
        bool flagX = float.TryParse(rotationXYZ[1], out x);
        bool flagY = float.TryParse(rotationXYZ[2], out y);
        bool flagZ = float.TryParse(rotationXYZ[3], out z);
        //Debug.Log(x.ToString());
        if (flagX && flagY && flagZ)
        {
            t.rotation = Quaternion.Euler(x, y, z);
        }
        else
        {
            Debug.Log("数据转换失败！");
        }
    }

    static public GameObject instantiatePrefab(GameObject obj, int index)
    {
        GameObject instant = GameObject.Instantiate(obj, Vector3.up, Quaternion.identity);
        instant.name = "player" + index;
        return instant;
    }
    }
}