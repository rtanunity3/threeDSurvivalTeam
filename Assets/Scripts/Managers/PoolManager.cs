using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Pool;

[System.Serializable]
public class ObjectInfo
{
    public string objectName;
    public GameObject prefab;
    public int defaultCapacity;
    public int maxSize;
}


public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    [SerializeField]
    private ObjectInfo[] projectile = null;
    [SerializeField]
    private ObjectInfo[] animals = null;

    [HideInInspector]
    public Queue<GameObject> TreePrefabQ = new Queue<GameObject>();
    private float repeatInterval = 10.0f;
    private float spawnRadius = 50f;

    private Dictionary<string, IObjectPool<PoolAble>> ojbectPoolDic = new Dictionary<string, IObjectPool<PoolAble>>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitObjectPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InvokeRepeating("RespawnTree", 0, repeatInterval);
    }

    private void InitObjectPool()
    {
        //Debug.LogWarning(objectInfos.Length);
        for (int idx = 0; idx < projectile.Length; idx++)
        {
            ObjectInfo tmpObjInfo = projectile[idx];
            IObjectPool<PoolAble> pool = new ObjectPool<PoolAble>(() =>
            CreatePooledItem(tmpObjInfo), OnGetFromPool
                , OnReleaseToPool, OnDestroyPoolObject
                , true, projectile[idx].defaultCapacity, projectile[idx].maxSize);

            if (ojbectPoolDic.ContainsKey(projectile[idx].objectName))
            {
                Debug.LogFormat("{0} 이미 등록된 오브젝트입니다.", projectile[idx].objectName);
                return;
            }

            ojbectPoolDic.Add(projectile[idx].objectName, pool);
        }

        for (int idx = 0; idx < animals.Length; idx++)
        {
            ObjectInfo tmpObjInfo = animals[idx];
            IObjectPool<PoolAble> pool = new ObjectPool<PoolAble>(() =>
            CreatePooledItem(tmpObjInfo), OnGetFromPool
                , OnReleaseToPool, OnDestroyPoolObject
                , true, animals[idx].defaultCapacity, animals[idx].maxSize);

            if (ojbectPoolDic.ContainsKey(animals[idx].objectName))
            {
                Debug.LogFormat("{0} 이미 등록된 오브젝트입니다.", animals[idx].objectName);
                return;
            }

            ojbectPoolDic.Add(animals[idx].objectName, pool);
        }
    }

    // 생성
    private PoolAble CreatePooledItem(ObjectInfo objectInfos)
    {
        PoolAble poolAble = Instantiate(objectInfos.prefab).GetComponent<PoolAble>();
        poolAble.SetPool(ojbectPoolDic[objectInfos.objectName]);
        return poolAble;
    }

    // 대여
    private void OnGetFromPool(PoolAble poolObj)
    {
        poolObj.gameObject.SetActive(true);
    }

    // 반환
    private void OnReleaseToPool(PoolAble poolObj)
    {
        poolObj.gameObject.SetActive(false);
    }

    // 삭제
    private void OnDestroyPoolObject(PoolAble poolObj)
    {
        Destroy(poolObj.gameObject);
    }

    public PoolAble GetPoolAble(string objectName)
    {
        if (ojbectPoolDic.ContainsKey(objectName) == false)
        {
            Debug.LogFormat("{0} 오브젝트풀에 등록되지 않은 오브젝트입니다.", objectName);
            return null;
        }

        return ojbectPoolDic[objectName].Get();
    }


    private void RespawnTree()
    {
        if (TreePrefabQ.Count > 0)
        {
            GameObject tree = TreePrefabQ.Dequeue();
            tree.gameObject.SetActive(true);
        }
    }

    public void RespawnAnimal(Vector3 centerPos)
    {

        int createNum = Random.Range(1, 4);

        for (int i = 0; i < createNum; ++i)
        {
            Vector3 randomPosition = (Random.onUnitSphere + centerPos) * spawnRadius;
            randomPosition.y = 100f;

            GameObject animal = animals[Random.Range(0, animals.Length)].prefab;

            Ray ray = new Ray(randomPosition, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                randomPosition.y = hit.point.y;
                Instantiate(animal, randomPosition, Quaternion.identity);
            }
        }
    }
}

