using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Сделать переход между сценами через ShowScreen + Часы
public class BlackScreen : MonoBehaviour
{
    [SerializeField]
    float FadeTime;
    [SerializeField]
    Image Screen;

    static BlackScreen Main;
    private void Awake()
    {
        if (Main == null) { Main = this; }
    }

    public static void FadeOut(float Time = -1)
    {
        Main.Fade(false, Time, false);
    }
    public static void FadeIn(float Time = -1, Action OnBlackScreen = null)
    {
        Main.Fade(true, Time, false, OnBlackScreen);
    }
    public static void FadeInOut(float Time = -1, Action OnBlackScreen = null)
    {
        Main.Fade(true, Time, true, OnBlackScreen);
    }

    float Timer;
    protected void Fade(bool In, float LFadeTime = -1, bool Pong = true, Action OnBlackScreen = null)
    {
        if (LFadeTime != -1)
        {
            FadeTime = LFadeTime;
        }
        if (In)
        {
            Timer = 0;
        }
        else
        {
            Timer = FadeTime;
        }
        StartCoroutine(Fader());

        IEnumerator Fader()
        {
            while (true)
            {
                Timer += Time.deltaTime * (In ? 1 : -1);
                if (Timer > FadeTime || Timer < 0)
                {
                    break;
                }
                float Percent = Timer / FadeTime;
                Color ScreenColor = Color.Lerp(Color.clear, Color.black, Percent);
                Screen.color = ScreenColor;

                yield return null;
            }
            if (In && OnBlackScreen != null)
            {
                OnBlackScreen.Invoke();
            }
            if (Pong)
            {
                Fade(!In, LFadeTime, false);
            }
        }
    }
    public static void ShowScreen(GameObject ToShow, float TrTime = -1, float ShowTime = 2,
        Action Callback = null, Action ExitCallback = null)
    {
        Main.StartCoroutine(ScreenShower());
        IEnumerator ScreenShower()
        {
            PlayerBase.LockControls(true);
            FadeInOut(TrTime, () =>
            {
                ToShow.SetActive(true);
                if (Callback != null)
                {
                    Callback.Invoke();
                }
            });


            yield return new WaitForSeconds(TrTime + ShowTime);

            FadeInOut(TrTime, () =>
            {
                ToShow.SetActive(false);
                PlayerBase.LockControls(false);
                if (ExitCallback != null)
                {
                    ExitCallback.Invoke();
                }
            });
        }
    }
}