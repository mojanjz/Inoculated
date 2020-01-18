using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue[] Dialogue;
    public bool IsActive { get; private set; } = false;
    public class BoolEvent : UnityEvent<bool> { }
    public BoolEvent OnDialogueEndEvent; // Passes true on success, false otherwise

    private void Awake()
    {
        if (OnDialogueEndEvent == null)
        {
            OnDialogueEndEvent = new BoolEvent();
        }
    }

    /* Method that starts a Dialogue in the dialogue panel.
     * PARAM: onEndCall, optional action to call when the dialogue completes */
    public void Trigger(Dialogue dialogue)
    {
        IsActive = true;

        try
        {
            StartCoroutine(DialogueManager.Instance.StartDialogue(dialogue));

            /* Continue if no errors */

            /* Create anonymous delegate that unsubscribes itself from the event afterwards */
            UnityAction handler = null;
            handler = () =>
            {
                OnDialogueEnd(true);
                DialogueManager.Instance.OnEndDialogueEvent.RemoveListener(handler);
            };

            DialogueManager.Instance.OnEndDialogueEvent.AddListener(handler);
        } catch (DialogueManager.NoInterruptException ex)
        {
            /* If the dialogue wasn't successfully started, display the exception
             * and close the process. */
            Debug.Log(ex);
            OnDialogueEnd(false);
        }
    }

    /* Method to be called when the triggered dialogue finishes. 
     * PARAM: result, true if the dialogue was started and finished, false otherwise */
    public void OnDialogueEnd(bool result)
    {
        IsActive = false;
        OnDialogueEndEvent.Invoke(result);
    }
}
