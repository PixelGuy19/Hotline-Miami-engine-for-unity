using UnityEngine;
using UnityEngine.UI;

public class RainbowShadow : MonoBehaviour
{
    [SerializeField]
    Text MyText = default;
    [SerializeField]
    Shadow MyShadow = default;
    [SerializeField]
    float CycleTime = 1;
    [SerializeField]
    Gradient ColourScheme = default;
    [SerializeField]
    Vector2 Offset = default;
    float Timer = 0;
    private void Update()
    {
        Timer = Mathf.PingPong(Time.time, CycleTime);

        float Percent = Timer / CycleTime;

        MyShadow.effectColor = ColourScheme.Evaluate(Percent);
        MyShadow.effectDistance = Offset * Percent;
        MyText.color += new Color(0.075f, 0.075f, 0.075f, 0.075f);
    }
}
