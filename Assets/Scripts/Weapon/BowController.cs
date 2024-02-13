using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class BowController : Equip
{
    [Header("Attack Status")]
    [SerializeField] private float attackRate;
    private bool attacking;
    [SerializeField] private float useStamina;

    [Header("Setting")]
    [SerializeField] private GameObject arrowObject;
    public GameObject arrowSpawnPosition;

    private Inventory inventroyScript;

    private void Awake()
    {
        inventroyScript = GameManager.Instance.playerObject.GetComponent<Inventory>();
    }


    //공격
    public override void OnAttackInput(PlayerConditions conditions)
    {
        if (!attacking)
        {
            if (conditions.UseStamina(useStamina))
            {
                //animator.SetTrigger("Attack");
                if(inventroyScript.CheckHaveItem(332))
                {
                    attacking = true;
                    Invoke("OnCanAttack", attackRate);
                    Debug.Log("활 공격");
                }
                else if(!inventroyScript.CheckHaveItem(332))
                {
                    Debug.Log("화살 없음");
                }    
            }
        }
    }
    
    

    void OnCanAttack() => attacking = false;
}
