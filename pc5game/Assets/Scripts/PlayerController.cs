using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private Vector2 direction;
    private Vector2 rotDirection = Vector2.right;
    private Vector2 rotNewDirection;
   // private bool m_FacingRight = true;  // For determining which way the player is currently facing.
   // private float rotAngle = 0; // For storing the angle of player orientation.

    [SerializeField]
    private Animator animator;

    /* Event to update relevant methods when the player's rotation is changed. */
    [System.Serializable]
    public class Vec2Event : UnityEvent<Vector2> { }
    public Vec2Event OnRotateEvent;

    private void Awake()
    {
        if (OnRotateEvent == null)
            OnRotateEvent = new Vec2Event();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        getInput();
        move();
    }

    public void move()
    {
        /* Set the rotation only if there was input. Otherwise, retain the old orientation.
         * Also set the translation direction vector to match the input rotation. */
        if (rotNewDirection != Vector2.zero)
        {
            if (rotDirection != rotNewDirection)
            {
                rotDirection = rotNewDirection;

                /* Invoke the OnRotateEvent, passing the direction that the player should face. */
                OnRotateEvent.Invoke(rotDirection);
            }

            direction = rotNewDirection;
        }

        /* Translates player in direction vector (if a key is pressed). */
        transform.Translate(direction * speed * Time.deltaTime);

        /* Tells the animation controller if the player is walking or idling. */
        animator.SetFloat("speed", direction.sqrMagnitude);
    }

    private void getInput()
    {
        /* Keeps the player stationary when no input is detected. */
        direction = Vector2.zero;

        /* Stores the desired rotational vector (sum rotational vectors if the 
         * user presses more than one key at the same time). */
        rotNewDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            rotNewDirection += Vector2.up;
        }

        if (Input.GetKey(KeyCode.A))
        {
            rotNewDirection += Vector2.left;
        }

        if (Input.GetKey(KeyCode.S))
        {
            rotNewDirection += Vector2.down;
        }

        if (Input.GetKey(KeyCode.D))
        {
            rotNewDirection += Vector2.right;
        }

        rotNewDirection.Normalize();
    }

    /* Method to be called when the player (sprite) turns to face a new direction.
     * PARAM: rotDirection, the direction that the player sprite faces. */
    public void OnRotate(Vector2 rotDirection)
    {
        ///* When rotDirection is (0, 1), the angle is 0 degrees. 
        // * Range is between -180 and 180 degrees. */
        //rotAngle = Vector2.SignedAngle(Vector2.right, rotDirection);

        //if (m_FacingRight && (rotAngle > 95 || rotAngle < -95))
        //{
        //    flip();
        //}

        //else if (!m_FacingRight && (rotAngle < 85 && rotAngle > -85))
        //{
        //    flip();
        //}

        //animator.SetFloat("rotation", rotAngle);

        animator.SetFloat("rotDirectionX", rotDirection.x);
        animator.SetFloat("rotDirectionY", rotDirection.y);

    }

    /* NOT USING METHOD BELOW BECAUSE SPRITES ARE NOT SYMMETRIC */
    ///* Method to change the whether the player sprite faces left or right. */
    //void flip()
    //{
    //    /* Switch the way the player is labelled as facing. */
    //    m_FacingRight = !m_FacingRight;

    //    /* Mirror the player object across its local y-axis. */
    //    Vector3 theScale = transform.localScale;
    //    theScale.x *= -1;
    //    transform.localScale = theScale;
    //}
}
