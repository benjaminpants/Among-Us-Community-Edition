using System;
using UnityEngine;

// Token: 0x02000149 RID: 329
public class OptionsConsole : MonoBehaviour, IUsable
{
	// Token: 0x1700010A RID: 266
	// (get) Token: 0x060006E0 RID: 1760 RVA: 0x00006408 File Offset: 0x00004608
	public float UsableDistance
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x060006E1 RID: 1761 RVA: 0x0000640F File Offset: 0x0000460F
	public float PercentCool
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x0002861C File Offset: 0x0002681C
	public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = @object.CanMove;
		canUse = couldUse;
		if (canUse)
		{
			num = Vector2.Distance(@object.GetTruePosition(), base.transform.position);
			canUse &= (num <= this.UsableDistance);
		}
		return num;
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00028674 File Offset: 0x00026874
	public void SetOutline(bool on, bool mainTarget)
	{
		if (this.Outline)
		{
			this.Outline.material.SetFloat("_Outline", (float)(on ? 1 : 0));
			this.Outline.material.SetColor("_OutlineColor", Color.white);
			this.Outline.material.SetColor("_AddColor", mainTarget ? Color.white : Color.clear);
		}
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x000286EC File Offset: 0x000268EC
	public void Use()
	{
		bool flag;
		bool flag2;
		this.CanUse(PlayerControl.LocalPlayer.Data, out flag, out flag2);
		if (!flag)
		{
			return;
		}
		PlayerControl.LocalPlayer.NetTransform.Halt();
		CustomPlayerMenu customPlayerMenu = UnityEngine.Object.Instantiate<CustomPlayerMenu>(this.MenuPrefab);
		customPlayerMenu.transform.SetParent(Camera.main.transform, false);
		customPlayerMenu.transform.localPosition = new Vector3(0f, 0f, -20f);
	}

	// Token: 0x040006A6 RID: 1702
	public CustomPlayerMenu MenuPrefab;

	// Token: 0x040006A7 RID: 1703
	public SpriteRenderer Outline;
}
