using System;
using System.Collections.Generic;

// Token: 0x02000222 RID: 546
public static class TempData
{
	// Token: 0x170001CA RID: 458
	// (get) Token: 0x06000BC5 RID: 3013 RVA: 0x00009126 File Offset: 0x00007326
	public static bool IsDo2Enabled
	{
		get
		{
			return SaveManager.PlayerName == "do2";
		}
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x00009137 File Offset: 0x00007337
	public static bool DidHumansWin(GameOverReason reason)
	{
		return reason == GameOverReason.HumansByTask || reason == GameOverReason.HumansByVote;
	}

	// Token: 0x04000B5A RID: 2906
	public static GameOverReason EndReason = GameOverReason.HumansByTask;

	// Token: 0x04000B5B RID: 2907
	public static bool showAd;

	// Token: 0x04000B5C RID: 2908
	public static bool playAgain;

	// Token: 0x04000B5D RID: 2909
	public static List<WinningPlayerData> winners = new List<WinningPlayerData>
	{
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 0,
			SkinId = 0U,
			IsDead = true
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 1,
			SkinId = 1U,
			IsDead = true
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 2,
			SkinId = 2U,
			IsDead = true
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 3,
			SkinId = 0U
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 4,
			SkinId = 1U
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 5,
			SkinId = 2U
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 6
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 7
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		},
		new WinningPlayerData
		{
			Name = "WWWWWWWWWW",
			ColorId = 8
		}
	};
}
