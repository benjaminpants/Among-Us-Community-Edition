using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class SortGameObject : MonoBehaviour
{
	// Token: 0x040005EC RID: 1516
	public SortGameObject.ObjType MyType;

	// Token: 0x040005ED RID: 1517
	public Collider2D Collider;

	// Token: 0x0200011F RID: 287
	public enum ObjType
	{
		// Token: 0x040005EF RID: 1519
		Plant,
		// Token: 0x040005F0 RID: 1520
		Mineral,
		// Token: 0x040005F1 RID: 1521
		Animal
	}
}
