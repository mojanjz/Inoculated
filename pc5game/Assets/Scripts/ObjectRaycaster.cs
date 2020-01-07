using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Component to control casting of a linear ray from the object origin. */
public class ObjectRaycaster : MonoBehaviour
{
    /* Interaction settings */
    [SerializeField] private float maxCastDist = 1.0f; // Distance that the object must be within for interaction.
    [SerializeField] private Color rayDebugColor = Color.red; // Colour of the debugging ray.
    [SerializeField] private float rayDebugDuration = 0.1f; // How long the debug ray shows on screen
    [SerializeField] private LayerMask rayLayer = 1 << 8; // Layer that contains the interactable objects (touchable by raycast).
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

    public RaycastHit2D ObjectRaycast()
    {
        /* Set raycast to start at object origin. */
        origin = transform.position;

        /* Cast a ray forwards from the player. */
        RaycastHit2D hit = Physics2D.Raycast(origin, castDirection, maxCastDist, rayLayer);

        /* Row the ray visually for debugging purposes. */
        DebugObjectRaycast();

        return hit;
    }

    private void DebugObjectRaycast()
    {
        /* Set raycast to start at object origin. */
        origin = transform.position;

        /* Visually shows the raycast for debugging purposes. */
        Debug.DrawRay(origin, castDirection * maxCastDist, rayDebugColor, rayDebugDuration);
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
