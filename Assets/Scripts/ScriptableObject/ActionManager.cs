using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Action Manager")]
public class ActionManager : ScriptableObject
{
    public BaseAttack[] availableAttacks;
    public BaseAttack selectedAttack;
    
    public void ResetAttack()
    {
        foreach (BaseAttack a in availableAttacks)
            a.Reset();
    }
}
