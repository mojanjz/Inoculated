using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Attackable : MonoBehaviour
{
    [SerializeField] private bool killable;
    [SerializeField] private int stunTime;
    [SerializeField] HealthBar healthBar;
    public EnemySpawnBehaviour EnemySpawner; // Need to keep track of number of enemies... reorganize later

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
        float size = (float)gameObject.GetComponent<CharacterStats>().getCurrentHealth() / (float)maxHP;
        if (healthBar)
            healthBar.SetSize(size);
    }

    public void OnAttack(int damage)
    {
        gameObject.GetComponent<CharacterStats>().updateHealth(-damage);
    }

    private void Stun(int time) 
    {
        StartCoroutine(DisableMovement(time));
    }

    IEnumerator DisableMovement(int time)
    {
        gameObject.GetComponent<CharacterStats>().setSpeed(0);
        yield return new WaitForSeconds(time);
        gameObject.GetComponent<CharacterStats>().setSpeed(speed_reserve);
        gameObject.GetComponent<CharacterStats>().setCurrentHealth(maxHP);
        state = "alive";
    }

    public void Die()
    {
        //moves really far away
        state = "dead";
        //gameObject.active = false;
        //transform.position = new Vector2(999, 999);
        Destroy(healthBar);
        Destroy(gameObject);
        EnemySpawner.Count--;
    }

    public int GetStunTime()
    {
        return stunTime;
    }
}
