using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueTree", menuName = "ScriptableObjects/DialogueTree", order = 35)]
public class DialogueTree : ScriptableObject
{
    public string CurrNodeID;
    public DialogueNode[] Nodes;

    [System.Serializable]
    public class DialogueNode
    {
        public string ID;
        public Dialogue Dialogue;
        public ChoiceSet[] Choices;
    }

    [System.Serializable]
    public class ChoiceSet
    {
        public string ChoiceText;
        public string DestNodeID;
    }
}
