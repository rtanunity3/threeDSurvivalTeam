using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class Resource : MonoBehaviour
{
    private int initCapacity = 0;

    public ItemData itemToGive;
    public int quantityPerHit = 1;
    public int capacity;

    private void Awake()
    {
        initCapacity = capacity;
    }

    private void OnEnable()
    {
        capacity = initCapacity;
    }

    public void Gather(Vector3 hitPoint, Vector3 hitNormal)
    {
        for (int i = 0; i < quantityPerHit; i++)
        {
            if (capacity <= 0) { break; }
            capacity -= 1; ;
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
        }

        if (capacity <= 0)
        {
            PoolManager.instance.TreePrefabQ.Enqueue(gameObject);
            gameObject.SetActive(false);
        }


    }
}
