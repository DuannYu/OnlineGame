using System;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseUIControl : MonoBehaviour {
    [Header("通用组件")]
    public Text aircraft;
    public Text length;
    public Text wing;
    public Text weight;
    public Text failed;
    public GameObject group;
    public InputField inputField;
    public GameObject AddPopout;
    public GameObject ModPopout;
    
    [Header("添加功能组件")]
    public InputField AddAircraftInputField;
    public InputField AddLengthInputField;
    public InputField AddWingInputField;
    public InputField AddWeightInputField;
    
    [Header("修改功能组件")]
    public InputField ModAircraftInputField;
    public InputField ModLengthInputField;
    public InputField ModWingInputField;
    public InputField ModWeightInputField;



    private static MySqlConnection conn;
    private static string connStr = "Database=simulator;Data Source=127.0.0.1;User Id=root;Password=root;port=3306";
    // Start is called before the first frame update
    void Start() {
        // 连接数据库
        OpenConnection();
        UIInit();
    }



    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            Search();
        }
    }
    
    /// <summary>
    /// UI组件初始化
    /// </summary>
    void UIInit() {
        failed.enabled = false;
        group.SetActive(false);
        AddPopout.SetActive(false);
        ModPopout.SetActive(false);
    }
    /// <summary>
    /// 连接数据库
    /// </summary>
    public static void OpenConnection()
    {
        try
        {
            conn = new MySqlConnection(connStr);
            conn.Open();
            Debug.Log("数据库连接成功");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void Search() {
        Search(inputField.text);
    }
    /// <summary>
    /// 根据输入框信息查询
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns></returns>
    public void Search(string inputString)
    {
        failed.enabled = true;
        failed.color = Color.red;
        failed.text = "查询失败，不存在机型：" + inputString;
        group.SetActive(false);
        
        int indexDic = 0;
        int indexList = 0;
        Dictionary<int, List<string>> dic = new Dictionary<int, List<string>>();
        MySqlDataReader reader = null;
        MySqlCommand command = new MySqlCommand("select lengths, wingspan, weights from aircraft where names=\"" + inputString + "\"", conn);
        reader = command.ExecuteReader();

        if (reader.Read()) {
            group.SetActive(true);
            failed.enabled = false;
            aircraft.text = inputField.text;
            wing.text = reader.GetString("wingspan") + " m";
            length.text = reader.GetString("lengths") + " m";
            weight.text = reader.GetString("weights") + " Kg";
        }


        reader.Close();
    }

    /// <summary>
    /// Delete包装方法
    /// </summary>
    public void Delete() {
        Delete(inputField.text);
    }
    /// <summary>
    /// 根据输入框信息删除记录
    /// </summary>
    /// <param name="inputString"></param>
    public void Delete(string inputString)
    {
        failed.enabled = true;
        failed.text = "删除失败，不存在机型：" + inputString;
        group.SetActive(false);
        
        MySqlDataReader reader = null;
        MySqlCommand searchCommand = new MySqlCommand("select * from aircraft where names=\"" + inputString + "\"", conn);
        
        // 先查询是否在库中
        reader = searchCommand.ExecuteReader();

        if (reader.Read()) {
            searchCommand.CommandText = "delete from aircraft where names=\"" + inputString + "\"";
            reader.Close();
            
            reader = searchCommand.ExecuteReader();
            failed.text = "成功删除机型：" + inputString;
            failed.color = Color.green;

        }
        reader.Close();
    }

    /// <summary>
    /// 添加按钮
    /// </summary>
    public void Add() {
        group.SetActive(false);
        AddPopout.SetActive(true);
    }
    
    /// <summary>
    /// 修改按钮
    /// </summary>
    public void Mod() {
        group.SetActive(false);
        ModPopout.SetActive(true);
    }

    /// <summary>
    /// 提交按钮
    /// </summary>
    public void Submit(string code) {
        failed.enabled = true;
        
        if (code == "add") {
            // 检查是否存在与库中
            MySqlCommand command = new MySqlCommand("SELECT * FROM aircraft WHERE NAMES=\"" + AddAircraftInputField.text + "\"", conn);
            MySqlDataReader reader = null;
            reader = command.ExecuteReader();

            if (reader.Read()) {
                failed.color = Color.red;
                failed.text = "添加失败，已存在机型：" + AddAircraftInputField.text;
                Cancel("add");
                reader.Close();
                return;
            }

            reader.Close();
            command.CommandText = "INSERT INTO aircraft (names, lengths, wingspan, weights) VALUES(\"" 
                                  + AddAircraftInputField.text + "\", " + AddLengthInputField.text + ", " + AddWingInputField.text + "," + AddWeightInputField.text + ")";
            reader = command.ExecuteReader();
            failed.color = Color.green;
            failed.text = "成功添加机型：" + AddAircraftInputField.text;
            reader.Close();
            

        }
        
        if (code == "mod") {
            // 检查是否存在与库中
            MySqlCommand command = new MySqlCommand("SELECT * FROM aircraft WHERE NAMES=\"" + ModAircraftInputField.text + "\"", conn);
            MySqlDataReader reader = null;
            reader = command.ExecuteReader();

            if (reader.Read()) {
                reader.Close();
                command.CommandText = "UPDATE aircraft SET " +
                                      "lengths=" + ModLengthInputField.text + ", " +
                                      "wingspan=" + ModWingInputField.text + ", " +
                                      "weights=" +  ModWeightInputField.text + 
                                      " WHERE names =\"" + ModAircraftInputField.text + "\"";
                Debug.Log(command.CommandText);
                reader = command.ExecuteReader();
                failed.color = Color.green;
                failed.text = "成功修改信息";
                reader.Close();
                ModPopout.SetActive(false);
                return;
            }

            
            failed.color = Color.red;
            failed.text = "修改失败，不存在机型：" + ModAircraftInputField.text;
            Cancel("add");
            reader.Close();
            ModPopout.SetActive(false);
            return;
        }
        
        AddPopout.SetActive(false);
        ModPopout.SetActive(false);
    }

    /// <summary>
    /// 弹出窗口中的取消按钮
    /// </summary>
    /// <param name="code"></param>
    public void Cancel(string code) {
        if (code == "add") {
            AddPopout.SetActive(false);
        }

        if (code == "mod") {
            ModPopout.SetActive(false);
        }
    }
    
    
}
