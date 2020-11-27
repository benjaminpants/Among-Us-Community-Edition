using System;
using InnerNet;

// Token: 0x020000F0 RID: 240
public class MeetingRoomManager : IDisconnectHandler
{
	// Token: 0x06000515 RID: 1301 RVA: 0x00005183 File Offset: 0x00003383
	public void AssignSelf(PlayerControl reporter, GameData.PlayerInfo target)
	{
		this.reporter = reporter;
		this.target = target;
		AmongUsClient.Instance.DisconnectHandlers.AddUnique(this);
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x000051A3 File Offset: 0x000033A3
	public void RemoveSelf()
	{
		AmongUsClient.Instance.DisconnectHandlers.Remove(this);
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x000051B6 File Offset: 0x000033B6
	public void HandleDisconnect(PlayerControl pc, DisconnectReasons reason)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			this.reporter.CmdReportDeadBody(this.target);
		}
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x000051D5 File Offset: 0x000033D5
	public void HandleDisconnect()
	{
		this.HandleDisconnect(null, DisconnectReasons.ExitGame);
	}

	// Token: 0x040004E3 RID: 1251
	public static readonly MeetingRoomManager Instance = new MeetingRoomManager();

	// Token: 0x040004E4 RID: 1252
	private PlayerControl reporter;

	// Token: 0x040004E5 RID: 1253
	private GameData.PlayerInfo target;
}
