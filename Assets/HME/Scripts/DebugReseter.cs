﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugReseter : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}
