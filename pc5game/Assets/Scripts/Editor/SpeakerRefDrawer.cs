using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpeakerRef))]
public class SpeakerRefDrawer : PropertyDrawer
{
    private bool typeCustom;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();

        // Get properties
        SerializedProperty chooseSpeaker = property.FindPropertyRelative("SpeakerChoice");
        SerializedProperty custom = property.FindPropertyRelative("Custom");

        // Store old indent level and set it to 0, the PrefixLabel takes care of it
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUI.PropertyField(position, chooseSpeaker, GUIContent.none, true);

        if (chooseSpeaker.enumNames[chooseSpeaker.enumValueIndex] == "Custom")
        {
            typeCustom = true;
            position.yMin += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, custom, GUIContent.none, true);
        } else
        {
            typeCustom = false;
        }

        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties();

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }


    /* Set the height for this property drawer. */
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = base.GetPropertyHeight(property, label);
       
        if (typeCustom)
        {
            // Adds height for the custom type name field
            SerializedProperty displayedProperty = property.FindPropertyRelative("Custom");
            height += EditorGUI.GetPropertyHeight(displayedProperty, label, true);
        }
            
        return height;
    }
}