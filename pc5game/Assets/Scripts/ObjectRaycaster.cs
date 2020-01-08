using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Component to control casting of a linear ray from the object origin. */
public class ObjectRaycaster : MonoBehaviour
{
    /* Interaction settings */
    [SerializeField] private Color rayDebugColor = Color.red; // Colour of the debugging ray.
    [SerializeField] private float rayDebugDuration = 0.1f; // How long the debug ray shows on screen
    [SerializeField] private Vector2 InitialCastDirection = Vector2.right;

    /* Internally used variables */
    private Vector2 origin; // Origin of raycast
    private Vector2 castDirection; // Direction of raycast

    // Start is called before the first frame update
    void Start()
    {
        castDirection = InitialCastDirection;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RaycastHit2D ObjectRaycast(float distance, LayerMask layer)
    {
        /* Set raycast to start at object origin. */
        origin = transform.position;

        /* Cast a ray forwards from the player. */
        RaycastHit2D hit = Physics2D.Raycast(origin, castDirection, distance, layer);

        /* Show the ray visually for debugging purposes. */
        Debug.DrawRay(origin, castDirection * distance, rayDebugColor, rayDebugDuration);

        return hit;
    }

    /* Method to be called when the player (sprite) turns to face a new 
     * direction.
     * PARAM: rotDirection, the direction that the player sprite faces.
     * POST: Raycast direction is set to match rotDirection. */
    public void OnRotate(Vector2 rotDirection)
    {
        /* Sets raycast to point in the direction that the player sprite is 
         * facing. */
        castDirection = rotDirection;
    }
}
