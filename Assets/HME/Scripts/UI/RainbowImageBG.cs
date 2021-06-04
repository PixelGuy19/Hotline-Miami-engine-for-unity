using UnityEngine;
using UnityEngine.UI;

public class RainbowImageBG : MonoBehaviour
{
    [SerializeField]
    Image MyImg = default;
    [SerializeField]
    Gradient ColourScheme = default;
    [SerializeField]
    float CycleTime = 1;
    float Timer;
    private void Update()
    {
        Timer = Mathf.PingPong(Time.time, CycleTime);

        float Percent = Timer / CycleTime;

        MyImg.color = ColourScheme.Evaluate(Percent);
    }
}
