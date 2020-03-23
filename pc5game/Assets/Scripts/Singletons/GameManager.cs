using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    private bool agreeToLeave = false;
    [SerializeField] DialogueTrigger leaveDialogue;

    // (Optional) Prevent non-singleton constructor use.
    protected GameManager() { }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(Instance.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

   public bool AskToLeave()
   {
        return true;
   }
}
