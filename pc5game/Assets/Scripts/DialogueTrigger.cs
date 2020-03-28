using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using XNode;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private CharacterStats thisStats; // Necessary if the dialogue uses this object's speaker name
    [SerializeField] private Unlockable thisUnlockable;
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

    private void Start()
    {
        thisStats = GetComponent<CharacterStats>();
    }

    /* Method that starts a Dialogue in the dialogue panel. */
    public void Trigger(KeyCode selectKey, KeyCode prevKey, KeyCode nextKey, CharacterStats playerStats = null, Attackable playerAttackable = null)
    {

        UnityAction<Node> handler = null;
        handler = (Node entryNode) =>
        {
            OnDialogueEnd(true, entryNode);
            DialogueManager.Instance.OnEndDialogueEvent.RemoveListener(handler);
        };

        DialogueManager.Instance.OnEndDialogueEvent.AddListener(handler);

        try
        {
            DialogueManager.Args args = new DialogueManager.Args
            {
                SelectKey = selectKey,
                PrevKey = prevKey,
                NextKey = nextKey,
                Player = playerStats,
                Interactable = thisStats,
                NewInteraction = true,
                Unlockable = thisUnlockable,
                PlayerAttackable = playerAttackable
            };

            if (DialogueRef.UseDirect)
            {
                DialogueManager.Instance.StartDialogue(DialogueRef.DirectValue, args, selectKey, prevKey, nextKey, player:playerStats, interactable:thisStats);
            }
            else
            {
                //DialogueManager.Instance.StartDialogue((DialogueNode)DialogueRef.NodeAsset, selectKey, prevKey, nextKey, player:player, interactable:ThisObj);
                DialogueManager.Instance.RunNode(DialogueRef.NodeAsset, args);
            }
        }
        catch (DialogueManager.NoInterruptEx Ex)
        {
            DialogueManager.Instance.OnEndDialogueEvent.RemoveListener(handler);
            OnDialogueEnd(false, null);
        }
    }

    // Method to be called when the triggered dialogue finishes. 
    public void OnDialogueEnd(bool wasDisplayed, Node entryNode)
    {
        if (entryNode != null)
        {
            DialogueRef.NodeAsset = entryNode;
        }

        OnDialogueEndEvent.Invoke(wasDisplayed);
    }
}
