using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgAudioController : MonoBehaviour
{
    public AudioSource bgAudioSource;
    public AudioClip bgAudioClip;

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
