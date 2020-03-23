using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    /* Relevant interaction behaviour controllers. */
    [SerializeField] private KeyMap keyMap;
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
            Interact(keyPressed);
        }
    }

    /* Method to initiate player interactions.
     * PARAM: keyPressed, the keyboard key that was pressed by the user. */
    private void Interact(KeyCode keyPressed)
    {
        OnInteractStart();

        if (keyPressed == keyMap.Unlock)
        {
            Debug.Log("Trying to open...");
            unlocker.Unlock();
        }

        if (keyPressed == keyMap.Examine)
        {
            Debug.Log("Looking...");
            examiner.Examine();
        }

        if (keyPressed == keyMap.Attack)
        {
            Debug.Log("BAM BAM");
            StartCoroutine(attacker.Attack());
        }

        // TEMP CODE FOR DEMO PURPOSES
        if (Input.GetKeyDown(keyMap.AttackSwitch))
        {
            Debug.Log("Switching weapon...");
            if (attacker.AtkIndex == 0)
            {
                try
                {
                    attacker.SwitchAttack("Weapon");
                } catch (Attacker.InvalidAttackName ex)
                {
                    // Ignore for now
                    Debug.Log(ex);
                }
                
            }
            else
            {
                attacker.SwitchAttack("Regular");
            }
            playerController.EnableMove();
        }
    }

    /* Method for getting user input. 
       RETURN: Key that was pressed, or KeyCode.None if no valid keys were pressed.*/
    private KeyCode GetInput()
    {
        if (Input.GetKeyDown(keyMap.Examine))
        {
            Debug.Log("interacted with " + keyMap.Examine);
            return keyMap.Examine;
        }

        if (Input.GetKeyDown(keyMap.Unlock))
        {
            Debug.Log("interacted with " + keyMap.Unlock);
            return keyMap.Unlock;
        }

        if (Input.GetKeyDown(keyMap.Attack))
        {
            Debug.Log("interacted with " + keyMap.Attack);
            return keyMap.Attack;
        }

        // TEMP CODE FOR DEMO PURPOSES
        if (Input.GetKeyDown(keyMap.AttackSwitch))
        {
            Debug.Log("interacted with " + keyMap.AttackSwitch);
            return keyMap.AttackSwitch;
        }

            /* Return none if no valid keys pressed. */
            return KeyCode.None;
    }

    public void OnInteractStart()
    {
        enabled = false;
        playerController.DisableMove();
    }

    // Added to Examiner, Attacker in the inspector
    public void OnInteractEnd()
    {
        enabled = true;
        playerController.EnableMove();
    }
}
