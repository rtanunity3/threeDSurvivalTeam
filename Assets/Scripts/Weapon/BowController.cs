using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum ItemIndex
{
    Arrow = 332
}

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
        arrowSpawnObject = EquipManager.instance.ArrowSpawnObject;
    }

    //공격
    public override void OnAttackInput(PlayerConditions conditions)
    {
        if (!attacking)
        {
            if (conditions.UseStamina(useStamina))
            {
                if (inventroyScript.CheckHaveItem((int)ItemIndex.Arrow))
                {
                    animator.SetBool("Shoot", true);
                    attacking = true;

                    StartCoroutine(Shooting());

                    Invoke("OnCanAttack", attackRate);
                }
                else if (!inventroyScript.CheckHaveItem((int)ItemIndex.Arrow))
                {
                    Debug.Log("화살 없음");
                }
            }
        }
    }

    //화살 발사 로직
    private IEnumerator Shooting()
    {
        //인벤토리 화살 사용
        Inventory.instance.UseItme((int)ItemIndex.Arrow);

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

        //화살 생성
        GameObject arrow = Instantiate(ProjectileObject);
        arrow.GetComponent<TrailRenderer>().enabled = true;
        StartCoroutine(arrow.GetComponent<Arrow>().DestroyObject());
        animator.SetBool("Shoot", false);
        yield return null;
    }

    void OnCanAttack() => attacking = false;
}
