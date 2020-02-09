using System.Linq;
using UnityEditor;
using XNodeEditor;
using XNode;

/// <summary> 
/// NodeEditor functions similarly to the Editor class, only it is xNode specific.
/// Custom node editors should have the CustomNodeEditor attribute that defines which node type it is an editor for.
/// </summary>
[CustomNodeEditor(typeof(DialogueNode))]
public class DialogueNodeEditor : NodeEditor
{
    /// <summary> Called whenever the xNode editor window is updated </summary>
    public override void OnBodyGUI()
    {
        /* Only the DisplayInputValue() portion is custom. Everything else below follows the default GUI. */





        //// Draw the default GUI first, so we don't have to do all of that manually.
        //base.OnBodyGUI();

        // Unity specifically requires this to save/update any serial object.
        // serializedObject.Update(); must go at the start of an inspector gui, and
        // serializedObject.ApplyModifiedProperties(); goes at the end.
        serializedObject.Update();
        string[] excludes = { "m_Script", "graph", "position", "ports" };

        // Iterate through serialized properties and draw them like the Inspector (But with ports)
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        EditorGUIUtility.labelWidth = 84;
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (excludes.Contains(iterator.name)) continue;
            NodeEditorGUILayout.PropertyField(iterator, true);

            // Display the value of the input port
            if (iterator.name == "EntryPort")
            {
                DisplayInputValue("EntryPort");
            }
        }

        // Iterate through dynamic ports and draw them in the order in which they are serialized
        foreach (XNode.NodePort dynamicPort in target.DynamicPorts)
        {
            if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
            NodeEditorGUILayout.PortField(dynamicPort);
        }

        serializedObject.ApplyModifiedProperties();
    }


    // Display the value of the input port of the given fieldName
    private void DisplayInputValue(string fieldName)
    {
        // `target` points to the node, but it is of type `Node`, so cast it.
        DialogueNode dialogueNode = target as DialogueNode;

        // Get the value from the node, and display it
        NodePort port = dialogueNode.GetInputPort(fieldName);
        object obj = dialogueNode.GetValue(port);
        if (obj != null) EditorGUILayout.LabelField(obj.ToString());
    }
}
