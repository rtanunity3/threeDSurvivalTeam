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
    public float arrowForce;

    [Header("Setting")]
    [SerializeField] private GameObject ProjectileObject;
    public GameObject arrowSpawnObject;
    public GameObject projectileAnimationObject;

    private Animator animator;
    private Inventory inventroyScript;
    

    private void Awake()
    {
        inventroyScript = GameManager.Instance.playerObject.GetComponent<Inventory>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        arrowSpawnObject = EquipManager.instance.ArrowSpawnPosition;
    }

    //공격
    public override void OnAttackInput(PlayerConditions conditions)
    {
        if (!attacking)
        {
            if (conditions.UseStamina(useStamina))
            {
                if (inventroyScript.CheckHaveItem(332))
                {
                    animator.SetBool("Shoot", true);
                    attacking = true;

                    StartCoroutine(Shooting());

                    Invoke("OnCanAttack", attackRate);
                    Debug.Log("활 공격");
                }
                else if (!inventroyScript.CheckHaveItem(332))
                {
                    Debug.Log("화살 없음");
                }
            }
        }
    }

    //화살 발사 로직
    private IEnumerator Shooting()
    {
        float forceTime = 0;
        while (true)
        {
            forceTime = (forceTime < 2) ? forceTime += Time.deltaTime : forceTime = 2;

            if (Input.GetMouseButtonUp(0))
            {
                arrowForce = forceTime;
                break;
            }

            yield return null;
        }

        Instantiate(ProjectileObject, arrowSpawnObject.transform);
        animator.SetBool("Shoot", false);
        projectileAnimationObject.SetActive(false);
        yield return null;
    }

    void OnCanAttack() => attacking = false;
}
