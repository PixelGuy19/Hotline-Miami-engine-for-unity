using UnityEngine;

public class GameLoader : MonoBehaviour
{
    [SerializeField]
    GameObject[] UniqueDontDestroyOnLoadObjects;
    [SerializeField]
    string FirstGameSceneName = "SampleScene";
    [SerializeField]
    bool AutoLoadWhenLoaded;

    static bool GameLoaded = false;
    private void Awake()
    {
        if (GameLoaded)
        {
            if (AutoLoadWhenLoaded) { GoToFirstScene(); }
            return;
        }

        foreach (GameObject Obj in UniqueDontDestroyOnLoadObjects)
        {
            DontDestroyOnLoad(Obj);
        }
        GameLoaded = true;

        if (AutoLoadWhenLoaded) { GoToFirstScene(); }
    }
    public void GoToFirstScene()
    {
        SceneChanger.LoadScene(FirstGameSceneName);
    }
}
