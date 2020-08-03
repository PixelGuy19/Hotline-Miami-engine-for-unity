using UnityEngine;

[CreateAssetMenu(fileName = "new Sound List", menuName = "HME/Sound list")]
public class SoundList : ScriptableObject
{
    public AudioClip[] Sounds;
}