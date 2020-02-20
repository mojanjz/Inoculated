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
        [SerializeField] internal string panelName;
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
    [SerializeField] private float letterDelay = 0.05f;
    [SerializeField] private int panelAnimLayer = 0;

    /* Transient data */
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
     * PARAM: panelName, the name of the dialogue panel to use
     * PARAM: selectKey, the key to use to update the dialogue (ie. show next sentence)
     * EXCEPTION: If another dialogue is already in process, NoInterruptEx will be thrown.
     * EXCEPTION: If the panelName is not valid, UnknownPanelNameEx will be thrown.*/
    public void StartDialogue(Dialogue dialogue, string panelName, KeyCode selectKey, KeyCode prevKey, KeyCode nextKey)
    {
        InitializeActivePanel(panelName, selectKey, prevKey, nextKey);

        activePanel.speakerNameUI.text = dialogue.Speaker;

        foreach (string sentence in dialogue.Sentences)
        {
            sentenceQ.Enqueue(sentence);
        }

        StartCoroutine(OpenActivePanel());
    }

    /* Overload that uses a DialogueNode instead of a Dialogue. */
    public void StartDialogue(DialogueNode dialogueNode, string panelName, KeyCode selectKey, KeyCode prevKey, KeyCode nextKey, bool newInteraction = true)
    {
        /* If this is not a continuation of a dialogue tree that is still in
         * progress, it's a new dialogue interaction so we need to initialize
         * again. */
        if (newInteraction)
        {
            InitializeActivePanel(panelName, selectKey, prevKey, nextKey);
        }

        activePanel.speakerNameUI.text = dialogueNode.Dialogue.Speaker;

        foreach (string sentence in dialogueNode.Dialogue.Sentences)
        {
            sentenceQ.Enqueue(sentence);
        }

        // Store the node so we can access the connections later
        currDialogueNode = dialogueNode;


        if (newInteraction)
        {
            // If it's a new interaction, open the panel and update sentence
            StartCoroutine(OpenActivePanel());
        } else
        {
            UpdateSentence();
        }
    }

    private void InitializeActivePanel(string panelName, KeyCode selectKey, KeyCode prevKey, KeyCode nextKey)
    {
        /* Don't interrupt a dialogue process that is running. */
        if (enabled)
        {
            throw new NoInterruptEx("Could not start new dialogue. " +
                "Dialogue panel is already in active process.");
        }

        if (panelName == sister.panelName)
        {
            activePanel = sister;
        }
        else if (panelName == brother.panelName)
        {
            activePanel = brother;
        }
        else
        {
            throw new UnknownPanelNameEx(panelName);
        }

        selectChoiceKey = selectKey;
        prevChoiceKey = prevKey;
        nextChoiceKey = nextKey;

        /* Start calling Update() each frame. */
        enabled = true;
    }

    public class UnknownPanelNameEx : Exception
    {
        public UnknownPanelNameEx(string panelName) : base(panelName + " does not exist.") { }
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
            DisplayChoices();
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
    public void DisplayChoices()
    {
        // If there are no choices, show nothing and end the dialogue
        if (currDialogueNode == null || currDialogueNode.Choices.Length == 0)
        {
            StartCoroutine(EndDialogue());
            return;
        }

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
        int i;
        for (i = 0; i < numChoices; i++)
        {
            activePanel.ChoiceText[i].text = "";
            activePanel.ChoiceText[i].gameObject.SetActive(false);
        }
        numChoices = 0;
        activePanel.SelectionBox.gameObject.SetActive(false);
        activePanel.speakerNameUI.gameObject.SetActive(true);
        activePanel.sentenceUI.gameObject.SetActive(true);
        activePanel.arrowUI.gameObject.SetActive(true);

        /* End dialogue if there was no connected node, or if the choice 
         * specifies that the dialogue should end. */
        if (endAfter || currDialogueNode == null)
        {
            StartCoroutine(EndDialogue());
        }
        else
        {
            StartDialogue(currDialogueNode, activePanel.panelName,
                selectChoiceKey, prevChoiceKey, nextChoiceKey, false);
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

        OnEndDialogueEvent.Invoke(currDialogueNode);
        currDialogueNode = null;
    }
}
