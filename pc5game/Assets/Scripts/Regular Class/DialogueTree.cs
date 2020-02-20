/* I just realized this ain't even necessary lol. */

[System.Serializable]
public class DialogueTree
{
    public DialogueTreeGraph Tree;
    public DialogueNode EntryNode; // This stores the node to enter the tree with.

    public DialogueTree() { }
    public DialogueTree(DialogueTreeGraph tree)
    {
        Tree = tree;

        if (tree.StartNode != null)
        {
            EntryNode = tree.StartNode;
        }
    }

    public DialogueTree(DialogueTreeGraph tree, DialogueNode entryNode)
    {
        Tree = tree;
        EntryNode = entryNode;
    }
}
