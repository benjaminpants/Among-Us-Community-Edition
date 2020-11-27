using System;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class CreateStoreButton : MonoBehaviour
{
	// Token: 0x06000229 RID: 553 RVA: 0x00012288 File Offset: 0x00010488
	public void Click()
	{
		StoreMenu storeMenu = UnityEngine.Object.Instantiate<StoreMenu>(this.StorePrefab, this.Target);
		storeMenu.transform.localPosition = new Vector3(0f, 0f, -100f);
		storeMenu.transform.localScale = Vector3.zero;
	}

	// Token: 0x0400020A RID: 522
	public Transform Target;

	// Token: 0x0400020B RID: 523
	public StoreMenu StorePrefab;
}
