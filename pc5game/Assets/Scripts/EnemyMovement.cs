using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //[SerializeField] private float speed;
    private Transform target;
    private Transform nonTarget;
    [SerializeField] private float aquisitionTime;  //time required to change target
    public int animationSpeedMulitplier;
    [SerializeField] public Vector2 direction;
    [SerializeField] public float stoppingDistance;
    private float count;

    [SerializeField] Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        findInitialTargets();
    }

    // Update is called once per frame
    void Update()
    {
        targetAquisition();
        if (Vector2.Distance(transform.position, target.position) > stoppingDistance) 
        {
            moveTowardsTarget();
        }
 
    }

    public void findInitialTargets()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        float distance0 = Vector2.Distance(transform.position, playerList[0].transform.position);
        float distance1 = Vector2.Distance(transform.position, playerList[1].transform.position);
        if (distance0 <= distance1)
        {
            target = playerList[0].transform;
            nonTarget = playerList[1].transform;
        }
        else
        {
            target = playerList[1].transform;
            nonTarget = playerList[0].transform;
        }
    }

    private void targetAquisition() 
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        float distanceToNonTarget = Vector2.Distance(transform.position, nonTarget.position);
        Transform temp;

        if (distanceToNonTarget < distanceToTarget)
        {
            count = count + 1*Time.deltaTime;
        }
        else
        {
            count = 0;
        }

        //Switches target player
        if (count >= aquisitionTime)
        {
            temp = target;
            target = nonTarget;
            nonTarget = temp;
            count = 0;
        }
    }

    private void moveTowardsTarget()
    {
        float distanceToTarget = getTargetDistance();
        float x_position_diff = target.position.x - transform.position.x;
        float y_position_diff = target.position.y - transform.position.y;

        // UNI-DIRECTIONAL Vector, 

        if (Mathf.Abs(x_position_diff) > Mathf.Abs(y_position_diff))
        {
            if (x_position_diff < 0)
            {
                direction = Vector2.left;
            }
            else
            {
                direction = Vector2.right;
            }
        }
        else
        {
            if (y_position_diff < 0)
            {
                direction = Vector2.down;
            }
            else
            {
                direction = Vector2.up;
            }
        }


        //Multiple direction Vector
        //direction = (target.position - transform.position).normalized;

        // Control animation
        animator.SetFloat("rotDirectionX", direction.x);
        animator.SetFloat("rotDirectionY", direction.y);

        //move in direction
        //transform.Translate(direction * gameObject.GetComponent<CharacterStats>().getSpeed() * Time.fixedDeltaTime);

        //move-towards regardless of direction input
        transform.position = Vector2.MoveTowards(transform.position, target.position, gameObject.GetComponent<CharacterStats>().getSpeed() * Time.deltaTime);
    }

    public float getAnimationSpeed() {
        return animationSpeedMulitplier * gameObject.GetComponent<CharacterStats>().getSpeed();
    }

    public Transform getTarget() {
        return target;
    }

    public float getTargetDistance() 
    {
        return Vector2.Distance(transform.position, target.position); 
    }
}
