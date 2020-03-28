using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public AudioSource bgAudioSource;
    public AudioClip bgAudioClip;

    // (Optional) Prevent non-singleton constructor use.
    protected GameManager() { }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(Instance.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        bgAudioSource.clip = bgAudioClip;
        bgAudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
