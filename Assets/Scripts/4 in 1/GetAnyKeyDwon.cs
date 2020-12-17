using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 标题界面中，按下任意键跳转至大厅界面
/// </summary>
public class GetAnyKeyDwon : MonoBehaviour {
    private void Awake() {

    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (Input.anyKeyDown)
            SceneManager.LoadScene("Scenes/4in1/4in1Aircraft");
    }
}
