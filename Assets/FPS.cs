using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    private float fps;

    private void Update()
    {
        fps = 1 / Time.unscaledDeltaTime;
    }

    private void OnGUI()
    {
        GUILayout.Box($"FPS: {fps}");
    }
}
