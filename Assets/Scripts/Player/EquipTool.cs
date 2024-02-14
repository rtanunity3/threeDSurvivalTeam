using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;

    public float useStamina;

    [Header("Resource Gathering")]
    public bool doseGatherResources;

    [Header("Combat")]
    public bool doseDealDamage;
    public int damage;

    private Animator animator;
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;
        animator = GetComponent<Animator>();

    }

    public override void OnAttackInput(PlayerConditions conditions)
    {
        if (!attacking)
        {
            if (conditions.UseStamina(useStamina))
            {
                attacking = true;
                animator.SetTrigger("Attack");
                Invoke("OnCanAttack", attackRate);
            }
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            if (doseGatherResources && hit.collider.TryGetComponent(out Resource resource))
            {
                resource.Gather(hit.point, hit.normal);
                switch (resource.itemToGive.displayName)
                {
                    case "나무":
                        SoundManager.instacne.PlayEffectSound(EffectSound.AxeWood);
                        break;

                    case "돌":
                        SoundManager.instacne.PlayEffectSound(EffectSound.AxeStone);
                        break;
                }
            }

            if (doseDealDamage && hit.collider.TryGetComponent(out IDamagable damagable))
            {
                damagable.TakePhysicalDamage(damage);
            }
        }
    }

    public void PlayUseEffectSound(EffectSound effectSound)
    {
        SoundManager.instacne.PlayEffectSound(effectSound);
    }
}


