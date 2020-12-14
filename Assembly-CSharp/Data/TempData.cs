using System.Collections.Generic;

public static class TempData
{
	public static GameOverReason EndReason;

	public static bool showAd;

	public static string CustomStinger;

	public static bool playAgain;

	public static List<WinningPlayerData> winners;

	public static bool IsDo2Enabled => SaveManager.PlayerName == "do2";

	public static bool DidHumansWin(GameOverReason reason)
	{
		if (reason != GameOverReason.HumansByTask)
		{
			return reason == GameOverReason.HumansByVote;
		}
		return true;
	}

	static TempData()
	{
		EndReason = GameOverReason.HumansByTask;
		winners = new List<WinningPlayerData>
		{
			new WinningPlayerData
			{
				Name = "WWWWWWWWWW",
				ColorId = 0,
				SkinId = 0u,
				IsDead = true
			},
			new WinningPlayerData
			{
				Name = "WWWWWWWWWW",
				ColorId = 1,
				SkinId = 1u,
				IsDead = true
			},
			new WinningPlayerData
			{
				Name = "WWWWWWWWWW",
				ColorId = 2,
				SkinId = 2u,
				IsDead = true
			},
			new WinningPlayerData
			{
				Name = "WWWWWWWWWW",
				ColorId = 3,
				SkinId = 0u
			},
			new WinningPlayerData
			{
				Name = "WWWWWWWWWW",
				ColorId = 4,
				SkinId = 1u
			},
			new WinningPlayerData
			{
				Name = "WWWWWWWWWW",
				ColorId = 5,
				SkinId = 2u
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

	public static bool DidJokerWin(GameOverReason reason)
	{
		return reason == GameOverReason.JokerEjected;
	}
}
