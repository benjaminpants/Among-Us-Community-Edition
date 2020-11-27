using System;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class ParallaxController : MonoBehaviour
{
	// Token: 0x060007BF RID: 1983 RVA: 0x00006C73 File Offset: 0x00004E73
	public void Start()
	{
		this.cam = Camera.main;
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x0002C10C File Offset: 0x0002A30C
	private void Update()
	{
		Vector3 vector = base.transform.parent.position - this.cam.transform.position;
		vector *= this.Rate;
		vector.z = -this.Rate;
		base.transform.localPosition = vector;
	}

	// Token: 0x040007A0 RID: 1952
	public float Rate;

	// Token: 0x040007A1 RID: 1953
	private Camera cam;
}
