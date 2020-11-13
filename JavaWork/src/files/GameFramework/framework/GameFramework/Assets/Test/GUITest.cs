using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITest : MonoBehaviour
{
    public Texture2D icon;
    void OnGUI()
    {
        GUI.Button(new Rect(10, 10, 100, 20), new GUIContent("Click me", 
            icon, "This is the tooltip"));
        GUI.Label(new Rect(10, 40, 100, 20), GUI.tooltip);
    }
}
