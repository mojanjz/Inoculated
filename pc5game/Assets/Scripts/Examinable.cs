using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Examinable : MonoBehaviour
{
    [SerializeField] private DialogueTrigger dialogueTrigger;
    private int dialogueIndex = 0;

    [Serializable] public class ExaminerEvent : UnityEvent<Examiner> { }

    public ExaminerEvent OnExamineStartEvent;
    public ExaminerEvent OnExamineEndEvent;

    private Examiner examiner = null; // Store a reference to the current examiner

    private void Awake()
    {
        if (OnExamineStartEvent == null)
            OnExamineStartEvent = new ExaminerEvent();

        if (OnExamineEndEvent == null)
            OnExamineEndEvent = new ExaminerEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* Method that gets called when the GameObject of this component is examined 
     * (by a GameObject with the Examiner component).
     * PARAM: ex, the examiner conducting the examination */
    public void OnExamine(Examiner ex)
    {
        /* In the inspector, choose some event handler(s) to evoke for OnExamineStartEvent. */
        /* Don't forget to add the related finishing handler to OnExamineEndEvent too. */

        if (OnExamineStartEvent.GetPersistentEventCount() == 0)
        {
            /*If no actions were selected, throw an exception.*/
            throw new NoOnExamineActionEx(gameObject.name);
        }

        OnExamineStartEvent.Invoke(ex);
    }

    public class NoOnExamineActionEx : Exception
    {
        public NoOnExamineActionEx(string name) : 
            base(String.Format(name + " has no OnExamine actions set.")) { }
    }

    /* Method to cycle through a collection of dialogues. Each examination
     * process calls one dialogue, then increments the index by 1. */
    public void CycleDialogue(Examiner ex)
    {
        /* If this object is already being examined, do nothing. */
        if (examiner != null) {
            OnExamineEndEvent.Invoke(ex);
            return;
        }

        examiner = ex;
        dialogueTrigger.OnDialogueEndEvent.AddListener(OnCycleDialogueEnd);
        dialogueTrigger.Trigger(dialogueTrigger.Dialogue[dialogueIndex], ex.gameObject);
    }

    /* Method that should be called to wrap up the cycle dialogue action. 
     * PARAM: result, true if the dialogue was displayed successfully, false otherwise */
    public void OnCycleDialogueEnd(bool wasDisplayed)
    {
        dialogueTrigger.OnDialogueEndEvent.RemoveListener(OnCycleDialogueEnd);

        /* If the dialogue was displayed, move the dialogue index to the next 
         * dialogue in the sequence. */
        if (wasDisplayed)
        {
            dialogueIndex++;

            /* If the end of the dialogue collection was reached, restart. */
            if (dialogueIndex == dialogueTrigger.Dialogue.Length)
            {
                dialogueIndex = 0;
            }
        }

        OnExamineEndEvent.Invoke(examiner);
        examiner = null;
    }
}
