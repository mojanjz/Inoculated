// ----------------------------------------------------------------------------
// Class to make it easier to choose between typing a dialogue directly in the
// inspector, versus using a DialogueTreeGraph asset.
//
// Is displayed in the inspector using a custom property drawer 
// (DialogueRefDrawer).
// ----------------------------------------------------------------------------

using System;

[Serializable]
public class DialogueRef
{
    public bool UseDirect = true;

    // Just a regular dialogue object. Can be typed in the inspector.
    public Dialogue DirectValue;

    // Saved asset with multiple dialogues and connections
    public DialogueTreeGraph TreeAsset; 
}