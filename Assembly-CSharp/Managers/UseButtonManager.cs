using System;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class UseButtonManager : MonoBehaviour
{
	// Token: 0x060005C6 RID: 1478 RVA: 0x00024418 File Offset: 0x00022618
	public void SetTarget(IUsable target)
	{
		this.currentTarget = target;
		if (target != null)
		{
			if (target is Vent)
			{
				this.UseButton.sprite = this.VentImage;
			}
			else if (target is MapConsole)
			{
				this.UseButton.sprite = this.AdminMapImage;
			}
			else if (target is OptionsConsole)
			{
				this.UseButton.sprite = this.OptionsImage;
			}
			else if (target is SystemConsole)
			{
				SystemConsole systemConsole = (SystemConsole)target;
				if (systemConsole.name.StartsWith("Surv"))
				{
					this.UseButton.sprite = this.SecurityImage;
				}
				else if (systemConsole.name.StartsWith("TaskAdd"))
				{
					this.UseButton.sprite = this.OptionsImage;
				}
				else
				{
					this.UseButton.sprite = this.UseImage;
				}
			}
			else
			{
				this.UseButton.sprite = this.UseImage;
			}
			this.UseButton.SetCooldownNormalizedUvs();
			this.UseButton.material.SetFloat("_Percent", target.PercentCool);
			this.UseButton.color = UseButtonManager.EnabledColor;
			return;
		}
		if (PlayerControl.LocalPlayer.Data.IsImpostor && PlayerControl.LocalPlayer.CanMove)
		{
			this.UseButton.sprite = this.SabotageImage;
			this.UseButton.SetCooldownNormalizedUvs();
			this.UseButton.color = UseButtonManager.EnabledColor;
			return;
		}
		this.UseButton.sprite = this.UseImage;
		this.UseButton.color = UseButtonManager.DisabledColor;
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x000245A8 File Offset: 0x000227A8
	public void DoClick()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (!PlayerControl.LocalPlayer)
		{
			return;
		}
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		if (this.currentTarget != null)
		{
			PlayerControl.LocalPlayer.UseClosest();
			return;
		}
		if (data != null && data.IsImpostor)
		{
			DestroyableSingleton<HudManager>.Instance.ShowMap(delegate(MapBehaviour m)
			{
				m.ShowInfectedMap();
			});
		}
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x00005A64 File Offset: 0x00003C64
	internal void Refresh()
	{
		this.SetTarget(this.currentTarget);
	}

	// Token: 0x040005A1 RID: 1441
	private static readonly Color DisabledColor = new Color(1f, 1f, 1f, 0.3f);

	// Token: 0x040005A2 RID: 1442
	private static readonly Color EnabledColor = new Color(1f, 1f, 1f, 1f);

	// Token: 0x040005A3 RID: 1443
	public SpriteRenderer UseButton;

	// Token: 0x040005A4 RID: 1444
	public Sprite UseImage;

	// Token: 0x040005A5 RID: 1445
	public Sprite SabotageImage;

	// Token: 0x040005A6 RID: 1446
	public Sprite VentImage;

	// Token: 0x040005A7 RID: 1447
	public Sprite AdminMapImage;

	// Token: 0x040005A8 RID: 1448
	public Sprite SecurityImage;

	// Token: 0x040005A9 RID: 1449
	public Sprite OptionsImage;

	// Token: 0x040005AA RID: 1450
	private IUsable currentTarget;
}
