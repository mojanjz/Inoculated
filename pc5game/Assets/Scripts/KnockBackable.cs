using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackable : MonoBehaviour
{
    // Start is called before the first frame update
    //[SerializeField] private float knockTime;
    [SerializeField] private float knockAmount;
    private Rigidbody2D rb;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        rb = GetComponent<Rigidbody2D>();   
    }

    public void knockDistance(Vector2 direction)
    {
        (gameObject.GetComponent("EnemyMovement") as MonoBehaviour).enabled = false;
        transform.Translate(direction * knockAmount);
       // rb.AddForce(transform.position * knockSpeed);
        (gameObject.GetComponent("EnemyMovement") as MonoBehaviour).enabled = true;
       
    }


}
