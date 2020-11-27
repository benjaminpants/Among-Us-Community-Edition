using System;

// Token: 0x0200021F RID: 543
public class WinningPlayerData
{
	// Token: 0x06000BC3 RID: 3011 RVA: 0x000023C4 File Offset: 0x000005C4
	public WinningPlayerData()
	{
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x00039DEC File Offset: 0x00037FEC
	public WinningPlayerData(GameData.PlayerInfo player)
	{
		this.IsYou = (player.Object == PlayerControl.LocalPlayer);
		this.Name = player.PlayerName;
		this.IsDead = (player.IsDead || player.Disconnected);
		this.ColorId = (int)player.ColorId;
		this.SkinId = player.SkinId;
		this.HatId = player.HatId;
	}

	// Token: 0x04000B49 RID: 2889
	public string Name;

	// Token: 0x04000B4A RID: 2890
	public bool IsDead;

	// Token: 0x04000B4B RID: 2891
	public int ColorId;

	// Token: 0x04000B4C RID: 2892
	public uint SkinId;

	// Token: 0x04000B4D RID: 2893
	public uint HatId;

	// Token: 0x04000B4E RID: 2894
	public bool IsYou;
}
