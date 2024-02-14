using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering,
    Attacking,
    Fleeing,
}

public enum AnimalType
{
    Herbivore,
    Carnivore
}

public class AnimalController : PoolAble, IDamagable
{
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;

    [Header("AI")]
    public AIState aiState;
    public AnimalType animalType;
    public float detectDistance;
    public float safeDistance;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;

    public float fieldOfView = 120f;

    private NavMeshAgent agent;
    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private bool enableSetDestination = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        SetState(AIState.Wandering);
    }

    private void Update()
    {
        //플레이어와의 거리
        playerDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

        animator.SetBool("Moving", aiState != AIState.Idle && aiState != AIState.Fleeing);

        switch (aiState)
        {
            case AIState.Idle: PassiveUpdate(); break;
            case AIState.Wandering: PassiveUpdate(); break;
            case AIState.Attacking: AttackingUpdate(); break;
            case AIState.Fleeing: FleeingUpdate(); break;
        }

    }

    private void FleeingUpdate()
    {
        //지정한 목적지에 도착 했다면
        if (agent.remainingDistance < 0.1f)
        {
            agent.SetDestination(GetFleeLocation());
        }

        if (playerDistance > safeDistance)
        {
            //Debug.Log($"{playerDistance}, {detectDistance}");
            //Debug.Log("추적에서 벗어남");
            SetState(AIState.Wandering);
            animator.SetBool("Fleeing", false);
        }
    }

    //추적
    private void AttackingUpdate()
    {
        if (playerDistance > attackDistance || !isplayerInFieldofView())
        {
            agent.isStopped = false;
            NavMeshPath path = new NavMeshPath();

            //플레이어 위치까지의 경로가 확인되는지
            if (agent.CalculatePath(PlayerController.instance.transform.position, path))
            {
                //플레이어 위치로 이동
                agent.SetDestination(PlayerController.instance.transform.position);
            }
        }
        else
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackRate && animalType == AnimalType.Carnivore)
            {
                lastAttackTime = Time.time;
                PlayerController.instance.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
        }
    }

    private void PassiveUpdate()
    {
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        //플레이어 거리가 감지 범위 내로 들어왔을 경우
        if (playerDistance < detectDistance)
        {
            switch (animalType)
            {
                //육식 동물
                case AnimalType.Carnivore:
                    SetState(AIState.Attacking);
                    break;

                //초식 동물
                case AnimalType.Herbivore:
                    SetState(AIState.Fleeing);
                    animator.SetBool("Fleeing", true);
                    break;
            }
        }
    }

    bool isplayerInFieldofView()
    {
        Vector3 directionToPlayer = PlayerController.instance.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < fieldOfView * 0.5f;
    }

    private void SetState(AIState newState)
    {
        aiState = newState;
        switch (aiState)
        {
            case AIState.Idle:
                {
                    agent.speed = walkSpeed;
                    agent.isStopped = true;
                }
                break;
            case AIState.Wandering:
                {
                    agent.speed = walkSpeed;
                    agent.isStopped = false;
                }
                break;
            case AIState.Attacking:
                {
                    agent.speed = runSpeed;
                    agent.isStopped = false;
                }
                break;
            case AIState.Fleeing:
                {
                    agent.speed = runSpeed;
                    agent.isStopped = false;
                }
                break;
        }

        animator.speed = agent.speed / walkSpeed;
    }

    public void WanderToNewLocation()
    {
        if (aiState != AIState.Idle)
        {
            return;
        }
        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position;
    }

    //도망갈 장소
    Vector3 GetFleeLocation()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (GetDesinationAngle(hit.position) > 90 || playerDistance < safeDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position;
    }

    float GetDesinationAngle(Vector3 targetPos)
    {
        return Vector3.Angle(transform.position - PlayerController.instance.transform.position, transform.position + targetPos);
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
            Die();

        StartCoroutine(DamageFlash());
    }

    void Die()
    {
        for (int x = 0; x < dropOnDeath.Length; x++)
        {
            Instantiate(dropOnDeath[x].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = new Color(1.0f, 0.6f, 0.6f);

        yield return new WaitForSeconds(0.1f);
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = Color.white;
    }

    //목적지 재설정
    public void OnReSetDestination()
    {
        if (enableSetDestination)
        {
            StartCoroutine(ReSetDesination());
        }
    }

    IEnumerator ReSetDesination()
    {
        enableSetDestination = false;
        agent.SetDestination(SetBackLocation());
        yield return new WaitForSeconds(3f);
        enableSetDestination = true;
    }

    Vector3 SetBackLocation()
    {
        NavMeshHit hit;

        for (int i = 0; i < 30; i++)
        {
            if (NavMesh.SamplePosition(transform.position, out hit, minWanderDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        NavMesh.SamplePosition(transform.position, out hit, minWanderDistance, NavMesh.AllAreas);
        return hit.position;
    }
}
