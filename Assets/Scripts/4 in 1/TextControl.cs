using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 控制标题界面字符闪烁速度
/// </summary>
public class TextControl : MonoBehaviour {

    private Color changeColor;
    private Text _text;

    private float i;

    private float flag;

    public int factor = 1;
    
    // Start is called before the first frame update
    void Start() {
        i = 20f;
        flag = 0.5f * factor;
        _text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        if (i >= 100 || i < 20) flag = -flag;
        changeColor.r = 1f;
        changeColor.g =1f;
        changeColor.b = 1f;
        changeColor.a = i / 100.0f;
        _text.color = changeColor;
        i += flag;
    }
}
