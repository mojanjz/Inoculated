using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Attackable : MonoBehaviour
{
    [SerializeField] private bool killable;
    [SerializeField] private int stunTime;
    [SerializeField] HealthBar healthBar;

    string state;
    float speed_reserve;
    int maxHP;
    void Start()
    {
       state = "alive";
       maxHP = gameObject.GetComponent<CharacterStats>().getMaxHealth();
       speed_reserve = gameObject.GetComponent<CharacterStats>().getSpeed();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnAttack(int damage)
    {
        if (state != "stunned") 
        {
            gameObject.GetComponent<CharacterStats>().updateHealth(-damage);
            float size = gameObject.GetComponent<CharacterStats>().getCurrentHealth()/100f;
            // health -= damage;
            // float size = health / 100;

            if(healthBar)
            healthBar.SetSize(size);

            if (gameObject.GetComponent<CharacterStats>().getCurrentHealth() <= 0)
            {
                if (!killable)
                {
                    Stun(stunTime);
                    
                }
                else
                {
                    Die();
                }
            }
        }  
    }

    private void Stun(int time) 
    {
        StartCoroutine(DisableMovement());
    }

    IEnumerator DisableMovement()
    {
        gameObject.GetComponent<CharacterStats>().setSpeed(0);
        yield return new WaitForSeconds(stunTime);
        gameObject.GetComponent<CharacterStats>().setSpeed(speed_reserve);
        gameObject.GetComponent<CharacterStats>().setCurrentHealth(maxHP);
        state = "alive";
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public int GetStunTime()
    {
        return stunTime;
    }
}
