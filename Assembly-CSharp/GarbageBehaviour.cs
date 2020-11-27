using System;
using UnityEngine;

// Token: 0x0200020C RID: 524
public class GarbageBehaviour : MonoBehaviour
{
	// Token: 0x06000B58 RID: 2904 RVA: 0x00008D6E File Offset: 0x00006F6E
	public void FixedUpdate()
	{
		if (base.transform.localPosition.y < -3.49f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
