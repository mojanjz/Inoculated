using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueNode : Node {

	public string NodeName;
 	public Dialogue Dialogue;
	[Input(ShowBackingValue.Never)] public DialogueNode Input;
	[Output(connectionType: ConnectionType.Override, dynamicPortList: true)] public ChoiceSet[] Choices; // Note sure why connection doesn't override

	[System.Serializable]
	public class ChoiceSet
	{
		[TextArea(1, 3)] public string ChoiceText;
		public bool EndAfter; // Whether or not the dialogue ends when this choice is chosen
	}

	//// Use this for initialization
	//protected override void Init() {
	//	base.Init();
	//}

	// Return the correct value of a port when requested
	public override object GetValue(NodePort port) {

		// Compare only the variable name portion of the fieldName (ignore the index)
		if (port.fieldName.Substring(0, 7) == "Choices")
		{
			// Different choice ports may be connected to different nodes,
			// but all choice ports output this node value.
			return this;
		}

		else if (port.fieldName == "Input") 
		{
			return GetInputValue<DialogueNode>("Input");
		}

		return null;
	}

	/* Called when a value in the node inspector changes. */
	public void OnValidate()
	{
		/* Change the node name to be the same as the NodeName field. */
		Object target = this;
		target.name = NodeName; // Renames the node GUI window, and the node asset.

		/* THIS BREAKS UNITY. (After closing and reopening Unity, it'll get stuck on loading packages.) :( */
		/* And it somehow renames the parent asset, as well as the actual asset. */
		// UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath(target), NodeID); // Renames the node asset in the Assets folder
	}
}