using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogFaceAnim : MonoBehaviour
{
    public Sprite[] Animation;
    public float AnimTime;
    [SerializeField]
    float CycleTime;
    [SerializeField]
    Image MyImg;

    float Timer;
    float AnimTimer;
    float KeyFrameOffset;
    private void Awake()
    {
        RecalculateKeyFrameOffset();
    }
    public void RecalculateKeyFrameOffset()
    {
        AnimTimer = 0;
        KeyFrameOffset = 1 / (float)Animation.Length;
    }

    private void Update()
    {
        Timer = Mathf.PingPong(Time.time, CycleTime);
        AnimTimer = Mathf.PingPong(Time.time, AnimTime);

        float Percent = Timer / CycleTime;
        Percent = Easing.Quadratic.InOut(Percent);
        float AnimPercent = AnimTimer / AnimTime;

        int ImgNum = (int)(AnimPercent / KeyFrameOffset);
        MyImg.sprite = Animation[ImgNum];
        MyImg.gameObject.transform.eulerAngles = new Vector3(0, 0, (-0.5f + Percent) * 2 * 5); 
    }
}
