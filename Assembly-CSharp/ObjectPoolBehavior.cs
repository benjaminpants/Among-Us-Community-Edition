using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class ObjectPoolBehavior : IObjectPool
{
	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000176 RID: 374 RVA: 0x00002E13 File Offset: 0x00001013
	public override int InUse
	{
		get
		{
			return this.activeChildren.Count;
		}
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x06000177 RID: 375 RVA: 0x00002E20 File Offset: 0x00001020
	public override int NotInUse
	{
		get
		{
			return this.inactiveChildren.Count;
		}
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00002E2D File Offset: 0x0000102D
	public virtual void Awake()
	{
		if (this.AutoInit)
		{
			this.InitPool(this.Prefab);
		}
	}

	// Token: 0x06000179 RID: 377 RVA: 0x0000F360 File Offset: 0x0000D560
	public void InitPool(PoolableBehavior prefab)
	{
		for (int i = 0; i < this.poolSize; i++)
		{
			this.CreateOneInactive(prefab);
		}
	}

	// Token: 0x0600017A RID: 378 RVA: 0x0000F388 File Offset: 0x0000D588
	private void CreateOneInactive(PoolableBehavior prefab)
	{
		PoolableBehavior poolableBehavior = UnityEngine.Object.Instantiate<PoolableBehavior>(prefab);
		poolableBehavior.transform.SetParent(base.transform);
		poolableBehavior.gameObject.SetActive(false);
		poolableBehavior.OwnerPool = this;
		this.inactiveChildren.Add(poolableBehavior);
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0000F3CC File Offset: 0x0000D5CC
	public void ReclaimAll()
	{
		foreach (PoolableBehavior obj in this.activeChildren.ToArray())
		{
			this.Reclaim(obj);
		}
	}

	// Token: 0x0600017C RID: 380 RVA: 0x0000F400 File Offset: 0x0000D600
	public override T Get<T>()
	{
		List<PoolableBehavior> obj = this.inactiveChildren;
		PoolableBehavior poolableBehavior;
		lock (obj)
		{
			if (this.inactiveChildren.Count == 0)
			{
				if (this.activeChildren.Count == 0)
				{
					this.InitPool(this.Prefab);
				}
				else
				{
					this.CreateOneInactive(this.Prefab);
				}
			}
			poolableBehavior = this.inactiveChildren[this.inactiveChildren.Count - 1];
			this.inactiveChildren.RemoveAt(this.inactiveChildren.Count - 1);
			this.activeChildren.Add(poolableBehavior);
		}
		if (this.DetachOnGet)
		{
			poolableBehavior.transform.SetParent(null, false);
		}
		poolableBehavior.gameObject.SetActive(true);
		poolableBehavior.Reset();
		return poolableBehavior as T;
	}

	// Token: 0x0600017D RID: 381 RVA: 0x0000F4E0 File Offset: 0x0000D6E0
	public override void Reclaim(PoolableBehavior obj)
	{
		if (!this)
		{
			DefaultPool.Instance.Reclaim(obj);
			return;
		}
		obj.gameObject.SetActive(false);
		obj.transform.SetParent(base.transform);
		List<PoolableBehavior> obj2 = this.inactiveChildren;
		lock (obj2)
		{
			if (this.activeChildren.Remove(obj))
			{
				this.inactiveChildren.Add(obj);
			}
			else if (this.inactiveChildren.Contains(obj))
			{
				Debug.Log("ObjectPoolBehavior: :| Something was reclaimed without being gotten");
			}
			else
			{
				Debug.Log("ObjectPoolBehavior: Destroying this thing I don't own");
				UnityEngine.Object.Destroy(obj.gameObject);
			}
		}
	}

	// Token: 0x0400016E RID: 366
	public int poolSize = 20;

	// Token: 0x0400016F RID: 367
	[SerializeField]
	private List<PoolableBehavior> inactiveChildren = new List<PoolableBehavior>();

	// Token: 0x04000170 RID: 368
	[SerializeField]
	public List<PoolableBehavior> activeChildren = new List<PoolableBehavior>();

	// Token: 0x04000171 RID: 369
	public PoolableBehavior Prefab;

	// Token: 0x04000172 RID: 370
	public bool AutoInit;

	// Token: 0x04000173 RID: 371
	public bool DetachOnGet;
}
