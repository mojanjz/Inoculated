using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class DialogueTreeGraph : NodeGraph
{
    [SerializeField] private DialogueNode startNode; // Default starting node for the tree.

    public DialogueNode StartNode
    {
        get { return startNode; }
    }
}