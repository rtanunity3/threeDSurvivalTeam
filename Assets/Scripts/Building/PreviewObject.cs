using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    private List<Collider> colliderList = new List<Collider>(); // 충돌한 오브젝트들 저장할 리스트

    [SerializeField]
    private LayerMask layerMaskToIgnore; // 무시할 레이어 마스크

    [SerializeField]
    private Material green;
    [SerializeField]
    private Material red;


    void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (colliderList.Count > 0)
            SetColor(red);
        else
            SetColor(green);
    }

    private void SetColor(Material mat)
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            var newMaterials = new Material[renderer.materials.Length];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = mat;
            }

            renderer.materials = newMaterials;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 해당 레이어가 무시할 레이어 마스크에 포함되지 않는 경우, 리스트에 추가
        if (((1 << other.gameObject.layer) & layerMaskToIgnore) == 0)
        {
            colliderList.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 해당 레이어가 무시할 레이어 마스크에 포함되지 않는 경우, 리스트에서 제거
        if (((1 << other.gameObject.layer) & layerMaskToIgnore) == 0)
        {
            colliderList.Remove(other);
        }
    }

    public bool isBuildable()
    {
        return colliderList.Count == 0;
    }
}
