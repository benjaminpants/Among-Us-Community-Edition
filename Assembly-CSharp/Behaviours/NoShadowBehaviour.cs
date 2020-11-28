using System;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class NoShadowBehaviour : MonoBehaviour
{
	// Token: 0x060007B6 RID: 1974 RVA: 0x00006BCF File Offset: 0x00004DCF
	public void Start()
	{
		LightSource.NoShadows.Add(base.gameObject, this);
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x00006BE2 File Offset: 0x00004DE2
	public void OnDestroy()
	{
		LightSource.NoShadows.Remove(base.gameObject);
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x0002C0A0 File Offset: 0x0002A2A0
	private void LateUpdate()
	{
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		if (data != null && !data.IsDead)
		{
			if (this.didHit)
			{
				this.didHit = false;
				ShipStatus instance = ShipStatus.Instance;
				if (instance && instance.CalculateLightRadius(data) > instance.MaxLightRadius / 3f)
				{
					this.SetMaskFunction(8);
					return;
				}
			}
			this.SetMaskFunction(1);
			return;
		}
		this.SetMaskFunction(8);
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x00006BF5 File Offset: 0x00004DF5
	private void SetMaskFunction(int func)
	{
		this.rend.material.SetInt("_Mask", func);
		if (this.shadowChild)
		{
			this.shadowChild.material.SetInt("_Mask", func);
		}
	}

	// Token: 0x0400079C RID: 1948
	public Renderer rend;

	// Token: 0x0400079D RID: 1949
	public bool didHit;

	// Token: 0x0400079E RID: 1950
	public Renderer shadowChild;
}
