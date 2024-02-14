using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    private BowController bowController;
    private Rigidbody rigid;
    private TrailRenderer trailRenderer;

    private float calcSpeed;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        bowController = EquipManager.instance.curEquip.GetComponent<BowController>();
    }

    //private void Start()
    //{
    //    //기모으는 시간 곱적용
    //    //Debug.LogWarning("Arrow start");
    //    //init();
    //    //Shoot();
    //}

    private void OnEnable()
    {
        //Debug.LogWarning("Arrow OnEnable");
        Init();
        Shoot();
    }

    public void Init()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        //화살 생성 좌표
        transform.position = EquipManager.instance.ArrowSpawnObject.transform.position;
        transform.rotation = EquipManager.instance.ArrowSpawnObject.transform.rotation;

        calcSpeed = projectile_Speed * bowController.arrowForce;

        trailRenderer.Clear();
        trailRenderer.emitting = true;
        //trailRenderer.enabled = true;

        CancelInvoke("DestroyArrow");
        Invoke("DestroyArrow", 5f);
    }

    //velocity가 가리키는 방향을 바라보게 설정
    private void Update()
    {
        //Debug.Log(rigid.velocity);
        if (rigid.velocity != Vector3.zero) { transform.rotation = Quaternion.LookRotation(rigid.velocity); }
    }

    public void Shoot()
    {
        rigid.AddForce(transform.forward * calcSpeed, ForceMode.Impulse);
    }

    public void DestroyArrow()
    {
        trailRenderer.Clear();
        trailRenderer.emitting = false;
        //trailRenderer.enabled = false;
        ReleaseObject();
    }

    //public IEnumerator DestroyObject()
    //{
    //    yield return new WaitForSeconds(5f);
    //    ReleaseObject();
    //    //Destroy(gameObject);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            other.GetComponent<AnimalController>().TakePhysicalDamage(projectile_Damage);
            Debug.Log("명중");
        }

        if (other.CompareTag("Ground"))
        {
            CancelInvoke("DestroyArrow");
            DestroyArrow();
        }
    }
}
