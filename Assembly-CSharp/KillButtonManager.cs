using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class KillButtonManager : MonoBehaviour
{
	// Token: 0x0600045E RID: 1118 RVA: 0x00004CA2 File Offset: 0x00002EA2
	public void Start()
	{
		this.renderer.SetCooldownNormalizedUvs();
		this.SetTarget(null);
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x0001AC70 File Offset: 0x00018E70
	public void PerformKill()
	{
		if (base.isActiveAndEnabled && this.CurrentTarget && !this.isCoolingDown && !PlayerControl.LocalPlayer.Data.IsDead)
		{
			PlayerControl.LocalPlayer.RpcMurderPlayer(this.CurrentTarget);
			if (PlayerControl.LocalPlayer.Data.role == GameData.PlayerInfo.Role.Sheriff)
			{
				PlayerControl.LocalPlayer.nameText.Color = Palette.White;
				List<GameData.PlayerInfo> list = (from pcd in GameData.Instance.AllPlayers
				where !pcd.Disconnected
				select pcd into pc
				where !pc.IsDead
				select pc into pci
				where !pci.IsImpostor
				select pci into pcs
				where pcs != PlayerControl.LocalPlayer.Data
				select pcs).ToList<GameData.PlayerInfo>();
				list.Shuffle<GameData.PlayerInfo>();
				GameData.PlayerInfo.Role[] roles = new GameData.PlayerInfo.Role[]
				{
					GameData.PlayerInfo.Role.None,
					GameData.PlayerInfo.Role.Sheriff
				};
				PlayerControl.LocalPlayer.RpcSetRole(new GameData.PlayerInfo[]
				{
					PlayerControl.LocalPlayer.Data,
					list.Take(1).ToArray<GameData.PlayerInfo>()[0]
				}, roles);
				base.gameObject.SetActive(false);
			}
			this.SetTarget(null);
		}
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0001ADE4 File Offset: 0x00018FE4
	public void SetTarget(PlayerControl target)
	{
		if (this.CurrentTarget && this.CurrentTarget != target)
		{
			this.CurrentTarget.GetComponent<SpriteRenderer>().material.SetFloat("_Outline", 0f);
		}
		this.CurrentTarget = target;
		if (this.CurrentTarget)
		{
			SpriteRenderer component = this.CurrentTarget.GetComponent<SpriteRenderer>();
			component.material.SetFloat("_Outline", (float)(this.isActive ? 1 : 0));
			component.material.SetColor("_OutlineColor", (PlayerControl.LocalPlayer.Data.role == GameData.PlayerInfo.Role.Sheriff) ? Palette.SheriffYellow : Color.red);
			this.renderer.color = Palette.EnabledColor;
			this.renderer.material.SetFloat("_Desat", 0f);
			return;
		}
		this.renderer.color = Palette.DisabledColor;
		this.renderer.material.SetFloat("_Desat", 1f);
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x0001AEEC File Offset: 0x000190EC
	public void SetCoolDown(float timer, float maxTimer)
	{
		float num = Mathf.Clamp(timer / maxTimer, 0f, 1f);
		if (this.renderer)
		{
			this.renderer.material.SetFloat("_Percent", num);
		}
		this.isCoolingDown = (num > 0f);
		if (this.isCoolingDown)
		{
			this.TimerText.Text = Mathf.CeilToInt(timer).ToString();
			this.TimerText.gameObject.SetActive(true);
			return;
		}
		this.TimerText.gameObject.SetActive(false);
	}

	// Token: 0x0400044A RID: 1098
	public PlayerControl CurrentTarget;

	// Token: 0x0400044B RID: 1099
	public SpriteRenderer renderer;

	// Token: 0x0400044C RID: 1100
	public TextRenderer TimerText;

	// Token: 0x0400044D RID: 1101
	public bool isCoolingDown = true;

	// Token: 0x0400044E RID: 1102
	public bool isActive;

	// Token: 0x0400044F RID: 1103
	private Vector2 uv;
}
