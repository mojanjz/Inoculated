using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

/* DialogueManager will iterate over an sentences contained in a Dialogue, 
 * proceeding to the next sentence in the queue when prompted. It controls UI 
 * display of the dialogue. */
public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private KeyMap[] keyMap; // TEMP CODE, NEED TO FIX LATER

    [SerializeField] private Animator panelAnimator;
    [SerializeField] private int panelAnimLayer = 0;
    [SerializeField] private Text speakerNameUI;
    [SerializeField] private Text sentenceUI;
    [SerializeField] private Text arrowUI;
    [SerializeField] private float letterDelay = 0.05f;

    private Queue<string> sentenceQ = new Queue<string>();
    private Coroutine typingCoroutine = null;
    private string currSentence;

    public UnityEvent OnEndDialogueEvent;

    // (Optional) Prevent non-singleton constructor use.
    protected DialogueManager() { }

    void Awake()
    {
        enabled = false;

        if (OnEndDialogueEvent == null)
        {
            OnEndDialogueEvent = new UnityEvent();
        }

        /* Set initial text to nothing. */
        speakerNameUI.text = sentenceUI.text = arrowUI.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyMap[0].NextSentence) ||
            Input.GetKeyDown(keyMap[1].NextSentence))
        {
            UpdateSentence();
        }
    }

    /* Method for starting a dialogue.
     * PARAM: dialogue, the dialogue to start
     * EXCEPTIONS: If another dialogue is already in process, NoInterruptException will be thrown. */
    public IEnumerator StartDialogue(Dialogue dialogue)
    {
        /* Don't interrupt a dialogue process that is running. */
        if (enabled)
        {
            throw new NoInterruptException("Could not start new dialogue. Dialogue panel is already in active process.");
        }

        /* Start calling Update() each frame. */
        enabled = true;

        speakerNameUI.text = dialogue.Speaker;

        panelAnimator.SetBool("IsOpen", true);

        foreach (string sentence in dialogue.Sentences)
        {
            sentenceQ.Enqueue(sentence);
        }

        /* Setting the IsOpen parameter doesn't immediately activate the transition, so we have to wait. */
        float seconds;
        while( (seconds = panelAnimator.GetAnimatorTransitionInfo(panelAnimLayer).duration) == 0)
        {
            yield return null; // Wait a frame, for each frame that the transition isn't active
        }   

        /* Wait for the dialogue panel to open fully before continuing. */
        yield return new WaitForSeconds(seconds);

        UpdateSentence();
    }

    public class NoInterruptException : Exception
    {
        public NoInterruptException (string message) : base(message) { }
    }

    public void UpdateSentence()
    {
        /* Disable arrows at the start. */
        arrowUI.text = "";

        /* If typing animation is still playing, stop typing and display the whole sentence. */
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            sentenceUI.text = currSentence;
            UpdateArrow();
            return;
        }

        /* Otherwise, if typing animation was already finished, begin typing the next sentence. */
        LoadNextSentence();
        if(currSentence != "")
        {
            typingCoroutine = StartCoroutine(TypeSentence(currSentence));
        }
    }

    public void LoadNextSentence()
    {
        /* End dialogue when there are no more sentences. */
        if (sentenceQ.Count == 0)
        {
            currSentence = "";
            StartCoroutine(EndDialogue());
            return;
        }

        currSentence = sentenceQ.Dequeue();
    }

    public IEnumerator TypeSentence(string sentence)
    {
        sentenceUI.text = "";
        
        foreach ( char letter in sentence.ToCharArray())
        {
            sentenceUI.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }

        UpdateArrow();

        typingCoroutine = null;
    }

    public void UpdateArrow()
    {
        if (sentenceQ.Count == 0)
        {
            /* If there are no sentences left in the dialogue, don't display arrows. */
            arrowUI.text = "";
        }
        else
        {
            /* If there are more sentences left in the dialogue, display arrows. */
            arrowUI.text = ">>";
        }
    }

    public IEnumerator EndDialogue()
    {
        /* Probably a lazy solution, but I'm delaying until end of frame before 
         * setting dialogue panel free... to prevent reading double input lol. */
        yield return new WaitForEndOfFrame();

        Debug.Log("Dialogue ended.");
        enabled = false;
        speakerNameUI.text = sentenceUI.text = arrowUI.text = "";
        panelAnimator.SetBool("IsOpen", false);
        OnEndDialogueEvent.Invoke();
    }
}
