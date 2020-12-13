using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolBehavior : IObjectPool
{
	public int poolSize = 20;

	[SerializeField]
	private List<PoolableBehavior> inactiveChildren = new List<PoolableBehavior>();

	[SerializeField]
	public List<PoolableBehavior> activeChildren = new List<PoolableBehavior>();

	public PoolableBehavior Prefab;

	public bool AutoInit;

	public bool DetachOnGet;

	public override int InUse => activeChildren.Count;

	public override int NotInUse => inactiveChildren.Count;

	public virtual void Awake()
	{
		if (AutoInit)
		{
			InitPool(Prefab);
		}
	}

	public void InitPool(PoolableBehavior prefab)
	{
		for (int i = 0; i < poolSize; i++)
		{
			CreateOneInactive(prefab);
		}
	}

	private void CreateOneInactive(PoolableBehavior prefab)
	{
		PoolableBehavior poolableBehavior = Object.Instantiate(prefab);
		poolableBehavior.transform.SetParent(base.transform);
		poolableBehavior.gameObject.SetActive(value: false);
		poolableBehavior.OwnerPool = this;
		inactiveChildren.Add(poolableBehavior);
	}

	public void ReclaimAll()
	{
		PoolableBehavior[] array = activeChildren.ToArray();
		foreach (PoolableBehavior obj in array)
		{
			Reclaim(obj);
		}
	}

	public override T Get<T>()
	{
		PoolableBehavior poolableBehavior;
		lock (inactiveChildren)
		{
			if (inactiveChildren.Count == 0)
			{
				if (activeChildren.Count == 0)
				{
					InitPool(Prefab);
				}
				else
				{
					CreateOneInactive(Prefab);
				}
			}
			poolableBehavior = inactiveChildren[inactiveChildren.Count - 1];
			inactiveChildren.RemoveAt(inactiveChildren.Count - 1);
			activeChildren.Add(poolableBehavior);
		}
		if (DetachOnGet)
		{
			poolableBehavior.transform.SetParent(null, worldPositionStays: false);
		}
		poolableBehavior.gameObject.SetActive(value: true);
		poolableBehavior.Reset();
		return poolableBehavior as T;
	}

	public override void Reclaim(PoolableBehavior obj)
	{
		if (!this)
		{
			DefaultPool.Instance.Reclaim(obj);
			return;
		}
		obj.gameObject.SetActive(value: false);
		obj.transform.SetParent(base.transform);
		lock (inactiveChildren)
		{
			if (activeChildren.Remove(obj))
			{
				inactiveChildren.Add(obj);
				return;
			}
			if (inactiveChildren.Contains(obj))
			{
				Debug.Log("ObjectPoolBehavior: :| Something was reclaimed without being gotten");
				return;
			}
			Debug.Log("ObjectPoolBehavior: Destroying this thing I don't own");
			Object.Destroy(obj.gameObject);
		}
	}
}
