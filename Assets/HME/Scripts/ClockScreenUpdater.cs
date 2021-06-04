using UnityEngine;
using UnityEngine.UI;

public class ClockScreenUpdater : MonoBehaviour
{
    static ClockScreenUpdater Main;
    private void Awake()
    {
        Main = this;
        gameObject.SetActive(false);
    }

    [SerializeField]
    Text TimeText = default, PlaceText = default;
    public static bool UpdateClock()
    {
        PlaceAndTimeData Data = FindObjectOfType<PlaceAndTimeData>();
        if (Data == null) { return false; }

        Main.TimeText.text = Data.Time;
        Main.PlaceText.text = Data.Place;

        return true;
    }
}