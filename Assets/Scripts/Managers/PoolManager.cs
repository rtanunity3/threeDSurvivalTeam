using System.Collections.Generic;
using UnityEngine;
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
    private ObjectInfo[] objectInfos = null;
    private ObjectInfo[] Animals = null;

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
        for (int idx = 0; idx < objectInfos.Length; idx++)
        {
            ObjectInfo tmpObjInfo = objectInfos[idx];
            IObjectPool<PoolAble> pool = new ObjectPool<PoolAble>(() =>
            CreatePooledItem(tmpObjInfo), OnGetFromPool
                , OnReleaseToPool, OnDestroyPoolObject
                , true, objectInfos[idx].defaultCapacity, objectInfos[idx].maxSize);

            if (ojbectPoolDic.ContainsKey(objectInfos[idx].objectName))
            {
                Debug.LogFormat("{0} 이미 등록된 오브젝트입니다.", objectInfos[idx].objectName);
                return;
            }

            ojbectPoolDic.Add(objectInfos[idx].objectName, pool);
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

        //Vector3 randomPosition = Random.onUnitSphere * spawnRadius;
        //randomPosition.y = 100f;

        //Ray ray = new Ray(randomPosition, Vector3.down);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        //{
        //    randomPosition.y = hit.point.y;
        //    Instantiate(myPrefab, randomPosition, Quaternion.identity);
        //}
    }
}

