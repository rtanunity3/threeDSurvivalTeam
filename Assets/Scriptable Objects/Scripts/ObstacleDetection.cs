using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleDetection : MonoBehaviour
{
    //장애물 감지 거리
    [Range(0f, 10f)] [SerializeField] private float maxDistance = 10f;
    [Range(0f, 2f)][SerializeField] private float rayPosY = 1f;

    AnimalController animalController;


    bool startDetect = false;

    private void Awake()
    {
        animalController = GetComponent<AnimalController>();
    }

    private void Start()
    {
        startDetect = true;
    }

    void OnDrawGizmos()
    {
        RaycastHit hit;
        Vector3 newTransformPosition = new Vector3(transform.position.x, transform.position.y + rayPosY, transform.position.z);

        bool isHit = Physics.BoxCast(newTransformPosition, transform.localScale / 2, transform.forward, out hit, transform.rotation, maxDistance);

        //장애물 검출 시
        if (isHit && startDetect)
        {
            if (hit.collider.CompareTag("Animal") || hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Ground"))
            {
                animalController.OnReSetDestination();
            }

            Gizmos.color = Color.red;
            Gizmos.DrawRay(newTransformPosition, transform.forward * hit.distance);
            Gizmos.DrawWireCube(newTransformPosition + transform.forward * hit.distance, transform.localScale);
        }
        //장애물이 검출되지 않았을 경우
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(newTransformPosition, transform.forward * maxDistance);
        }
    }
}
