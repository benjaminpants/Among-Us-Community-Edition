using System;
using UnityEngine;

// Token: 0x0200001B RID: 27
public static class Constants
{
	// Token: 0x06000075 RID: 117 RVA: 0x000025CF File Offset: 0x000007CF
	internal static int GetBroadcastVersion()
	{
		return 50483450;
	}

	// Token: 0x06000076 RID: 118 RVA: 0x000025D6 File Offset: 0x000007D6
	internal static int GetVersion(int year, int month, int day, int rev)
	{
		return year * 25000 + month * 1800 + day * 50 + rev;
	}

	// Token: 0x06000077 RID: 119 RVA: 0x000025EE File Offset: 0x000007EE
	internal static byte[] GetBroadcastVersionBytes()
	{
		return BitConverter.GetBytes(Constants.GetBroadcastVersion());
	}

	// Token: 0x06000078 RID: 120 RVA: 0x000025FA File Offset: 0x000007FA
	public static bool ShouldPlaySfx()
	{
		return !AmongUsClient.Instance || AmongUsClient.Instance.GameMode != GameModes.LocalGame || DetectHeadset.Detect();
	}

	// Token: 0x0400009F RID: 159
	public const string LocalNetAddress = "127.0.0.1";

	// Token: 0x040000A0 RID: 160
	public const int GamePlayPort = 22023;

	// Token: 0x040000A1 RID: 161
	public const int AnnouncementPort = 22024;

	// Token: 0x040000A2 RID: 162
	public const int ServersPort = 22025;

	// Token: 0x040000A3 RID: 163
	public const string InfinitySymbol = "∞";

	// Token: 0x040000A4 RID: 164
	public const int GhostLayer = 12;

	// Token: 0x040000A5 RID: 165
	public static readonly int ShipOnlyMask = LayerMask.GetMask(new string[]
	{
		"Ship"
	});

	// Token: 0x040000A6 RID: 166
	public static readonly int ShipAndObjectsMask = LayerMask.GetMask(new string[]
	{
		"Ship",
		"Objects"
	});

	// Token: 0x040000A7 RID: 167
	public static readonly int ShipAndAllObjectsMask = LayerMask.GetMask(new string[]
	{
		"Ship",
		"Objects",
		"ShortObjects"
	});

	// Token: 0x040000A8 RID: 168
	public static readonly int NotShipMask = ~LayerMask.GetMask(new string[]
	{
		"Ship"
	});

	// Token: 0x040000A9 RID: 169
	public static readonly int Usables = ~LayerMask.GetMask(new string[]
	{
		"Ship",
		"UI"
	});

	// Token: 0x040000AA RID: 170
	public static readonly int PlayersOnlyMask = LayerMask.GetMask(new string[]
	{
		"Players",
		"Ghost"
	});

	// Token: 0x040000AB RID: 171
	public static readonly int ShadowMask = LayerMask.GetMask(new string[]
	{
		"Shadow",
		"Objects",
		"IlluminatedBlocking"
	});

	// Token: 0x040000AC RID: 172
	public static readonly int[] CompatVersions = new int[]
	{
		Constants.GetBroadcastVersion(),
		Constants.GetVersion(2019, 4, 15, 0),
		Constants.GetVersion(2019, 4, 24, 1)
	};

	// Token: 0x040000AD RID: 173
	public const int Year = 2019;

	// Token: 0x040000AE RID: 174
	public const int Month = 4;

	// Token: 0x040000AF RID: 175
	public const int Day = 25;

	// Token: 0x040000B0 RID: 176
	public const int Revision = 0;
}
