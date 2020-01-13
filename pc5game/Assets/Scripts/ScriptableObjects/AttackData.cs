using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "ScriptableObjects/Attack", order = 35)]
public class AttackData : ScriptableObject
{
    public string Name;
    public int Damage;
    public AudioClip Audio;
    public float Area;
    public float Distance;
    public AnimatorOverrideController animController;


    public int getDamage() 
    {
        return Damage;
    }
}


