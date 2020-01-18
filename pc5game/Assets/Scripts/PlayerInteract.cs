using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    /* Relevant interaction behaviour controllers. */
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Unlocker unlocker;
    [SerializeField] private Attacker attacker;
    [SerializeField] private Examiner examiner;

    /* Stores the interaction key that the user presses. */
    private KeyCode keyPressed;
    
    /* Start is called before the first frame update */
    void Start()
    {
    }

    /* Update is called once per frame */
    void Update()
    {
        /* Get the key that the user presses, if any. */
        keyPressed = GetInput();

        /* If any valid key was pressed, interact with the object with the 
         * specified key. */
        if (keyPressed != KeyCode.None)
        {
            playerController.DisableMove();
            Interact(keyPressed);
        }
    }

    /* Method to initiate player interactions.
     * PARAM: keyPressed, the keyboard key that was pressed by the user. */
    private void Interact(KeyCode keyPressed)
    {
        if (keyPressed == KeyMap.Unlock)
        {
            Debug.Log("Trying to open...");
            unlocker.Unlock();
        }

        if (keyPressed == KeyMap.Examine)
        {
            Debug.Log("Looking...");
            examiner.Examine();
        }

        if (keyPressed == KeyMap.Attack)
        {
            Debug.Log("BAM BAM");
            StartCoroutine(attacker.Attack());
        }

        // TEMP CODE FOR DEMO PURPOSES
        if (Input.GetKeyDown(KeyMap.AttackSwitch))
        {
            Debug.Log("Switching weapon...");
            if (attacker.AtkIndex == 0)
            {
                attacker.SwitchAttack("Weapon");
            }
            else
            {
                attacker.SwitchAttack("Regular");
            }
        }
    }

    /* Method for getting user input. 
       RETURN: Key that was pressed, or KeyCode.None if no valid keys were pressed.*/
    private KeyCode GetInput()
    {
        if (Input.GetKeyDown(KeyMap.Examine))
        {
            Debug.Log("interacted with " + KeyMap.Examine);
            return KeyMap.Examine;
        }

        if (Input.GetKeyDown(KeyMap.Unlock))
        {
            Debug.Log("interacted with " + KeyMap.Unlock);
            return KeyMap.Unlock;
        }

        if (Input.GetKeyDown(KeyMap.Attack))
        {
            Debug.Log("interacted with " + KeyMap.Attack);
            return KeyMap.Attack;
        }

        /* Return none if no valid keys pressed. */
        return KeyCode.None;
    }
}
