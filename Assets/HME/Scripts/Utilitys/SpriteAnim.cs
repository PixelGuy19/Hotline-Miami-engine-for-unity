using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnim : MonoBehaviour
{
    [SerializeField]
    bool PlayAtStart = true;
    public Sprite[] Sprites;
    public float FrameTime;
    SpriteRenderer Renderer;
    bool IsPaused = false;
    private void Start()
    {
        if (PlayAtStart) { Play(); }
    }
    public void Play()
    {
        if (Renderer == null) { Renderer = GetComponent<SpriteRenderer>(); }
        IsPaused = false;

        if (!AnimatorCreated)
        {
            StartCoroutine(Animator());
            AnimatorCreated = true;
        }
    }
    public void Pause()
    {
        IsPaused = true;
    }
    public void Restart()
    {
        Stop();
        Play();
    }

    public UnityEvent OnStop;
    public void Stop()
    {
        StopAllCoroutines();
        OnStop.Invoke();
        AnimatorCreated = false;
        IsPaused = true;
    }

    [SerializeField]
    public bool StopAfterAnim;
    bool AnimatorCreated = false;
    IEnumerator Animator()
    {
        while (true)
        {
            for (int i = 0; i < Sprites.Length; i++)
            {
                Renderer.sprite = Sprites[i];
                yield return new WaitForSeconds(FrameTime);
                while (IsPaused)
                {
                    yield return 0;
                }
            }
            if (StopAfterAnim) { Stop(); break; }
        }
    }
    private void OnValidate()
    {
        if (FrameTime <= 0)
        {
            FrameTime = 1;
        }
    }
}
