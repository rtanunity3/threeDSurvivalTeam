using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowController : Equip
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;

    public float useStamina;

    public override void OnAttackInput(PlayerConditions conditions)
    {

    }
}
