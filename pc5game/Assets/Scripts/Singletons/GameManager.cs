using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public AudioSource bgAudioSource;
    public AudioClip bgAudioClip;
    [SerializeField] private Animator animator;

    private int sceneToLoad;

    public class SceneRef
    {
        public string name;
        public int index;
    }

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

    public void FadeToScene(int index)
    {
        sceneToLoad = index;
        animator.SetBool("FadeOut", true);
        // Calls OnFadeComplete after animation
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
        FadeIn();
    }

    public void FadeIn()
    {
        animator.SetBool("FadeOut", false);
    }
}
