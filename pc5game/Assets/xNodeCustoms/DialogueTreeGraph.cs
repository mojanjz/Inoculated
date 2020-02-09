using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class DialogueTreeGraph : NodeGraph 
{
	// The node to enter the tree with.
	public DialogueNode EntryNode = null;
}