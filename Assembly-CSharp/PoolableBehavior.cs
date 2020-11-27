using System;
using UnityEngine;

// Token: 0x02000045 RID: 69
public class PoolableBehavior : MonoBehaviour
{
	// Token: 0x06000182 RID: 386 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void Reset()
	{
	}

	// Token: 0x06000183 RID: 387 RVA: 0x00002E69 File Offset: 0x00001069
	public void Awake()
	{
		this.OwnerPool = DefaultPool.Instance;
	}

	// Token: 0x0400018C RID: 396
	[HideInInspector]
	public IObjectPool OwnerPool;
}
