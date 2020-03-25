using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeEvents : MonoBehaviour
{
    public void AskToLeave(BoolEventNode.NullableBoolRef boolRef)
    {
        boolRef.value = false;
    }
}
