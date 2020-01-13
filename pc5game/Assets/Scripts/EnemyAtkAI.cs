using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAtkAI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float attackTime;
    private float remainingTime;

    //public UnityEvent OnAttackStartEvent;
    
    void Start()
    {
        remainingTime = attackTime;
    }

    // Update is called once per frame
    void Update()
    {
        float timeElapse = Time.deltaTime;

        if (remainingTime > 0)
        {
            remainingTime -= timeElapse;
        }
        else
        {
            remainingTime = attackTime;
        }
      
    }
}
