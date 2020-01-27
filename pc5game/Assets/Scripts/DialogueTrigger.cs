using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue[] Dialogue;

    public class BoolEvent : UnityEvent<bool> { }
    public BoolEvent OnDialogueEndEvent; // Passes true on success, false otherwise.

    private void Awake()
    {
        if (OnDialogueEndEvent == null)
        {
            OnDialogueEndEvent = new BoolEvent();
        }
    }

    /* Method that starts a Dialogue in the dialogue panel.
     * PARAM: dialogue, the dialogue to start
     * PARAM: engager, the player game object that started the dialogue */
    public void Trigger(Dialogue dialogue, GameObject engager)
    {
        try
        {
            DialogueManager.Instance.StartDialogue(dialogue, engager);

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
     * PARAM: result, true if the dialogue was shown fully, false otherwise */
    public void OnDialogueEnd(bool result)
    {
        OnDialogueEndEvent.Invoke(result);
    }
}
