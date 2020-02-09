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
    // Class that stores all the UI references to a particular dialogue panel
    [Serializable]
    class PanelSet
    {
        [SerializeField] internal Animator panelAnimator;
        [SerializeField] internal Text speakerNameUI;
        [SerializeField] internal Text sentenceUI;
        [SerializeField] internal Text arrowUI;
    }

    [SerializeField] private PanelSet brother;
    [SerializeField] private PanelSet sister;
    [SerializeField] private float letterDelay = 0.05f;
    [SerializeField] internal int panelAnimLayer = 0;

    /* Transient data */
    private PanelSet activePanel;
    private Queue<string> sentenceQ = new Queue<string>();
    private Coroutine typingCoroutine = null;
    private string currSentence = null;
    private KeyCode nextSentence = KeyCode.None;

    public UnityEvent OnEndDialogueEvent;

    // (Optional) Prevent non-singleton constructor use.
    protected DialogueManager() { }

    protected override void Awake()
    {
        base.Awake();

        enabled = false;

        if (OnEndDialogueEvent == null)
        {
            OnEndDialogueEvent = new UnityEvent();
        }

        /* Set initial text to nothing. */
        brother.speakerNameUI.text = brother.sentenceUI.text = brother.arrowUI.text = "";
        sister.speakerNameUI.text = sister.sentenceUI.text = sister.arrowUI.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(nextSentence))
        {
            UpdateSentence();
        }
    }

    /* Method for starting a dialogue.
     * PARAM: dialogue, the dialogue to start
     * PARAM: engager, the player GameObject that engaged the dialogue
     * EXCEPTIONS: If another dialogue is already in process, NoInterruptException will be thrown. */
    public void StartDialogue(Dialogue dialogue, GameObject engager)
    {
        /* Don't interrupt a dialogue process that is running. */
        if (enabled)
        {
            throw new NoInterruptException("Could not start new dialogue. Dialogue panel is already in active process.");
        }

        /* Start calling Update() each frame. */
        enabled = true;

        /* Get the key mapping and panel that's relevant to the current engager. */
        nextSentence = engager.GetComponent<KeyMap>().NextSentence;

        if (engager.name == "PlayerSis")
        {
            activePanel = sister;
        } else if (engager.name == "PlayerBro")
        {
            activePanel = brother;
        } else
        {
            Debug.Log("Uh, player unidentifiable, cannot choose a panel to use...");
        }
        
        activePanel.speakerNameUI.text = dialogue.Speaker;

        activePanel.panelAnimator.SetBool("IsOpen", true);

        foreach (string sentence in dialogue.Sentences)
        {
            sentenceQ.Enqueue(sentence);
        }

        StartCoroutine(StartDialogueHelper());
    }

    /* Coroutine that animates the dialogue panel opening. 
     * I separated this code from the main StartDialogue() method so it can be 
     * a regular method. This way, other code calling StartDialogue() can catch 
     * the exception. (If StartDialogue() was a coroutine, it'd be more complicated.) */
    public IEnumerator StartDialogueHelper()
    {
        /* Setting the IsOpen parameter doesn't immediately activate the transition, so we have to wait. */
        float seconds;
        while ((seconds = activePanel.panelAnimator.GetAnimatorTransitionInfo(panelAnimLayer).duration) == 0)
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
        activePanel.arrowUI.text = "";

        /* If typing animation is still playing, stop typing and display the whole sentence. */
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            activePanel.sentenceUI.text = currSentence;
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
        activePanel.sentenceUI.text = "";
        
        foreach ( char letter in sentence.ToCharArray())
        {
            activePanel.sentenceUI.text += letter;
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
            activePanel.arrowUI.text = "";
        }
        else
        {
            /* If there are more sentences left in the dialogue, display arrows. */
            activePanel.arrowUI.text = ">>";
        }
    }

    public IEnumerator EndDialogue()
    {
        /* Probably a lazy solution, but I'm delaying until end of frame before 
         * setting dialogue panel free... to prevent reading double input lol. */
        yield return new WaitForEndOfFrame();

        Debug.Log("Dialogue ended.");
        enabled = false;
        activePanel.speakerNameUI.text = activePanel.sentenceUI.text = activePanel.arrowUI.text = ""; // Set all text to nothing
        activePanel.panelAnimator.SetBool("IsOpen", false);
        OnEndDialogueEvent.Invoke();
    }
}
