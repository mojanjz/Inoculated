using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip attackSound;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnAttack()
    {
        audioSource.clip = attackSound;
        audioSource.Play();
    }
}
