using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public string SpeakerName;
    public Dialogue.PanelID PanelID = Dialogue.PanelID.Null;

    public int maxHealth;
    public int currentHealth;
    public float speed;
    public int baseHit;
    public float damageMultiplier;

    void Start()
    {
        currentHealth = maxHealth;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Getters and setters
    public int getMaxHealth()
    {
        return maxHealth;
    }

    public void setMaxHealth(int newMax)
    {
        maxHealth = newMax;
    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }

    public void setCurrentHealth(int newHealth)
    {
        currentHealth = newHealth;
    }

    public void updateHealth(int delta)
    {
        currentHealth += delta;
    }


    public float getSpeed()
    {
        return speed;
    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void updateSpeed(int delta)
    {
        speed += delta;
    }

    public int getBaseHit()
    {
        return baseHit;
    }

    public void setBaseHit(int newBase)
    {
        baseHit = newBase;
    }

    public float getDamageMultiplier()
    {
        return damageMultiplier;
    }

    public void setDamageMultiplier(float newMultiplier)
    {
        damageMultiplier = newMultiplier;
    }

}
