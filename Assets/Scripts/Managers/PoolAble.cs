using UnityEngine;
using UnityEngine.Pool;

public class PoolAble : MonoBehaviour
{
    private IObjectPool<PoolAble> _Pool;

    public void SetPool(IObjectPool<PoolAble> pool)
    {
        _Pool = pool;
    }

    public void ReleaseObject()
    {
        _Pool.Release(this);
    }
}
