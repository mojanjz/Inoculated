using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float maxCastDist = 1.0f; // Distance that the object must be within for interaction.
    [SerializeField] private Color rayDebugColor = Color.red; // Colour of the debugging ray.
    [SerializeField] private LayerMask rayLayer = 1<<8; // Layer that contains the interactable objects (touchable by raycast).
    [SerializeField] private PlayerController playerController; // PlayerController script

    private Vector2 origin; // Origin of raycast
    private Vector2 castDirection; // Direction of raycast
    private KeyCode keyPressed; // Stores the interaction key that the user presses.

    /* Class for defining which keys do what action. */
    public static class KeyMap
    {
        public static KeyCode unlock = KeyCode.E;
        public static KeyCode inspect = KeyCode.F;
        public static KeyCode attack = KeyCode.Space;
    }


    /* Start is called before the first frame update */
    void Start()
    {
        /* Initially set raycast to cast leftwards (ie. forwards from sprite 
         * perspective) from player origin. */
        castDirection = transform.right;
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

    /* Method for interacting with other objects.
     * PARAM: keyPressed, the keyboard key that was pressed by the user. */
    private void Interact(KeyCode keyPressed)
    {
        /* Set raycast to start at player origin. */
        origin = transform.position;

        /* Cast a ray forwards from the player. */
        RaycastHit2D hit = Physics2D.Raycast(origin, castDirection, maxCastDist, rayLayer);

        /* Visually shows the raycast for debugging purposes. */
        Debug.DrawRay(origin, castDirection * maxCastDist, rayDebugColor);

        if (hit.collider != null)
        {
            Debug.Log("hit something\n");
        }

        if (keyPressed == KeyMap.unlock)
        {
            var unlockable = hit.collider.GetComponent<Unlockable>();
            if (unlockable)
            {
                Debug.Log("Unlocking\n");
                unlockable.OnUnlock();
            }
        }

        if (keyPressed == KeyMap.inspect)
        {
            var inspectable = hit.collider.GetComponent<Inspectable>();
            if (inspectable)
            {
                Debug.Log("wow, check this out!\n");
                inspectable.OnInspect();
            }
        }

        if (keyPressed == KeyMap.attack)
        {
            Debug.Log("BAM BAM\n");
            StartCoroutine(playerController.Attack());

            if (hit.collider != null)
            {
                var attackable = hit.collider.GetComponent<Attackable>();

                if (attackable)
                {
                    Debug.Log("now u die\n");
                    attackable.OnAttack();
                }
            }
        }
    }

    /* Method for getting user input. 
       RETURN: Key that was pressed, or KeyCode.None if no valid keys were pressed.*/
    private KeyCode GetInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("interacted with E\n");
            return KeyCode.E;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("interacted with F\n");
            return KeyCode.F;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("interacted with space bar\n");
            return KeyCode.Space;
        }

        /* Return none if no valid keys pressed. */
        return KeyCode.None;
    }

    /* Method to be called when the player (sprite) turns to face a new 
     * direction.
     * PARAM: rotDirection, the direction that the player sprite faces. */
    public void OnRotate(Vector2 rotDirection)
    {
        /* Sets raycast to point in the direction that the player sprite is 
         * facing. */
        castDirection = rotDirection;
    }

}
