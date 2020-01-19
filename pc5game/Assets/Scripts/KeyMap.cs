using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMap : MonoBehaviour
/* Class for defining which keyboard keys do what action in the game. */
{
    public KeyCode Unlock = KeyCode.E;
    public KeyCode Examine = KeyCode.F;
    public KeyCode NextSentence = KeyCode.F;
    public KeyCode Attack = KeyCode.Space;
    public KeyCode AttackSwitch = KeyCode.LeftShift;

    public KeyCode MoveUp = KeyCode.W;
    public KeyCode MoveLeft = KeyCode.A;
    public KeyCode MoveDown = KeyCode.S;
    public KeyCode MoveRight = KeyCode.D;
}
