using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    static MusicManager Main;
    public static MusicManager GetManager()
    {
        return Main;
    }
    private void Awake()
    {
        if (Main == null) { Main = this; }
        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        LevelAudioData Data = FindObjectOfType<LevelAudioData>();
        if (Data == null) { return; }

        Intro = Data.Intro;
        Level = Data.Level;
        Outro = Data.Outro;
    }

    [HideInInspector]
    public AudioClip Intro, Level, Outro;
    private void Start()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    [SerializeField]
    AudioSource MusicSource = default;
    public bool Lock;

    private void Update()
    {
        if (Lock) { return; }

        if (FloorManager.GetFloorIndex() < FloorManager.GetFirstFloorIndex())
        {
            ChangeMusic(Intro);
        }
        else if (FloorManager.GetFloorIndex() > FloorManager.GetLastFloorIndex())
        {
            ChangeMusic(Outro);
        }
        else
        {
            ChangeMusic(Level);
        }
    }
    void ChangeMusic(AudioClip Clip)
    {
        if (Clip == null) { return; }
        if (MusicSource.clip != Clip)
        {
            MusicSource.clip = Clip;
            MusicSource.Play();
        }
    }
}
