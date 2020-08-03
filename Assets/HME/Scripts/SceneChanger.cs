using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    static SceneChanger Main;
    private void Awake()
    {
        if (Main == null) { Main = this; }
    }

    [SerializeField]
    MusicManager MusicManager;
    [SerializeField]
    GameObject SkylineScreen, ClockScreen;
    public static void LoadScene(string SceneName)
    {
        Main.MusicManager.Lock = true;
        BlackScreen.ShowScreen(Main.SkylineScreen, 1, 5, () =>
        {
            SceneManager.LoadScene(SceneName);
        },
        () =>
        {
            if (ClockScreenUpdater.UpdateClock())
            {
                Main.MusicManager.Lock = false;
                BlackScreen.ShowScreen(Main.ClockScreen, 1, 2);
            }
        });
    }
    public void PLoadScene(string SceneName)
    {
        LoadScene(SceneName);
    }
}