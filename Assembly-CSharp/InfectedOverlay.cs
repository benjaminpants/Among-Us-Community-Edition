using System;
using UnityEngine;

// Token: 0x020000DE RID: 222
public class InfectedOverlay : MonoBehaviour
{
	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x060004A8 RID: 1192 RVA: 0x00004E9C File Offset: 0x0000309C
	public bool CanUseDoors
	{
		get
		{
			return !this.SabSystem.AnyActive;
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x060004A9 RID: 1193 RVA: 0x00004EAC File Offset: 0x000030AC
	public bool CanUseSpecial
	{
		get
		{
			return this.SabSystem.Timer <= 0f && !this.doors.IsActive;
		}
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x0001B3F8 File Offset: 0x000195F8
	public void Start()
	{
		for (int i = 0; i < this.rooms.Length; i++)
		{
			this.rooms[i].Parent = this;
		}
		this.SabSystem = (SabotageSystemType)ShipStatus.Instance.Systems[SystemTypes.Sabotage];
		this.doors = (IActivatable)ShipStatus.Instance.Systems[SystemTypes.Doors];
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x0001B460 File Offset: 0x00019660
	private void FixedUpdate()
	{
		if (this.doors == null)
		{
			return;
		}
		float specialActive = this.doors.IsActive ? 1f : this.SabSystem.PercentCool;
		for (int i = 0; i < this.rooms.Length; i++)
		{
			this.rooms[i].SetSpecialActive(specialActive);
			this.rooms[i].OOBUpdate();
		}
	}

	// Token: 0x0400048B RID: 1163
	public MapRoom[] rooms;

	// Token: 0x0400048C RID: 1164
	private IActivatable doors;

	// Token: 0x0400048D RID: 1165
	private SabotageSystemType SabSystem;
}
