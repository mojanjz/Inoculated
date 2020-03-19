using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using XNode;

/* DialogueManager will iterate over sentences contained in a Dialogue, 
 * proceeding to the next sentence in the queue when prompted. It controls UI 
 * display of the dialogue. */
public class DialogueManager : Singleton<DialogueManager>
{
    // Class that stores all the UI references to a particular dialogue panel
    [Serializable] class PanelSet
    {
        [SerializeField] internal Animator panelAnimator;
        [SerializeField] internal Text speakerNameUI;
        [SerializeField] internal Text sentenceUI;
        [SerializeField] internal Text arrowUI;
        [SerializeField] internal RectTransform SelectionBox;
        [SerializeField] internal Text[] ChoiceText;
        internal Vector2[] ChoiceTextPos; // Array to store the center position of the choice text boxes.
    }

    [SerializeField] private PanelSet brother;
    [SerializeField] private PanelSet sister;
    [SerializeField] private CharacterStats brotherStats; // Need to retrieve speaker name
    [SerializeField] private CharacterStats sisterStats; // Need to retrieve speaker name
    [SerializeField] private float letterDelay = 0.05f;
    [SerializeField] private int panelAnimLayer = 0;

    /* Transient data */
    private CharacterStats playerStats = null;
    private CharacterStats interactableStats = null;
    private PanelSet activePanel = null; // Only one panel should be active at a time
    private KeyCode selectChoiceKey = KeyCode.None;
    private KeyCode prevChoiceKey = KeyCode.None;
    private KeyCode nextChoiceKey = KeyCode.None;
    private int choiceIndex = 0;
    private int numChoices = 0;
    private DialogueNode currDialogueNode = null;
    private Queue<string> sentenceQ = new Queue<string>();
    private Coroutine typingCoroutine = null;
    private string currSentence = null;

    public class DialogueNodeEvent : UnityEvent<DialogueNode> { }
    public DialogueNodeEvent OnEndDialogueEvent;

    // (Optional) Prevent non-singleton constructor use.
    protected DialogueManager() { }

    protected override void Awake()
    {
        base.Awake();

        enabled = false;

        if (OnEndDialogueEvent == null)
        {
            OnEndDialogueEvent = new DialogueNodeEvent();
        }

        RectTransform tempRectTrans;
        int i;
        foreach (PanelSet panel in new PanelSet[] { brother, sister })
        {
            panel.ChoiceTextPos = new Vector2[panel.ChoiceText.Length];

            panel.speakerNameUI.gameObject.SetActive(true);
            panel.sentenceUI.gameObject.SetActive(true);
            panel.arrowUI.gameObject.SetActive(true);
            panel.SelectionBox.gameObject.SetActive(false);

            // Set initial text to nothing.
            panel.speakerNameUI.text = panel.sentenceUI.text = panel.arrowUI.text = "";
      
            i = 0;
            foreach (Text text in panel.ChoiceText)
            {
                text.text = "";
                text.gameObject.SetActive(false);

                /* Store the position of the choice text boxes so we can move the
                 * selection box to match them when the user moves the selection box. */
                tempRectTrans = text.gameObject.GetComponent<RectTransform>();

                if (tempRectTrans == null)
                {
                    throw new MissingComponentEx(text.gameObject.name, "RectTransform");
                }

                panel.ChoiceTextPos[i] = tempRectTrans.anchoredPosition;

                i++;
            }
        }
    }

    public class MissingComponentEx : Exception
    {
        public MissingComponentEx(string gameObj, string component) : base(gameObj + " does not have a " + component) { }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(selectChoiceKey))
        {
            // UpdateSentence();
            SelectChoice();
        }

        else if (Input.GetKeyDown(prevChoiceKey) && (numChoices != 0))
        {
            PrevChoice();
        }

        else if (Input.GetKeyDown(nextChoiceKey) && (numChoices != 0))
        {
            NextChoice();
        }
    }

    /* Method for starting a dialogue.
     * PARAM: dialogue, the dialogue to start
     * PARAM: selectKey, the key to use to update the dialogue (ie. show next sentence)
     * EXCEPTION: If another dialogue is already in process, NoInterruptEx will be thrown.
     * EXCEPTION: If the panel is not valid, UnknownPanelEx will be thrown.*/
    public void StartDialogue(Dialogue dialogue, KeyCode selectKey, KeyCode prevKey, KeyCode nextKey,
        CharacterStats player = null, CharacterStats interactable = null)
    {
        InitializeActivePanel(dialogue, selectKey, prevKey, nextKey, player: player, interactable: interactable);
        activePanel.speakerNameUI.text = GetSpeakerName(dialogue);

        foreach (string sentence in dialogue.Sentences)
        {
            sentenceQ.Enqueue(sentence);
        }

        StartCoroutine(OpenActivePanel());
    }

    /* Overload that uses a DialogueNode instead of a Dialogue. */
    public void StartDialogue(DialogueNode dialogueNode, KeyCode selectKey, KeyCode prevKey, KeyCode nextKey, 
         CharacterStats player = null, CharacterStats interactable = null, bool newInteraction = true)
    {
        currDialogueNode = dialogueNode; // Store the node so we can access the connections later
        Dialogue dialogue = dialogueNode.Dialogue;

        /* If this is not a continuation of a dialogue tree that is still in
         * progress, it's a new dialogue interaction so we need to initialize
         * again. */
        if (newInteraction)
        {
            InitializeActivePanel(dialogue, selectKey, prevKey, nextKey, player: player, interactable: interactable);
        }

        activePanel.speakerNameUI.text = GetSpeakerName(dialogue);

        foreach (string sentence in dialogue.Sentences)
        {
            sentenceQ.Enqueue(sentence);
        }

        if (newInteraction)
        {
            // If it's a new interaction, open the panel and update sentence
            StartCoroutine(OpenActivePanel());
        } else
        {
            UpdateSentence();
        }
    }

    private void InitializeActivePanel(Dialogue dialogue, 
        KeyCode selectKey, KeyCode prevKey, KeyCode nextKey, 
        CharacterStats player = null, CharacterStats interactable = null)
    {
        /* Don't interrupt a dialogue process that is running. */
        if (enabled)
        {
            throw new NoInterruptEx("Could not start new dialogue. " +
                "Dialogue panel is already in active process.");
        }

        this.playerStats = player;
        this.interactableStats = interactable;
        activePanel = GetPanelSet(dialogue.Panel);

        selectChoiceKey = selectKey;
        prevChoiceKey = prevKey;
        nextChoiceKey = nextKey;

        /* Start calling Update() each frame. */
        enabled = true;
    }

    private PanelSet GetPanelSet(Dialogue.PanelID panelID)
    {
        PanelSet panel;

        if ( panelID == Dialogue.PanelID.Player)
        {
            panelID = playerStats.PanelID;
        }

        switch (panelID)
        {
            case Dialogue.PanelID.Sister:
                panel = sister;
                break;
            case Dialogue.PanelID.Brother:
                panel = brother;
                break;
            default:
                StartCoroutine(EndDialogue());
                throw new UnknownPanelEx(panelID);
        }

        return panel;
    }

    public class UnknownPanelEx : Exception
    {
        public UnknownPanelEx(Dialogue.PanelID panelID) : base(panelID.ToString() + " does not exist.") { }
    }

    // Returns the string name associated with the selected Speaker.
    public string GetSpeakerName (Dialogue dialogue)
    {
        string name = null;

        switch (dialogue.Speaker.SpeakerChoice)
        {
            case SpeakerRef.Speaker.Custom:
                name = dialogue.Speaker.Custom;
                break;

            case SpeakerRef.Speaker.ThisObject:
                if (interactableStats == null)
                {
                    throw new UnknownSpeakerNameEx(SpeakerRef.Speaker.ThisObject);
                }
                name = interactableStats.SpeakerName;
                break;

            case SpeakerRef.Speaker.Player:
                if (playerStats == null)
                {
                    throw new UnknownSpeakerNameEx(SpeakerRef.Speaker.Player);
                }
                name = playerStats.SpeakerName;
                break;

            case SpeakerRef.Speaker.Brother:
                name = brotherStats.SpeakerName;
                break;

            case SpeakerRef.Speaker.Sister:
                name = sisterStats.SpeakerName;
                break;
        }

        return name;
    }

    public class UnknownSpeakerNameEx: Exception
    {
        public UnknownSpeakerNameEx(SpeakerRef.Speaker speaker) : base(string.Format("Unknown name associated with {0} speaker.", speaker)) { }
    }


    /* Coroutine that animates the dialogue panel opening. 
     * I separated this code from the StartDialogue() method so it can be 
     * a regular method. This way, other code calling StartDialogue() can catch 
     * the exceptions. (If StartDialogue() was a coroutine, it'd be more complicated.) */
    public IEnumerator OpenActivePanel()
    {
        activePanel.panelAnimator.SetBool("IsOpen", true);

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

    public class NoInterruptEx : Exception
    {
        public NoInterruptEx(string message) : base(message) { }
    }

    /* Fully displays the current sentence if it's still typing. Or, starts 
     * typing the next sentence. */
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
            UpdateArrows();
        }

        /* Otherwise, begin typing the next sentence. */
        else
        {
            ShowNextSentence();
        }
    }

    public void ShowNextSentence()
    {
        // Show next sentence
        if (sentenceQ.Count > 0)
        {
            currSentence = sentenceQ.Dequeue();
            typingCoroutine = StartCoroutine(TypeSentence(currSentence));
        }

        // Show choices if it's the end of the dialogue
        else
        {
            currSentence = "";
            DisplayChoiceUI();
        }
    }

    // Clears the sentence display, then animates typing of the given sentence.
    public IEnumerator TypeSentence(string sentence)
    {
        activePanel.sentenceUI.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            activePanel.sentenceUI.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }

        UpdateArrows();
        typingCoroutine = null;
    }

    // Display arrows indicating to player if there are more sentences in the dialogue
    public void UpdateArrows()
    {
        // If there are more sentences, or choices available, display arrows
        if (sentenceQ.Count != 0 || (currDialogueNode != null && currDialogueNode.Choices.Length > 0))
        {
            activePanel.arrowUI.text = ">>";
        }
        else
        {
            activePanel.arrowUI.text = "";
        }
    }

    // Display dialogue choices
    public void DisplayChoiceUI()
    {
        // If there are no choices, show nothing and end the dialogue
        if (currDialogueNode == null || currDialogueNode.Choices.Length == 0)
        {
            StartCoroutine(EndDialogue());
            return;
        }

        ClearSpeechText();

        activePanel.speakerNameUI.gameObject.SetActive(false);
        activePanel.sentenceUI.gameObject.SetActive(false);
        activePanel.arrowUI.gameObject.SetActive(false);

        activePanel.SelectionBox.gameObject.SetActive(true);
        choiceIndex = 0; // Start at the first choice
        activePanel.SelectionBox.anchoredPosition = activePanel.ChoiceTextPos[choiceIndex];

        int i = 0;
        numChoices = 0;
        foreach (DialogueNode.ChoiceSet choice in currDialogueNode.Choices)
        {
            activePanel.ChoiceText[i].text = choice.ChoiceText;
            activePanel.ChoiceText[i].gameObject.SetActive(true);

            numChoices++;
            i++;
        }
    }

     public void DisplaySpeechUI(bool state)
    {
        activePanel.speakerNameUI.gameObject.SetActive(state);
        activePanel.sentenceUI.gameObject.SetActive(state);
        activePanel.arrowUI.gameObject.SetActive(state);
    }

    public void PrevChoice()
    {
        choiceIndex = ((--choiceIndex) + numChoices) % numChoices;
        activePanel.SelectionBox.anchoredPosition = activePanel.ChoiceTextPos[choiceIndex];
    }

    public void NextChoice()
    {
        choiceIndex = (++choiceIndex) % numChoices;
        activePanel.SelectionBox.anchoredPosition = activePanel.ChoiceTextPos[choiceIndex];
    }

    public void SelectChoice()
    {
        // If there are no choices, "select" simply moves to the next sentence
        if (numChoices == 0)
        {
            UpdateSentence();
            return;
        }

        bool endAfter = currDialogueNode.Choices[choiceIndex].EndAfter;

        // Get the node port connected to the selected choice
        NodePort port = currDialogueNode.GetOutputPort("Choices " + choiceIndex).Connection;
        if (port != null)
        {
            // Set the connected node as the new active node
            currDialogueNode = port.node as DialogueNode;
        }
        else
        {
            // If no nodes are connected
            currDialogueNode = null;
        }

        // Reset the choice text boxes for next time.
        ResetChoiceUI();
        DisplaySpeechUI(true);

        /* End dialogue if there was no connected node, or if the choice 
         * specifies that the dialogue should end. */
        if (endAfter || currDialogueNode == null)
        {
            StartCoroutine(EndDialogue());
        }
        else
        {
            StartDialogue(currDialogueNode,
                selectChoiceKey, prevChoiceKey, nextChoiceKey, newInteraction: false);
        }
    }

    public IEnumerator EndDialogue()
    {
        /* Probably a lazy solution, but I'm delaying until end of frame before 
         * setting dialogue panel free... to prevent reading double input lol. */
        yield return new WaitForEndOfFrame();

        enabled = false;
        ClearSpeechText();
        ResetChoiceUI();
        activePanel.panelAnimator.SetBool("IsOpen", false);
        OnEndDialogueEvent.Invoke(currDialogueNode);
        ClearDialogueInfo();
    }

    public void ResetChoiceUI()
    {
        int i;
        for (i = 0; i < numChoices; i++)
        {
            activePanel.ChoiceText[i].text = "";
            activePanel.ChoiceText[i].gameObject.SetActive(false);
        }

        activePanel.SelectionBox.gameObject.SetActive(false);

        numChoices = 0;
        choiceIndex = 0;
    }

    public void ClearSpeechText()
    {
        activePanel.speakerNameUI.text = activePanel.sentenceUI.text = activePanel.arrowUI.text = ""; // Set all text to nothing
    }
    public void ClearDialogueInfo()
    {
        currDialogueNode = null;
        playerStats = null;
        interactableStats = null;
        activePanel = null;
        selectChoiceKey = KeyCode.None;
        prevChoiceKey = KeyCode.None;
        nextChoiceKey = KeyCode.None;
        sentenceQ.Clear();
        typingCoroutine = null;
        currSentence = null;
    }
}
