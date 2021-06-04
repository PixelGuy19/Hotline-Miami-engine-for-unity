using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    [SerializeField]
    SoundList SoundList = default;
    AudioSource AudioPlayer;
    private void Awake()
    {
        AudioPlayer = gameObject.GetComponent<AudioSource>();
        AudioMixer Mixer = Resources.Load<AudioMixer>("Master");
        AudioPlayer.outputAudioMixerGroup = Mixer.FindMatchingGroups("Sounds")[0];
    }
    public void PlaySound(string SoundName)
    {
        AudioClip ToPlay = SoundList.Sounds.Where((AudioClip C) => { return C.name == SoundName; }).First();
        if (ToPlay != null)
        {
            AudioPlayer.clip = ToPlay;
            AudioPlayer.Play();
        }
    }
}
