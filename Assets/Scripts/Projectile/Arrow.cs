using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    private BowController bowController;
    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        bowController = EquipManager.instance.curEquip.GetComponent<BowController>();
    }

    private void Start()
    {
        //화살 생성 좌표
        transform.position = EquipManager.instance.ArrowSpawnObject.transform.position;
        transform.rotation = EquipManager.instance.ArrowSpawnObject.transform.rotation;

        //기모으는 시간 곱적용
        projectile_Speed *= bowController.arrowForce;
        Shoot();
    }

    //velocity가 가리키는 방향을 바라보게 설정
    private void Update()
    {
        if (rigid.velocity != Vector3.zero) { transform.rotation = Quaternion.LookRotation(rigid.velocity); }
    }

    public void Shoot() => rigid.AddForce(transform.forward * projectile_Speed, ForceMode.Impulse);
    public IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Animal"))
        {
            other.GetComponent<AnimalController>().TakePhysicalDamage(projectile_Damage);
            Debug.Log("명중");
        }
    }
}
