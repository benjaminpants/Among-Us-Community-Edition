using System;
using UnityEngine;

// Token: 0x0200003D RID: 61
public abstract class IObjectPool : MonoBehaviour
{
	// Token: 0x06000165 RID: 357
	public abstract T Get<T>() where T : PoolableBehavior;

	// Token: 0x06000166 RID: 358
	public abstract void Reclaim(PoolableBehavior obj);

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x06000167 RID: 359
	public abstract int InUse { get; }

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x06000168 RID: 360
	public abstract int NotInUse { get; }
}
