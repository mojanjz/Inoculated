using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using XNode;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueRef DialogueRef;

    public class BoolEvent : UnityEvent<bool> { }
    public BoolEvent OnDialogueEndEvent; // Passes true on success, false otherwise.

    private void Awake()
    {
        if (OnDialogueEndEvent == null)
        {
            OnDialogueEndEvent = new BoolEvent();
        }
    }

    /* Method that starts a Dialogue in the dialogue panel. */
    public void Trigger(string panelName, KeyCode selectKey, KeyCode prevKey, KeyCode nextKey)
    {

        UnityAction<DialogueNode> handler = null;
        handler = (DialogueNode entryNode) =>
        {
            OnDialogueEnd(true, entryNode);
            DialogueManager.Instance.OnEndDialogueEvent.RemoveListener(handler);
        };

        DialogueManager.Instance.OnEndDialogueEvent.AddListener(handler);

        try
        {
            if (DialogueRef.UseDirect)
            {
                DialogueManager.Instance.StartDialogue(DialogueRef.DirectValue, panelName, selectKey, prevKey, nextKey);
            }
            else
            {
                DialogueManager.Instance.StartDialogue(DialogueRef.NodeAsset, panelName, selectKey, prevKey, nextKey);
            }
        }
        catch (DialogueManager.NoInterruptEx Ex)
        {
            DialogueManager.Instance.OnEndDialogueEvent.RemoveListener(handler);
            OnDialogueEnd(false, null);
        }
        
        
        //try
        //{
        //    DialogueManager.Instance.StartDialogue(dialogue, engager);

        //    /* Continue if no errors */

        //    /* Create anonymous delegate that unsubscribes itself from the event afterwards */
        //    UnityAction handler = null;
        //    handler = () =>
        //    {
        //        OnDialogueEnd(true);
        //        DialogueManager.Instance.OnEndDialogueEvent.RemoveListener(handler);
        //    };

        //    DialogueManager.Instance.OnEndDialogueEvent.AddListener(handler);
        //}
        //catch (DialogueManager.NoInterruptEx ex)
        //{
        //    /* If the dialogue wasn't successfully started, display the exception
        //     * and close the process. */
        //    Debug.Log(ex);
        //    OnDialogueEnd(false);
        //}
    }

    // Method to be called when the triggered dialogue finishes. 
    public void OnDialogueEnd(bool wasDisplayed, DialogueNode entryNode)
    {
        if (entryNode != null)
        {
            DialogueRef.NodeAsset = entryNode;
        }

        OnDialogueEndEvent.Invoke(wasDisplayed);
    }
}
