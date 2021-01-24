using System;
using UnityEngine;

public static class Constants
{
	public const string LocalNetAddress = "127.0.0.1";

	public const int GamePlayPort = 22023;

	public const int AnnouncementPort = 22024;

	public const int ServersPort = 25565;

	public const string InfinitySymbol = "∞";

	public const int GhostLayer = 12;

	public static readonly int ShipOnlyMask;

	public static readonly int ShipAndObjectsMask;

	public static readonly int ShipAndAllObjectsMask;

	public static readonly int NotShipMask;

	public static readonly int Usables;

	public static readonly int PlayersOnlyMask;

	public static readonly int ShadowMask;

	public static readonly int[] CompatVersions;

	public const int Year = 4200;

	public const int Month = 69;

	public const int Day = 69;

	public const int Revision = 0;

	internal static int GetBroadcastVersion()
	{
		return 50483450;
	}

	internal static int GetVersion(int year, int month, int day, int rev)
	{
		return year * 25000 + month * 1800 + day * 50 + rev;
	}

	internal static byte[] GetBroadcastVersionBytes()
	{
		return BitConverter.GetBytes(GetBroadcastVersion());
	}

	public static bool ShouldPlaySfx()
	{
		if ((bool)AmongUsClient.Instance && AmongUsClient.Instance.GameMode == GameModes.LocalGame)
		{
			return DetectHeadset.Detect();
		}
		return true;
	}

	static Constants()
	{
		ShipOnlyMask = LayerMask.GetMask("Ship");
		ShipAndObjectsMask = LayerMask.GetMask("Ship", "Objects");
		ShipAndAllObjectsMask = LayerMask.GetMask("Ship", "Objects", "ShortObjects");
		NotShipMask = ~LayerMask.GetMask("Ship");
		Usables = ~LayerMask.GetMask("Ship", "UI");
		PlayersOnlyMask = LayerMask.GetMask("Players", "Ghost");
		ShadowMask = LayerMask.GetMask("Shadow", "Objects", "IlluminatedBlocking");
		CompatVersions = new int[1]
		{
			GetBroadcastVersion()
		};
	}
}
