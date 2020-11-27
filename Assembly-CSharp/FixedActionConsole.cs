using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D4 RID: 468
public class FixedActionConsole : MonoBehaviour, IUsable
{
	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06000A23 RID: 2595 RVA: 0x000082C7 File Offset: 0x000064C7
	public float UsableDistance
	{
		get
		{
			return this.usableDistance;
		}
	}

	// Token: 0x17000186 RID: 390
	// (get) Token: 0x06000A24 RID: 2596 RVA: 0x0000640F File Offset: 0x0000460F
	public float PercentCool
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x00034E7C File Offset: 0x0003307C
	public void SetOutline(bool on, bool mainTarget)
	{
		if (this.Image)
		{
			this.Image.material.SetFloat("_Outline", (float)(on ? 1 : 0));
			this.Image.material.SetColor("_OutlineColor", Color.white);
			this.Image.material.SetColor("_AddColor", mainTarget ? Color.white : Color.clear);
		}
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x00034EF4 File Offset: 0x000330F4
	public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = (pc.Object.CanMove && !pc.IsDead);
		canUse = couldUse;
		if (canUse)
		{
			num = Vector2.Distance(@object.GetTruePosition(), base.transform.position);
			canUse &= (num <= this.UsableDistance);
		}
		return num;
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x00034F60 File Offset: 0x00033160
	public void Use()
	{
		bool flag;
		bool flag2;
		this.CanUse(PlayerControl.LocalPlayer.Data, out flag, out flag2);
		if (!flag)
		{
			return;
		}
		this.OnUse.Invoke();
	}

	// Token: 0x040009C7 RID: 2503
	public float usableDistance = 1f;

	// Token: 0x040009C8 RID: 2504
	public SpriteRenderer Image;

	// Token: 0x040009C9 RID: 2505
	public Button.ButtonClickedEvent OnUse;
}
