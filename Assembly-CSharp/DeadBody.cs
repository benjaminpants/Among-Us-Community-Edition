using System;
using UnityEngine;

// Token: 0x02000179 RID: 377
public class DeadBody : MonoBehaviour
{
	// Token: 0x1700012C RID: 300
	// (get) Token: 0x060007C4 RID: 1988 RVA: 0x00006C80 File Offset: 0x00004E80
	public Vector2 TruePosition
	{
		get
		{
			return (Vector2)base.transform.position + this.myCollider.offset;
		}
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x0002C1C0 File Offset: 0x0002A3C0
	public void OnClick()
	{
		if (this.Reported)
		{
			return;
		}
		if (!PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.GetTruePosition(), this.TruePosition, Constants.ShipAndObjectsMask, false))
		{
			this.Reported = true;
			GameData.PlayerInfo target = PlayerControl.LocalPlayer.Data;
			if (PlayerControl.GameOptions.Gamemode != 1)
			{
				target = GameData.Instance.GetPlayerById(this.ParentId);
			}
			PlayerControl.LocalPlayer.CmdReportDeadBody(target);
		}
	}

	// Token: 0x040007A3 RID: 1955
	public bool Reported;

	// Token: 0x040007A4 RID: 1956
	public short KillIdx;

	// Token: 0x040007A5 RID: 1957
	public byte ParentId;

	// Token: 0x040007A6 RID: 1958
	public Collider2D myCollider;
}
