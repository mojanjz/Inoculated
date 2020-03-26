// ----------------------------------------------------------------------------
// Script inspired by and modified from Ryan Hipple's sample project from 
// "Unite 2017 - Game Architecture with Scriptable Objects"
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueRef))]
public class DialogueRefDrawer : PropertyDrawer
{
    ///// <summary>
    ///// Options to display in the popup to type dialogue directly in the inspector, or use a dialogue tree asset.
    ///// </summary>
    //private readonly string[] popupOptions =
    //    { "Type Dialogue", "Use Dialogue Tree Asset" };

    ///// <summary> Cached style to use to draw the popup button. </summary>
    //private GUIStyle popupStyle;

    private bool useDirectDialogue = false;
    private string toggleLabel = "Type single dialogue?";
    private GUIContent directValueLabel = new GUIContent("Dialogue");
    private GUIContent treeAssetLabel = new GUIContent("Node");

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float leftSide = position.x; // Stores the leftmost value for later use

        //if (popupStyle == null)
        //{
        //    popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
        //    popupStyle.imagePosition = ImagePosition.ImageOnly;
        //}

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();

        // Get properties
        SerializedProperty useDirect = property.FindPropertyRelative("UseDirect");
        SerializedProperty directValue = property.FindPropertyRelative("DirectValue");
        SerializedProperty nodeAsset = property.FindPropertyRelative("NodeAsset");

        //// Calculate rect for configuration button
        //Rect buttonRect = new Rect(position);
        //buttonRect.yMin += popupStyle.margin.top;
        //buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
        //position.xMin = buttonRect.xMax;

        // Store old indent level and set it to 0, the PrefixLabel takes care of it
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        //int result = EditorGUI.Popup(buttonRect, useDirect.boolValue ? 0 : 1, popupOptions, popupStyle);

        //// Stores user's popup selection
        //useDirect.boolValue = result == 0;

        /* Create toggle GUI. */
        Rect toggleRect = new Rect(position);
        toggleRect.height = EditorGUIUtility.singleLineHeight;
        useDirect.boolValue = EditorGUI.ToggleLeft(toggleRect, toggleLabel, useDirect.boolValue);
        useDirectDialogue = useDirect.boolValue;

        /* Height for dialogueRect appears to not matter when 
         * EditorGUI.PropertyField() is called with includeChildren = true. */
        Rect dialogueRect = new Rect(leftSide, position.y + EditorGUIUtility.singleLineHeight,
            position.xMax - leftSide, EditorGUIUtility.singleLineHeight);

        /* Display the dialogue property indented under the main property label. */
        EditorGUI.indentLevel++;
        EditorGUI.PropertyField(dialogueRect,
            useDirect.boolValue ? directValue : nodeAsset,
            useDirect.boolValue ? directValueLabel : treeAssetLabel,
            true);

        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties();

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }


    /* Called before OnGUI(). Sets the height for this property drawer. */
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = base.GetPropertyHeight(property, label);

        /* Sets the height to match the serialized property that is currently 
         * being displayed. (The toggle state determines which property is
         * displayed.) */
        SerializedProperty displayedProperty;
        if (useDirectDialogue)
        {
             displayedProperty = property.FindPropertyRelative("DirectValue");
             height += EditorGUI.GetPropertyHeight(displayedProperty, label, true);
        } else
        {
            displayedProperty = property.FindPropertyRelative("NodeAsset");
            height += EditorGUI.GetPropertyHeight(displayedProperty, label, true);
        }

        return height;
    }
}