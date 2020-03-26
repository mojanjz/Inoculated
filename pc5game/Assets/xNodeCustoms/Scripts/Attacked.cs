using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class Attacked : Node
{
	[Input(ShowBackingValue.Never)] public Anything Input;
	[Output(connectionType: ConnectionType.Override)] public Anything Output;

	public int Damage;

	//This class is defined for the sole purpose of being serializable
	[System.Serializable] public class Anything { }

	// Return the correct value of a port when requested
	public override object GetValue(NodePort port)
	{
		// Don't really care, just need this node type for the manager to handle
		if (port.fieldName == "Output")
		{
			return this;
		}

		else if (port.fieldName == "Input")
		{
			return GetInputValue<Anything>("Input");
		}

		return null;
	}
}