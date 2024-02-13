using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetArrowPos : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = Camera.main.transform.position;
    }
}
