using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class SwitchScene : Node {

	//public string NodeName;
	[Input(ShowBackingValue.Never)] public Anything Input;
	//[Output(ShowBackingValue.Never, ConnectionType.Override)] public Anything Output;

	public bool AgreeToLeave;

	//public string SceneName;

	/// <summary> This class is defined for the sole purpose of being serializable </summary>
	[System.Serializable] public class Anything { }

	// Return the correct value of a port when requested
	public override object GetValue(NodePort port) {

		//meh
		if (port.fieldName == "Input")
		{
			return GetInputValue<object>("Input");
		}

		return null;
	}

	///* Called when a value in the node inspector changes. */
	//public void OnValidate()
	//{
	//	/* Change the node name to be the same as the NodeID field. */
	//	Object target = this;
	//	target.name = NodeName; // Renames the node GUI window, and the node asset.

	//	/* THIS BREAKS UNITY. (After closing and reopening Unity, it'll get stuck on loading packages.) :( */
	//	/* And it somehow renames the parent asset, as well as the actual asset. */
	//	// UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath(target), NodeID); // Renames the node asset in the Assets folder
	//}
}