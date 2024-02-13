using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowController : Equip
{
    public float attackRate;
    private bool attacking;
    public float useStamina;


    public override void OnAttackInput(PlayerConditions conditions)
    {
        if (!attacking)
        {
            if (conditions.UseStamina(useStamina))
            {
                attacking = true;
                //animator.SetTrigger("Attack");
                Invoke("OnCanAttack", attackRate);
                Debug.Log("활 공격");
            }
        }
    }
     
    void OnCanAttack() => attacking = false;
}
