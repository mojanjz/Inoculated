using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System;

public class TestDialogueChoice : MonoBehaviour
{
    public Dialogue Dia;
    public DialogueTreeGraph DialogueTree;

    public DialogueRef DiaRef;
   

    NodePort port = null;

    // Start is called before the first frame update
    void Start()
    {
        if (DialogueTree.EntryNode == null)
        {
            throw new NoEntryNodeEx(DialogueTree);
        }

        Debug.Log(DialogueTree.EntryNode.Dialogue.Sentences[0]);
        Debug.Log(DialogueTree.EntryNode.Choices[0]);
        Debug.Log(DialogueTree.EntryNode.Choices[1]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            port = DialogueTree.EntryNode.GetOutputPort("Choices 0").Connection;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            port = DialogueTree.EntryNode.GetOutputPort("Choices 1").Connection;
        }

        if (port != null)
        {
            DialogueTree.EntryNode = port.node as DialogueNode;
            Debug.Log(DialogueTree.EntryNode.Dialogue.Sentences[0]);
            Debug.Log(DialogueTree.EntryNode.Choices[0]);
            Debug.Log(DialogueTree.EntryNode.Choices[1]);
            port = null;
        }
    }
    public class NoEntryNodeEx : Exception
    {
        public NoEntryNodeEx(DialogueTreeGraph tree) : base(tree.name + " has no entry node specified.") { }
    }
}
