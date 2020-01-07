using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    /* Interaction settings */
    [SerializeField] private float maxCastDist = 1.0f; // Distance that the object must be within for interaction.
    [SerializeField] private Color rayDebugColor = Color.red; // Colour of the debugging ray.
    [SerializeField] private LayerMask rayLayer = 1<<8; // Layer that contains the interactable objects (touchable by raycast).

    /* Relevant interaction behaviour controllers. */
    [SerializeField] private ObjectRaycaster rayCaster;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Unlocker unlocker;
    [SerializeField] private Attacker attacker;
    [SerializeField] private Examiner examiner;

    /* Internally used variables */
    private Vector2 origin; // Origin of raycast
    private Vector2 castDirection; // Direction of raycast
    private KeyCode keyPressed; // Stores the interaction key that the user presses.

    /* Start is called before the first frame update */
    void Start()
    {
        ///* Initially set raycast to cast leftwards (ie. forwards from sprite 
        // * perspective) from player origin. */
        //castDirection = transform.right;
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

    void FixedUpdate()
    {
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
