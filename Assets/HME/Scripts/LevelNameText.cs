﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelNameText : MonoBehaviour
{
    [SerializeField]
    Text MyText = default;
    private void Start()
    {
        MyText.text = SceneManager.GetActiveScene().name;
    }
}