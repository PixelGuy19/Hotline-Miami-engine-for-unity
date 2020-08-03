using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogManager : MonoBehaviour
{
    static DialogManager MainManager;
    private void Awake()
    {
        MainManager = this;
        SceneManager.activeSceneChanged += (Scene CurS, Scene NewS) => { MainManager = this; };
    }
    private void OnLevelWasLoaded(int level)
    {
        MainManager = this;
    }

    [SerializeField]
    Text Content;
    [SerializeField]
    DialogFaceAnim Face;
    [SerializeField]
    GameObject MessageDialogObj;
    LocalizedMessage CurrentMessage;
    public static void LoadDialog(LocalizedMessage Message = null, Action Callback = null)
    {
        if(Message == null)
        {
            MainManager.MessageDialogObj.SetActive(false);
            PlayerBase.LockControls(false);
            if(Callback != null)
            {
                Callback.Invoke();
            }
            return;
        }
        PlayerBase.LockControls(true);
        MainManager.CurrentMessage = Message;
        MainManager.MessageDialogObj.SetActive(true);
        MainManager.Face.RecalculateKeyFrameOffset();
        MainManager.Face.Animation = Message.Face.Sprites;
        MainManager.Content.text = Message.GetText();
        MainManager.Face.AnimTime = Message.AnimTime;
        MainManager.Content.color = Color.clear;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && CurrentMessage != null)
        {
            LoadDialog(CurrentMessage.NextMessage);
        }
    }
}