using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform target;
    [SerializeField] private Transform nonTarget;
    [SerializeField] private float aquisitionTime;
    [SerializeField] float count;
    


    // Start is called before the first frame update
    void Start()
    {
        //target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        //nonTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        targetAquisition();
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
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

        if (count >= aquisitionTime)
        {
            temp = target;
            target = nonTarget;
            nonTarget = temp;
        }



    }

   

}
