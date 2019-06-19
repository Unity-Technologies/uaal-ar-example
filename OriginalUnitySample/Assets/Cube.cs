﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine;

public class NativeAPI {
    [DllImport("__Internal")]
    public static extern void showHostMainWindow(string lastStringColor);
}

public class Cube : MonoBehaviour
{
    public Text text;    
    void appendToText(string line) { text.text += line + "\n"; }

    void Update()
    {
        transform.Rotate(0, Time.deltaTime*10, 0);
    }

    string lastStringColor = "";
    void ChangeColor(string newColor)
    {
        appendToText( "Chancing Color to " + newColor );

        lastStringColor = newColor;
    
        if (newColor == "red") GetComponent<Renderer>().material.color = Color.red;
        else if (newColor == "blue") GetComponent<Renderer>().material.color = Color.blue;
        else if (newColor == "yellow") GetComponent<Renderer>().material.color = Color.yellow;
        else GetComponent<Renderer>().material.color = Color.black;
    }


    void showHostMainWindow()
    {
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.company.product.OverrideUnityActivity");
            AndroidJavaObject overrideActivity = jc.GetStatic<AndroidJavaObject>("instance");
            overrideActivity.Call("showMainActivity", lastStringColor);
        } catch(Exception e)
        {
            appendToText("Exception during showHostMainWindow");
            appendToText(e.Message);
        }
#elif UNITY_IOS
        NativeAPI.showHostMainWindow(lastStringColor);
#endif
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 100), "Red")) ChangeColor("red");
        if (GUI.Button(new Rect(10, 110, 200, 100), "Blue")) ChangeColor("blue");
        if (GUI.Button(new Rect(10, 300, 400, 100), "Show Main With Color")) showHostMainWindow();
    }
}

