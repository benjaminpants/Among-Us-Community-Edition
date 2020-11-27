using System;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class MapRoom : MonoBehaviour
{
	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x060004B7 RID: 1207 RVA: 0x00004F34 File Offset: 0x00003134
	// (set) Token: 0x060004B8 RID: 1208 RVA: 0x00004F3C File Offset: 0x0000313C
	public InfectedOverlay Parent { get; set; }

	// Token: 0x060004B9 RID: 1209 RVA: 0x0001B6DC File Offset: 0x000198DC
	public void Start()
	{
		if (this.door)
		{
			if (PlayerControl.GameOptions.SabControl == 3 || PlayerControl.GameOptions.SabControl == 4)
			{
				UnityEngine.Object.Destroy(base.transform.gameObject);
			}
			this.door.SetCooldownNormalizedUvs();
		}
		if (this.special)
		{
			if (PlayerControl.GameOptions.SabControl == 3 || PlayerControl.GameOptions.SabControl == 4)
			{
				UnityEngine.Object.Destroy(base.transform.gameObject);
			}
			this.special.SetCooldownNormalizedUvs();
		}
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x0001B770 File Offset: 0x00019970
	public void OOBUpdate()
	{
		if (this.door && ShipStatus.Instance)
		{
			float timer = ((DoorsSystemType)ShipStatus.Instance.Systems[SystemTypes.Doors]).GetTimer(this.room);
			float value = this.Parent.CanUseDoors ? (timer / 30f) : 1f;
			this.door.material.SetFloat("_Percent", value);
		}
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x00004F45 File Offset: 0x00003145
	internal void SetSpecialActive(float perc)
	{
		if (this.special)
		{
			this.special.material.SetFloat("_Percent", perc);
		}
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0001B7EC File Offset: 0x000199EC
	public void SabotageReactor()
	{
		if (PlayerControl.GameOptions.SabControl == 2 || PlayerControl.GameOptions.SabControl == 3 || PlayerControl.GameOptions.SabControl == 4)
		{
			return;
		}
		if (!this.Parent.CanUseSpecial)
		{
			return;
		}
		ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 3);
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0001B83C File Offset: 0x00019A3C
	public void SabotageComms()
	{
		if (PlayerControl.GameOptions.SabControl == 2 || PlayerControl.GameOptions.SabControl == 3 || PlayerControl.GameOptions.SabControl == 4)
		{
			return;
		}
		if (!this.Parent.CanUseSpecial)
		{
			return;
		}
		ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 14);
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0001B890 File Offset: 0x00019A90
	public void SabotageOxygen()
	{
		if (PlayerControl.GameOptions.SabControl == 2 || PlayerControl.GameOptions.SabControl == 3 || PlayerControl.GameOptions.SabControl == 4)
		{
			return;
		}
		if (!this.Parent.CanUseSpecial)
		{
			return;
		}
		ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 8);
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0001B8E0 File Offset: 0x00019AE0
	public void SabotageLights()
	{
		if (PlayerControl.GameOptions.SabControl == 2 || PlayerControl.GameOptions.SabControl == 3 || PlayerControl.GameOptions.SabControl == 4)
		{
			return;
		}
		if (!this.Parent.CanUseSpecial)
		{
			return;
		}
		ShipStatus.Instance.RpcRepairSystem(SystemTypes.Sabotage, 7);
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0001B930 File Offset: 0x00019B30
	public void SabotageDoors()
	{
		if (PlayerControl.GameOptions.SabControl == 1 || PlayerControl.GameOptions.SabControl == 3 || PlayerControl.GameOptions.SabControl == 4)
		{
			return;
		}
		if (!this.Parent.CanUseDoors)
		{
			return;
		}
		if (((DoorsSystemType)ShipStatus.Instance.Systems[SystemTypes.Doors]).GetTimer(this.room) > 0f)
		{
			return;
		}
		ShipStatus.Instance.RpcCloseDoorsOfType(this.room);
	}

	// Token: 0x04000495 RID: 1173
	public SystemTypes room;

	// Token: 0x04000496 RID: 1174
	public SpriteRenderer door;

	// Token: 0x04000497 RID: 1175
	public SpriteRenderer special;
}
