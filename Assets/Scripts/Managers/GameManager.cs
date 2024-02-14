using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject playerObject;

    public int UIDepth;

    private void Awake()
    {
        Instance = this; 
    }
}
