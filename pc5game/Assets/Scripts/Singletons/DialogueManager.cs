using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using XNode;
using UnityEngine.SceneManagement;

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
    [SerializeField] private int highlightAnimLayer = 2;

    // Input args
    // NEED TO RETHINK THIS
    public class Args
    {
        public Args() { }

        // Dialogue node
        public KeyCode SelectKey = KeyCode.None;
        public KeyCode PrevKey = KeyCode.None;
        public KeyCode NextKey = KeyCode.None;
        public CharacterStats Player = null;
        public CharacterStats Interactable = null;
        public bool NewInteraction = true;

        // Unlock node
        public Unlockable Unlockable = null;

        // Attacked node
        public Attackable PlayerAttackable = null;
    }

    // Transient data
    private Args savedArgs = new Args();
    //private KeyCode savedArgs.SelectKey = KeyCode.None;
    //private KeyCode savedArgs.PrevKey = KeyCode.None;
    //private KeyCode savedArgs.NextKey = KeyCode.None;
    //private CharacterStats savedArgs.Player = null;
    //private CharacterStats savedArgs.Interactable = null;

    // Dialogue
    private PanelSet activePanel = null; // Only one panel should be active at a time
    private int choiceIndex = 0;
    private int numChoices = 0;
    private Node currDialogueNode = null;
    private Queue<string> sentenceQ = new Queue<string>();
    private Coroutine typingCoroutine = null;
    private string currSentence = null;

    // SwitchScene
    public bool agreeToLeave = false;
    public string nextScene = "";
    [SerializeField] private DialogueNode askToLeave;

    public class NodeEvent : UnityEvent<Node> { }
    public NodeEvent OnEndDialogueEvent;

    // (Optional) Prevent non-singleton constructor use.
    protected DialogueManager() { }

    protected override void Awake()
    {
        base.Awake();

        enabled = false;

        if (OnEndDialogueEvent == null)
        {
            OnEndDialogueEvent = new NodeEvent();
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
        if (Input.GetKeyDown(savedArgs.SelectKey))
        {
            // UpdateSentence();
            StartCoroutine(SelectChoice());
        }

        else if (Input.GetKeyDown(savedArgs.PrevKey) && (numChoices != 0))
        {
            PrevChoice();
        }

        else if (Input.GetKeyDown(savedArgs.NextKey) && (numChoices != 0))
        {
            NextChoice();
        }
    }

    public void RunNode(Node node, Args args)
    {
        // Don't interrupt a process that is running.
        if ( (activePanel != null) && (args.NewInteraction == true))  // Maybe check currDialogueNode != null since some nodes don't have activePanel
        {
            throw new NoInterruptEx("Could not start new dialogue. " +
                "Dialogue panel is already in active process.");
        }

        savedArgs = args;

        switch (node)
        {
            case DialogueNode cast:
                StartDialogue(cast, args.SelectKey, args.PrevKey, args.NextKey,
                args.Player, args.Interactable, args.NewInteraction);
                break;
            case SwitchScene cast:
                StartCoroutine(AskToLeave(cast));
                break;
            case Attacked cast:
                savedArgs.PlayerAttackable.OnAttack(cast.Damage);
                GetNextNode();
                break;
            case UnlockNode cast:
                args.Unlockable.OnUnlock(null, overrideKey:true);
                GetNextNode();
                break;
            case EndNode cast:
                NodePort port = currDialogueNode.GetOutputPort("Output").Connection;
                if (port != null)
                {
                    // Set the connected node as the new active node
                    currDialogueNode = port.node;
                }
                StartCoroutine(EndDialogue());
                break;
        }
    }

    // Need to combine with SelectChoice()
    private void GetNextNode()
    {
        NodePort port = currDialogueNode.GetOutputPort("Output").Connection;
        if (port != null)
        {
            // Set the connected node as the new active node
            currDialogueNode = port.node;

            savedArgs.NewInteraction = false;
            RunNode(currDialogueNode, savedArgs);
        }
        else
        {
            // If no nodes are connected
            currDialogueNode = null;
            StartCoroutine(EndDialogue());
        }
    }

    // NEED TO ORGANIZE BETTER LATER!
    private IEnumerator AskToLeave(SwitchScene currNode)
    {
        //if (currNode.SceneName != "" && currNode.SceneName != null)
        //{
        //    nextScene = currNode.SceneName;
        //}

        NextSceneName component = savedArgs.Interactable?.GetComponent<NextSceneName>();

        if (component != null)
        {
            nextScene = component.SceneName;
        }

        // If other player already agreed, go to next scene.
        if (agreeToLeave && currNode.AgreeToLeave)
        {
            Debug.Log("switching scene...");
            agreeToLeave = false;
            askToLeave = ((DialogueTreeGraph)askToLeave.graph).StartNode;
            SceneManager.LoadScene(nextScene);
            yield break;
        }

        agreeToLeave = currNode.AgreeToLeave;

        // Didn't agree
        if (!currNode.AgreeToLeave)
        {
            askToLeave = ((DialogueTreeGraph)askToLeave.graph).StartNode;
            StartCoroutine(EndDialogue());
            yield break;
        }

        // Ask other player

        // Reset for first player
        currDialogueNode = ((DialogueTreeGraph)currNode.graph).StartNode;
        yield return StartCoroutine(EndDialogue());

        PlayerInteract otherPlayer;
        KeyMap otherKey;

        // Check with other player
        if (savedArgs.Player == brotherStats)
        {
            // Check with sister
            otherPlayer = sisterStats.gameObject.GetComponent<PlayerInteract>();
            otherKey = sisterStats.gameObject.GetComponent<KeyMap>();
        }
        else
        {
            // Check with brother
            otherPlayer = brotherStats.gameObject.GetComponent<PlayerInteract>();
            otherKey = brotherStats.gameObject.GetComponent<KeyMap>();
        }

        otherPlayer.OnInteractStart();

        UnityAction<Node> handler = null;
        handler = (Node entryNode) =>
        {
            otherPlayer.OnInteractEnd();
            DialogueManager.Instance.OnEndDialogueEvent.RemoveListener(handler);
        };

        DialogueManager.Instance.OnEndDialogueEvent.AddListener(handler);
        StartDialogue(askToLeave, otherKey.SelectKey, otherKey.PrevKey, otherKey.NextKey,
                sisterStats);
    }

    /* Method for starting a dialogue.
     * PARAM: dialogue, the dialogue to start
     * PARAM: selectKey, the key to use to update the dialogue (ie. show next sentence)
     * EXCEPTION: If another dialogue is already in process, NoInterruptEx will be thrown.
     * EXCEPTION: If the panel is not valid, UnknownPanelEx will be thrown.*/
    public void StartDialogue(Dialogue dialogue, KeyCode selectKey, KeyCode prevKey, KeyCode nextKey,
        CharacterStats player = null, CharacterStats interactable = null)
    {
        activePanel = GetPanelSet(dialogue.Panel);
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
            activePanel = GetPanelSet(dialogue.Panel);
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

    private PanelSet GetPanelSet(Dialogue.PanelID panelID)
    {
        PanelSet panelSet;
        bool getOther = false;

        //if (panelID == Dialogue.PanelID.ThisPlayer)
        //{
        //    panelID = savedArgs.Player.PanelID;
        //}

        switch (panelID)
        {
            case Dialogue.PanelID.ThisPlayer:
                panelID = savedArgs.Player.PanelID;
                break;
            case Dialogue.PanelID.OtherPlayer:
                panelID = savedArgs.Player.PanelID;
                getOther = true;
                break;
        }

        switch (panelID)
        {
            case Dialogue.PanelID.Sister:
                panelSet = sister;

                if (getOther)
                {
                    panelSet = brother;
                }
                break;
            case Dialogue.PanelID.Brother:
                panelSet = brother;

                if (getOther)
                {
                    panelSet = sister;
                }
                break;
            default:
                StartCoroutine(EndDialogue());
                throw new UnknownPanelEx(panelID);
        }

        return panelSet;
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
                if (savedArgs.Interactable == null)
                {
                    throw new UnknownSpeakerNameEx(SpeakerRef.Speaker.ThisObject);
                }
                name = savedArgs.Interactable.SpeakerName;
                break;

            case SpeakerRef.Speaker.ThisPlayer:
                if (savedArgs.Player == null)
                {
                    throw new UnknownSpeakerNameEx(SpeakerRef.Speaker.ThisPlayer);
                }
                name = savedArgs.Player.SpeakerName;
                break;
            case SpeakerRef.Speaker.OtherPlayer:
                if (savedArgs.Player == null)
                {
                    throw new UnknownSpeakerNameEx(SpeakerRef.Speaker.ThisPlayer);
                }

                if (savedArgs.Player == sisterStats)
                {
                    name = brotherStats.SpeakerName;
                } else
                {
                    name = sisterStats.SpeakerName;
                }
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

        // Start calling Update() each frame.
        enabled = true;

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
        DialogueNode cast = (DialogueNode)currDialogueNode;

        // If there are no more sentences and no valid choices, don't display arrows
        if ( sentenceQ.Count == 0)
        {
            // Need to reconsider NoChoiceText idea...
            if ( cast.Choices.Length == 0 || cast.Choices[0]?.ChoiceText == cast.NoChoiceText)
            {
                activePanel.arrowUI.text = "";
            }
        }
            
        else
        {
            activePanel.arrowUI.text = ">>";
        }
    }

    // Display dialogue choices
    public void DisplayChoiceUI()
    {
        DialogueNode cast = (DialogueNode)currDialogueNode;

        // If there are no choices, show nothing and end the dialogue
        if ( cast.Choices.Length == 0 )
        {
            StartCoroutine(EndDialogue());
            return;
        }

        // If there is only one choice, and it's the fake choice (use to connect to next node),
        // just select that choice
        else if (cast.Choices[0].ChoiceText == cast.NoChoiceText)
        {
            ClearSpeechText();
            numChoices = 1;
            StartCoroutine(SelectChoice());
        }

        else
        {
            ClearSpeechText();
            choiceIndex = 0; // Start at the first choice
            numChoices = 0;
            foreach (DialogueNode.ChoiceSet choice in cast.Choices)
            {
                activePanel.ChoiceText[numChoices].text = choice.ChoiceText;
                activePanel.ChoiceText[numChoices].gameObject.SetActive(true);

                numChoices++;
            }

            activePanel.SelectionBox.gameObject.SetActive(true);
            activePanel.SelectionBox.anchoredPosition = activePanel.ChoiceTextPos[choiceIndex];

            DisplaySpeechUI(false);
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

    public IEnumerator SelectChoice()
    {
        // If there are no choices, "select" simply moves to the next sentence
        if (numChoices == 0)
        {
            UpdateSentence();
            yield break;
        }

        DialogueNode cast = (DialogueNode)currDialogueNode;

        if (cast.Choices[0].ChoiceText != cast.NoChoiceText) // If not fake choice
        {
            enabled = false;

            // Animate selection highlighter

            activePanel.panelAnimator.SetTrigger("OnSelect");

            // Wait until it's the highlighted state
            AnimatorStateInfo clipInfo = activePanel.panelAnimator.GetCurrentAnimatorStateInfo(highlightAnimLayer);
            while (!clipInfo.IsName("Selection.OnSelect")){
                clipInfo = activePanel.panelAnimator.GetCurrentAnimatorStateInfo(highlightAnimLayer);
                yield return null;
            }

            // Wait until highlight animation finishes
            float myTime = activePanel.panelAnimator.GetCurrentAnimatorStateInfo(highlightAnimLayer).length;
            yield return new WaitForSeconds(myTime);

            enabled = true;
        }

        bool endAfter = cast.Choices[choiceIndex].EndAfter;

        // Get the node port connected to the selected choice
        NodePort port = cast.GetOutputPort("Choices " + choiceIndex).Connection;
        if (port != null)
        {
            // Set the connected node as the new active node
            //currDialogueNode = port.node as DialogueNode;
            currDialogueNode = port.node;
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
            //StartDialogue(((DialogueNode)currDialogueNode),
            //    savedArgs.SelectKey, savedArgs.PrevKey, savedArgs.NextKey, newInteraction: false);

            savedArgs.NewInteraction = false;

            RunNode(currDialogueNode, savedArgs);
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


        // Setting the parameter doesn't immediately activate the transition, so we have to wait.
        float seconds;
        while ((seconds = activePanel.panelAnimator.GetAnimatorTransitionInfo(panelAnimLayer).duration) == 0)
        {
            yield return null; // Wait a frame, for each frame that the transition isn't active
        }

        // Wait for the dialogue panel to open fully before continuing.
        yield return new WaitForSeconds(seconds);


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
        savedArgs.Player = null;
        savedArgs.Interactable = null;
        activePanel = null;
        savedArgs.SelectKey = KeyCode.None;
        savedArgs.PrevKey = KeyCode.None;
        savedArgs.NextKey = KeyCode.None;
        sentenceQ.Clear();
        typingCoroutine = null;
        currSentence = null;
    }
}
