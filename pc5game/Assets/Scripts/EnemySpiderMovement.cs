using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpiderMovement : MonoBehaviour
{
    //[SerializeField] private float speed;
    private Transform target;
    private Transform nonTarget;
    [SerializeField] private float aquisitionTime;  //time required to change target
    [SerializeField] private Vector2 direction;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float runTime;
    [SerializeField] private float pauseTime;
    private float count;
    public int animationSpeedMulitplier;

    float runningCount;
    float pauseCount;

    // Start is called before the first frame update
    void Start()
    {
        runningCount = 0;
        pauseCount = 0;
        findInitialTargets();
    }

    // Update is called once per frame
    void Update()
    {
        targetAquisition();
        checkForMovement();
        //moveTowardsTarget();
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
            count = count + 1 * Time.deltaTime;
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
        //float distanceToTarget = Vector2.Distance(transform.position, target.position);
        float x_position_diff = target.position.x - transform.position.x;
        float y_position_diff = target.position.y - transform.position.y;
        

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

        transform.Translate(direction * gameObject.GetComponent<CharacterStats>().getSpeed() * speedMultiplier *Time.fixedDeltaTime);
        //transform.position = Vector2.MoveTowards(transform.position, target.position, gameObject.GetComponent<CharacterStats>().getSpeed()* speedMultiplier * Time.deltaTime);
    }

    private void checkForMovement()
    {
        if (runningCount < runTime)
        {
            runningCount = runningCount + 1 * Time.deltaTime;
            moveTowardsTarget();
        }

        else if (pauseCount < pauseTime)
        {
            pauseCount = pauseCount + 1 * Time.deltaTime;
            direction = -direction;
        }
        else
        {
            pauseCount = 0;
            runningCount = 0;
        }
    }

    public float getAnimationSpeed()
    {
        return animationSpeedMulitplier * gameObject.GetComponent<CharacterStats>().getSpeed();
    }
}
