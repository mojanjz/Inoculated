using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Attackable : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;
    [SerializeField] private int stunTimer;
    private string state;
    
    
    void Start()
    {
        health = maxHealth;
        state = "alive";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnAttack(int damage)
    {
        if (state != "stunned") 
        {
            health -= damage;
            if (health <= 0)
            {
                if (gameObject.tag == "Enemy")
                {
                    Stun(stunTimer);
                }
                else
                {
                    Die();
                }
            }
            else
            {
                //Debug.Log(gameObject.name + " has been hit, now has " + health + " health left");
            }
        }  
    }

    private void Stun(int time) 
    {
        state = "stunned";
        StartCoroutine(DisableMovementandReEnable(time));
    }

    public void Die()
    {
        Debug.Log("SNAPPED OUT OF EXISTENCE.");
        Destroy(gameObject);
    }

    private IEnumerator DisableMovementandReEnable(int time)
    {
        (gameObject.GetComponent("EnemyMovement") as MonoBehaviour).enabled = false;
        yield return new WaitForSeconds(time);
        health = maxHealth;
        state = "alive";
        gameObject.GetComponent<EnemyMovement>().findInitialTargets();
        (gameObject.GetComponent("EnemyMovement") as MonoBehaviour).enabled = true;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetStunTimer()
    {
        return stunTimer;
    }
}
