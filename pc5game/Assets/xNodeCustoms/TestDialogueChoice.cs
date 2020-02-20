using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System;

public class TestDialogueChoice : MonoBehaviour
{
    public DialogueTreeGraph DialogueTree;
    public DialogueTrigger trig;

    NodePort port = null;
    DialogueNode currNode = null;

    private void Start()
    {
        //trig.Trigger("BrotherPanel", KeyCode.F, KeyCode.A, KeyCode.D);
    }

    //// Start is called before the first frame update
    //void Start()
    //{
    //    if (DialogueTree.StartNode == null)
    //    {
    //        throw new NoStartNodeEx(DialogueTree);
    //    }

    //    Debug.Log(DialogueTree.StartNode.Dialogue.Sentences[0]);
    //    Debug.Log(DialogueTree.StartNode.Choices[0]);
    //    Debug.Log(DialogueTree.StartNode.Choices[1]);
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        port = DialogueTree.StartNode.GetOutputPort("Choices 0").Connection;
    //    }
    //    else if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        port = DialogueTree.StartNode.GetOutputPort("Choices 1").Connection;
    //    }

    //    if (port != null)
    //    {
    //        currNode = port.node as DialogueNode;
    //        Debug.Log(currNode.Dialogue.Sentences[0]);
    //        Debug.Log(currNode.Choices[0]);
    //        Debug.Log(currNode.Choices[1]);
    //        port = null;
    //    }
    //}
    public class NoStartNodeEx : Exception
    {
        public NoStartNodeEx(DialogueTreeGraph tree) : base(tree.name + " has no entry node specified.") { }
    }
}
