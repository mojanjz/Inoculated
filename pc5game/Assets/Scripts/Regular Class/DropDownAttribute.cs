using UnityEngine;

public class DropDownAttribute : PropertyAttribute
{
    public string[] options;
    public int optionsLength;

    public DropDownAttribute(string[] options)
    {
        this.options = options;
        optionsLength = options.Length;
    }
}