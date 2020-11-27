using System;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class OneWayShadows : MonoBehaviour
{
	// Token: 0x060007BB RID: 1979 RVA: 0x00006C30 File Offset: 0x00004E30
	public void Start()
	{
		LightSource.OneWayShadows.Add(base.gameObject, this);
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x00006C43 File Offset: 0x00004E43
	public void OnDestroy()
	{
		LightSource.OneWayShadows.Remove(base.gameObject);
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x00006C56 File Offset: 0x00004E56
	public bool IsIgnored(LightSource lightSource)
	{
		return this.RoomCollider.OverlapPoint(lightSource.transform.position);
	}

	// Token: 0x0400079F RID: 1951
	public Collider2D RoomCollider;
}
