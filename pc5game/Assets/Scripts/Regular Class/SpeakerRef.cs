// Is displayed in the inspector using a custom property drawer (SpeakerRefDrawer).

using System;

[Serializable]
public class SpeakerRef
{
    // Reference a specific speaker.
    public Speaker SpeakerChoice;

    // If the speaker is "Custom", type speaker name directly into the inspector.
    public string Custom;

    public enum Speaker
    {
        Custom = 0, // If selected, allows user to type in a custom name
        ThisObject = 1,
        ThisPlayer = 2,
        OtherPlayer = 3,
        Brother = 4,
        Sister = 5
    }
}