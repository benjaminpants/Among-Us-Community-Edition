using System;
using UnityEngine;

// Token: 0x020001D8 RID: 472
public class MapConsole : MonoBehaviour, IUsable
{
	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06000A36 RID: 2614 RVA: 0x0000834F File Offset: 0x0000654F
	public float UsableDistance
	{
		get
		{
			return this.usableDistance;
		}
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x06000A37 RID: 2615 RVA: 0x0000640F File Offset: 0x0000460F
	public float PercentCool
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x00034F94 File Offset: 0x00033194
	public void SetOutline(bool on, bool mainTarget)
	{
		if (this.Image)
		{
			this.Image.material.SetFloat("_Outline", (float)(on ? 1 : 0));
			this.Image.material.SetColor("_OutlineColor", Color.white);
			this.Image.material.SetColor("_AddColor", mainTarget ? Color.white : Color.clear);
		}
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0003500C File Offset: 0x0003320C
	public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = pc.Object.CanMove;
		canUse = couldUse;
		if (canUse)
		{
			num = Vector2.Distance(@object.GetTruePosition(), base.transform.position);
			canUse &= (num <= this.UsableDistance);
		}
		return num;
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0003506C File Offset: 0x0003326C
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
		DestroyableSingleton<HudManager>.Instance.ShowMap(delegate(MapBehaviour m)
		{
			m.ShowCountOverlay();
		});
	}

	// Token: 0x040009CD RID: 2509
	public float usableDistance = 1f;

	// Token: 0x040009CE RID: 2510
	public SpriteRenderer Image;
}
