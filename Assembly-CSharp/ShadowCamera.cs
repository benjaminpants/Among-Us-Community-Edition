using System;
using UnityEngine;

// Token: 0x020001C8 RID: 456
public class ShadowCamera : MonoBehaviour
{
	// Token: 0x060009E6 RID: 2534 RVA: 0x000080AE File Offset: 0x000062AE
	public void OnEnable()
	{
		base.GetComponent<Camera>().SetReplacementShader(this.Shadozer, "RenderType");
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x000080C6 File Offset: 0x000062C6
	public void OnDisable()
	{
		base.GetComponent<Camera>().ResetReplacementShader();
	}

	// Token: 0x04000989 RID: 2441
	public Shader Shadozer;
}
