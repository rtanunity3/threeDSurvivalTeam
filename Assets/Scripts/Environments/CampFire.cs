using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate;

    private List<IDamagable> thingsToDamage = new List<IDamagable>();

    private void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate); // 지연실행 0 을넣어서 바로실행됨 
    }

    void DealDamage()
    {
        for(int i = 0; i < thingsToDamage.Count; i++)
        {
            thingsToDamage[i].TakePhysicalDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            thingsToDamage.Add(damagable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            thingsToDamage.Remove(damagable);
        }
    }
}
