using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private KeyMap keyMap;
    //[SerializeField] private float speed;
    private bool isMoveEnabled = true;
    private Vector2 rotPrevDirection = Vector2.right; // Player rotation for previous frame
    private Vector2 rotCurrDirection; // Player rotation for current frame (is updated each frame)
    // private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    // private float rotAngle = 0; // For storing the angle of player orientation.

    [SerializeField] private Animator animator;

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
    }

    private void FixedUpdate()
    {
        GetInput();
        Move();
    }

    public void Move()
    {
        /* Disable movement if flag is unset. */
        if ( !isMoveEnabled)
        {
            rotCurrDirection = Vector2.zero;
        }

        /* If the rotation direction has changed, update it. */
        else if ((rotCurrDirection != Vector2.zero) && (rotCurrDirection != rotPrevDirection))
        {
            rotPrevDirection = rotCurrDirection;
            Rotate(rotPrevDirection);
        }

        /* Translates player in rotNewDirection vector (if a key is pressed). */
        transform.Translate(rotCurrDirection * gameObject.GetComponent<CharacterStats>().getSpeed() * Time.fixedDeltaTime);

        /* Tells the animation controller if the player is walking or idling. */
        animator.SetFloat("speed", rotCurrDirection.sqrMagnitude);
    }

    private void GetInput()
    {
        /* Stores the desired rotational vector (sum rotational vectors if want
         * to go diagonally when the user presses more than one key at the same 
         * time). */
        rotCurrDirection = Vector2.zero;

        if (Input.GetKey(keyMap.MoveUp))
        {
            rotCurrDirection = Vector2.up;
        }

        if (Input.GetKey(keyMap.MoveLeft))
        {
            rotCurrDirection = Vector2.left;
        }

        if (Input.GetKey(keyMap.MoveDown))
        {
            rotCurrDirection = Vector2.down;
        }

        if (Input.GetKey(keyMap.MoveRight))
        {
            rotCurrDirection = Vector2.right;
        }

        rotCurrDirection.Normalize();
    }

    /* Method to be called when the player (sprite) turns to face a new direction.
     * PARAM: rotDirection, the direction that the player sprite faces. */
    public void Rotate(Vector2 rotDirection)
    {
        /* Invoke the OnRotateEvent, passing the direction that the player should face. */
        OnRotateEvent.Invoke(rotDirection);

        ///* When rotDirection is (0, 1), the angle is 0 degrees. 
        // * Range is between -180 and 180 degrees. */
        //rotAngle = Vector2.SignedAngle(Vector2.right, rotDirection);

        //if (m_FacingRight && (rotAngle > 95 || rotAngle < -95))
        //{
        //    Flip();
        //}

        //else if (!m_FacingRight && (rotAngle < 85 && rotAngle > -85))
        //{
        //    Flip();
        //}

        //animator.SetFloat("rotation", rotAngle);

        animator.SetFloat("rotDirectionX", rotDirection.x);
        animator.SetFloat("rotDirectionY", rotDirection.y);

    }

    /* NOT USING METHOD BELOW BECAUSE SPRITES ARE NOT SYMMETRIC */
    ///* Method to change the whether the player sprite faces left or right. */
    //void Flip()
    //{
    //    /* Switch the way the player is labelled as facing. */
    //    m_FacingRight = !m_FacingRight;

    //    /* Mirror the player object across its local y-axis. */
    //    Vector3 theScale = transform.localScale;
    //    theScale.x *= -1;
    //    transform.localScale = theScale;
    //}

    public void EnableMove()
    {
        isMoveEnabled = true;
    }

    public void DisableMove()
    {
        isMoveEnabled = false;
    }


    public Vector2 getRotCurrDirection()
    {
        return rotCurrDirection;
    }

    public Vector2 getRotPrevDirection()
    {
        return rotPrevDirection;
    }

}


