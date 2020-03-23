// ----------------------------------------------------------------------------
// Class to make it easier to choose between typing a dialogue directly in the
// inspector, versus using DialogueTreeGraph asset.
//
// Is displayed in the inspector using a custom property drawer 
// (DialogueRefDrawer).
// ----------------------------------------------------------------------------

using System;
using XNode;

[Serializable]
public class DialogueRef
{
    public bool UseDirect = true;

    // Just a regular dialogue object. Can be typed in the inspector.
    public Dialogue DirectValue;

    // From a tree asset with potentially various kinds of nodes and connections
    public Node NodeAsset;
}