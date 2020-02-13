using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform target;
    [SerializeField] private Transform nonTarget;
    [SerializeField] private float aquisitionTime;  //time required to change target
    [SerializeField] private float minDistanceRadius;
    private string moveState;
    private float count;
    
    // Start is called before the first frame update
    void Start()
    {
        findInitialTargets();
    }

    // Update is called once per frame
    void Update()
    {
        targetAquisition();
        moveTowardsTarget();
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
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
   

}
