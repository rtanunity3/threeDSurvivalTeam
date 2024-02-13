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
        //기모으는 시간 곱적용
        projectile_Speed *= bowController.arrowForce;
        Shoot();
    }
    public void Shoot()
    {
        rigid.AddForce(Camera.main.ScreenPointToRay(Input.mousePosition).direction * projectile_Speed, ForceMode.Impulse);
    }
}
