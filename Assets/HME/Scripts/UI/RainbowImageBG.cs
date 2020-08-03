using UnityEngine;
using UnityEngine.UI;

public class RainbowImageBG : MonoBehaviour
{
    [SerializeField]
    Image MyImg;
    [SerializeField]
    Gradient ColourScheme;
    [SerializeField]
    float CycleTime;
    float Timer;
    private void Update()
    {
        Timer = Mathf.PingPong(Time.time, CycleTime);

        float Percent = Timer / CycleTime;

        MyImg.color = ColourScheme.Evaluate(Percent);
    }
}
