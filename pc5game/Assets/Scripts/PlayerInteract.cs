using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float maxCastDist = 1.0f; // Distance that the object must be within for interaction.
    [SerializeField] private LayerMask rayLayer = 1<<8; // Layer that contains the interactable objects.
    [SerializeField] private string lockTag = "Lock";
    [SerializeField] private Color rayDebugColor = Color.red; // Colour of the debugging ray.

    Vector2 origin;
    Vector2 castDirection;

    /* Class for defining which keys do what action. */
    public static class KeyMap
    {
        public static KeyCode open = KeyCode.E;
        public static KeyCode inspect = KeyCode.Space;
    }


    /* Start is called before the first frame update */
    void Start()
    {
        /* Initially set raycast to cast "upwards" (forwards from sprite 
         * perspective) from player origin. */
        castDirection = transform.up;
    }

    /* Update is called once per frame */
    void Update()
    {
        /* Get the key that the user presses, if any. */
        KeyCode keyPressed = GetInput();

        /* If any key was pressed, interact with the object with the specified 
         * key. */
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

        /* If an object is hit, checking whether the object is interactable 
         * using hitInfo. */
        if (hit.collider != null)
        {
            Debug.Log("hit something\n");
            if (hit.collider.CompareTag(lockTag))
            {
                Debug.Log("hit lock\n");
                if( keyPressed == KeyMap.open)
                {
                    OpenLock(hit.collider.gameObject);
                }

                if (keyPressed == KeyMap.inspect)
                {
                    Debug.Log("wow look at this lock!\n");
                }
            }
        }
    }

    /* Method for getting user input. 
       RETURN: Key that was pressed, or KeyCode.None if no keys were pressed.*/
    private KeyCode GetInput()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Debug.Log("interacted with E\n");
            return KeyCode.E;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("interacted with space bar\n");
            return KeyCode.Space;
        }

        /* Return none if no keys pressed. */
        return KeyCode.None;
    }

    /* Method for opening a "lock" object.
     * PARAM: obj, The "lock" object that is to be "opened".
     * PRE: obj is a valid "lock" object. */
    private void OpenLock(GameObject obj)
    {
        Debug.Log("lock opened\n");
        //Destroy(obj);
        obj.GetComponent<InteractableController>().UpdateState();
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
