using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Attack")]
public class BaseAttack :  ScriptableObject
{
    public string attackName;
    public int attackID;
    
    public float attackDamage;

    public float arenaDistance;
    public float portionOfArena;
    public float attackRange;

    public float attackStaminaCost;
    
    public StatusType statusEffect;
    public enum StatusType {none, poison, stun, push};
    public float statusDuration;
    public float statusDamage;

    public float setupTime;
    public float waitingTime;

    public float attackCooldown;
    public float timeToBeReady;
    public bool attackReady = true;
        
    public void Reset()
    {
        timeToBeReady = 0;
        attackReady = true;
        attackRange = arenaDistance * portionOfArena;
    }

}
