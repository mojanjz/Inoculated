using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string Name;
    public string SpeakerName;

    [TextArea(3, 5)]
    public string[] sentences;
}
