using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 direction;
    private Vector2 rotDirection = Vector2.right;
    private Vector2 rotNewDirection;
    // private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    // private float rotAngle = 0; // For storing the angle of player orientation.

    [SerializeField] private Animator animator;

    public bool isAttacking = false;

    /* Event to update relevant methods when the player's rotation is changed. */
    [System.Serializable]
    public class Vec2Event : UnityEvent<Vector2> { }
    public Vec2Event OnRotateEvent;

    public UnityEvent OnAttackEvent;

    private void Awake()
    {
        if (OnRotateEvent == null)
            OnRotateEvent = new Vec2Event();

        if (OnAttackEvent == null)
            OnAttackEvent = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
    }

    public void Move()
    {
        /* Set the rotation only if there was input. Otherwise, retain the old orientation.
         * Also set the translation direction vector to match the input rotation. 
         * Don't move if currently attacking. */
        if ((rotNewDirection != Vector2.zero) & !isAttacking)
        {
            if (rotDirection != rotNewDirection)
            {
                rotDirection = rotNewDirection;
                Rotate(rotDirection);
            }

            direction = rotNewDirection;
        }

        /* Translates player in direction vector (if a key is pressed). */
        transform.Translate(direction * speed * Time.deltaTime);

        /* Tells the animation controller if the player is walking or idling. */
        animator.SetFloat("speed", direction.sqrMagnitude);
    }

    private void GetInput()
    {
        /* Keeps the player stationary when no input is detected. */
        direction = Vector2.zero;

        /* Stores the desired rotational vector (sum rotational vectors if want
         * to go diagonally when the user presses more than one key at the same 
         * time). */
        rotNewDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            rotNewDirection = Vector2.up;
        }

        if (Input.GetKey(KeyCode.A))
        {
            rotNewDirection = Vector2.left;
        }

        if (Input.GetKey(KeyCode.S))
        {
            rotNewDirection = Vector2.down;
        }

        if (Input.GetKey(KeyCode.D))
        {
            rotNewDirection = Vector2.right;
        }

        rotNewDirection.Normalize();
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

    public IEnumerator Attack()
    {
        isAttacking = true;

        /* Invokes methods in other relevant scripts as CharacterAudioController. */
        OnAttackEvent.Invoke();

        /* Play player attack animation. */
        animator.SetBool("attack", true);

        /* Switch isAttacking state after the attack animation has completed. */
        float myTime = animator.GetCurrentAnimatorStateInfo(2).length;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(2).length);
        isAttacking = false;
    }
}
