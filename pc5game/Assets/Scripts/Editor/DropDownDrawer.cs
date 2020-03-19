// ----------------------------------------------------------------------------
// Script inspired by and modified from Ryan Hipple's sample project from 
// "Unite 2017 - Game Architecture with Scriptable Objects"
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using System;

[CustomPropertyDrawer(typeof(DropDownAttribute))]
public class DropDownDrawer : PropertyDrawer
{ 
    /// Cached style to use to draw the popup button.
    private GUIStyle popupStyle;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // First get the attribute since it contains the range for the slider
        DropDownAttribute dropDown = attribute as DropDownAttribute;

        float leftSide = position.x; // Stores the leftmost value for later use

        if (popupStyle == null)
        {
            popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
            popupStyle.imagePosition = ImagePosition.ImageOnly;
        }

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();

        //// Calculate rect for configuration button
        Rect buttonRect = new Rect(position);
        buttonRect.yMin += popupStyle.margin.top;
        buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
        position.xMin = buttonRect.xMax;

        // Store old indent level and set it to 0, the PrefixLabel takes care of it
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        int result = EditorGUI.Popup(buttonRect, System.Array.IndexOf(dropDown.options, property.stringValue), dropDown.options, popupStyle);

        // Stores user's popup selection
        if (result >= 0 && result < dropDown.optionsLength)
        {
            property.stringValue = dropDown.options[result];
        }

        GUIContent popUpLabel = new GUIContent();
        popUpLabel.text = property.stringValue;
        Rect nameRect = new Rect(buttonRect);
        nameRect.xMin = nameRect.xMax;
        nameRect.width = position.xMax - nameRect.xMin;
        nameRect = EditorGUI.PrefixLabel(nameRect, popUpLabel);

        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties();

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}