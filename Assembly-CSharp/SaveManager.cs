using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

// Token: 0x020001AF RID: 431
public static class SaveManager
{
	// Token: 0x1700015E RID: 350
	// (get) Token: 0x0600091F RID: 2335 RVA: 0x00030B28 File Offset: 0x0002ED28
	public static bool AmBanned
	{
		get
		{
			return (DateTime.UtcNow - SaveManager.LastGameStart).TotalMinutes < 5.0;
		}
	}

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000920 RID: 2336 RVA: 0x00030B58 File Offset: 0x0002ED58
	public static int BanMinutesLeft
	{
		get
		{
			float num = (float)(DateTime.UtcNow - SaveManager.LastGameStart).TotalMinutes;
			if (num > 6f)
			{
				return 0;
			}
			return Mathf.CeilToInt(5f - num);
		}
	}

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000921 RID: 2337 RVA: 0x0000785B File Offset: 0x00005A5B
	// (set) Token: 0x06000922 RID: 2338 RVA: 0x00007867 File Offset: 0x00005A67
	public static DateTime LastGameStart
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return SaveManager.lastGameStart;
		}
		set
		{
			SaveManager.lastGameStart = value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000923 RID: 2339 RVA: 0x00007874 File Offset: 0x00005A74
	// (set) Token: 0x06000924 RID: 2340 RVA: 0x00007880 File Offset: 0x00005A80
	public static Announcement LastAnnouncement
	{
		get
		{
			SaveManager.LoadAnnouncement();
			return SaveManager.lastAnnounce;
		}
		set
		{
			SaveManager.lastAnnounce = value;
			SaveManager.SaveAnnouncement();
		}
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000925 RID: 2341 RVA: 0x0000788D File Offset: 0x00005A8D
	// (set) Token: 0x06000926 RID: 2342 RVA: 0x00007899 File Offset: 0x00005A99
	public static DateTime LastStartDate
	{
		get
		{
			SaveManager.LoadSecureData2();
			return SaveManager.lastStartDate;
		}
		set
		{
			SaveManager.LoadSecureData2();
			if (SaveManager.lastStartDate < value)
			{
				SaveManager.lastStartDate = value;
				SaveManager.SaveSecureData2();
			}
		}
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000927 RID: 2343 RVA: 0x00030B94 File Offset: 0x0002ED94
	public static int Month
	{
		get
		{
			return SaveManager.LastStartDate.Month;
		}
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x000078B8 File Offset: 0x00005AB8
	private static void SaveSecureData2()
	{
		SaveManager.secure2.SaveData(new object[]
		{
			SaveManager.lastStartDate.Ticks
		});
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x00030BB0 File Offset: 0x0002EDB0
	private static void LoadSecureData2()
	{
		if (SaveManager.secure2.Loaded)
		{
			return;
		}
		try
		{
			using (BinaryReader binaryReader = SaveManager.secure2.LoadData())
			{
				SaveManager.lastStartDate = new DateTime(binaryReader.ReadInt64());
			}
		}
		catch
		{
		}
	}

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x0600092A RID: 2346 RVA: 0x000078DC File Offset: 0x00005ADC
	public static bool BoughtNoAds
	{
		get
		{
			SaveManager.LoadSecureData();
			return SaveManager.purchases.Contains("bought_ads");
		}
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x000078F2 File Offset: 0x00005AF2
	public static bool GetPurchase(string key)
	{
		SaveManager.LoadSecureData();
		return SaveManager.purchases.Contains(key);
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x00007904 File Offset: 0x00005B04
	public static void SetPurchased(string key)
	{
		SaveManager.LoadSecureData();
		SaveManager.purchases.Add(key);
		if (key == "bought_ads")
		{
			SaveManager.ShowAdsScreen = ShowAdsState.Purchased;
		}
		SaveManager.SaveSecureData();
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x00030C14 File Offset: 0x0002EE14
	private static void LoadSecureData()
	{
		if (SaveManager.purchaseFile.Loaded)
		{
			return;
		}
		try
		{
			using (BinaryReader binaryReader = SaveManager.purchaseFile.LoadData())
			{
				while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
				{
					SaveManager.purchases.Add(binaryReader.ReadString());
				}
			}
		}
		catch
		{
			Debug.Log("Deleted corrupt secure file");
			SaveManager.purchaseFile.Delete();
		}
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00007933 File Offset: 0x00005B33
	private static void SaveSecureData()
	{
		SaveManager.purchaseFile.SaveData(new object[]
		{
			SaveManager.purchases
		});
	}

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x0600092F RID: 2351 RVA: 0x0000794D File Offset: 0x00005B4D
	// (set) Token: 0x06000930 RID: 2352 RVA: 0x00007959 File Offset: 0x00005B59
	public static ShowAdsState ShowAdsScreen
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return (ShowAdsState)SaveManager.showAdsScreen;
		}
		set
		{
			SaveManager.showAdsScreen = (byte)value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000931 RID: 2353 RVA: 0x00007966 File Offset: 0x00005B66
	// (set) Token: 0x06000932 RID: 2354 RVA: 0x00007972 File Offset: 0x00005B72
	public static bool SendDataScreen
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return SaveManager.sendDataScreen;
		}
		set
		{
			SaveManager.sendDataScreen = value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000933 RID: 2355 RVA: 0x0000797F File Offset: 0x00005B7F
	// (set) Token: 0x06000934 RID: 2356 RVA: 0x00007994 File Offset: 0x00005B94
	public static bool SendName
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return SaveManager.sendName && SaveManager.SendTelemetry;
		}
		set
		{
			SaveManager.sendName = value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06000935 RID: 2357 RVA: 0x000079A1 File Offset: 0x00005BA1
	// (set) Token: 0x06000936 RID: 2358 RVA: 0x000079AD File Offset: 0x00005BAD
	public static bool SendTelemetry
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return SaveManager.sendTelemetry;
		}
		set
		{
			SaveManager.sendTelemetry = value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06000937 RID: 2359 RVA: 0x000079BA File Offset: 0x00005BBA
	// (set) Token: 0x06000938 RID: 2360 RVA: 0x000079C6 File Offset: 0x00005BC6
	public static bool ShowMinPlayerWarning
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return SaveManager.showMinPlayerWarning;
		}
		set
		{
			SaveManager.showMinPlayerWarning = value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06000939 RID: 2361 RVA: 0x000079D3 File Offset: 0x00005BD3
	// (set) Token: 0x0600093A RID: 2362 RVA: 0x000079DF File Offset: 0x00005BDF
	public static bool ShowOnlineHelp
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return SaveManager.showOnlineHelp;
		}
		set
		{
			SaveManager.showOnlineHelp = value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x0600093B RID: 2363 RVA: 0x000079EC File Offset: 0x00005BEC
	// (set) Token: 0x0600093C RID: 2364 RVA: 0x000079FF File Offset: 0x00005BFF
	public static float SfxVolume
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return (float)SaveManager.sfxVolume / 255f;
		}
		set
		{
			SaveManager.sfxVolume = (byte)(value * 255f);
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x0600093D RID: 2365 RVA: 0x00007A13 File Offset: 0x00005C13
	// (set) Token: 0x0600093E RID: 2366 RVA: 0x00007A26 File Offset: 0x00005C26
	public static float MusicVolume
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return (float)SaveManager.musicVolume / 255f;
		}
		set
		{
			SaveManager.musicVolume = (byte)(value * 255f);
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x0600093F RID: 2367 RVA: 0x00007A3A File Offset: 0x00005C3A
	// (set) Token: 0x06000940 RID: 2368 RVA: 0x00007A46 File Offset: 0x00005C46
	public static int TouchConfig
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return SaveManager.touchConfig;
		}
		set
		{
			SaveManager.touchConfig = value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06000941 RID: 2369 RVA: 0x00007A53 File Offset: 0x00005C53
	// (set) Token: 0x06000942 RID: 2370 RVA: 0x00007A5F File Offset: 0x00005C5F
	public static float JoystickSize
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return SaveManager.joyStickSize;
		}
		set
		{
			SaveManager.joyStickSize = value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000943 RID: 2371 RVA: 0x00007A6C File Offset: 0x00005C6C
	// (set) Token: 0x06000944 RID: 2372 RVA: 0x00007A81 File Offset: 0x00005C81
	public static string PlayerName
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return SaveManager.lastPlayerName ?? "Enter Name";
		}
		set
		{
			SaveManager.lastPlayerName = value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000945 RID: 2373 RVA: 0x00007A8E File Offset: 0x00005C8E
	// (set) Token: 0x06000946 RID: 2374 RVA: 0x00007A9A File Offset: 0x00005C9A
	public static uint LastHat
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return SaveManager.lastHat;
		}
		set
		{
			SaveManager.lastHat = value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000947 RID: 2375 RVA: 0x00007AA7 File Offset: 0x00005CA7
	// (set) Token: 0x06000948 RID: 2376 RVA: 0x00007AB3 File Offset: 0x00005CB3
	public static uint LastSkin
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return SaveManager.lastSkin;
		}
		set
		{
			SaveManager.lastSkin = value;
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000949 RID: 2377 RVA: 0x00007AC0 File Offset: 0x00005CC0
	// (set) Token: 0x0600094A RID: 2378 RVA: 0x00007AD3 File Offset: 0x00005CD3
	public static byte BodyColor
	{
		get
		{
			SaveManager.LoadPlayerPrefs();
			return (byte)(SaveManager.colorConfig & 255U);
		}
		set
		{
			SaveManager.colorConfig = ((SaveManager.colorConfig & 16776960U) | (uint)(value & byte.MaxValue));
			SaveManager.SavePlayerPrefs();
		}
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x0600094B RID: 2379 RVA: 0x00030CA8 File Offset: 0x0002EEA8
	// (set) Token: 0x0600094C RID: 2380 RVA: 0x00007AF2 File Offset: 0x00005CF2
	public static GameOptionsData GameHostOptions
	{
		get
		{
			GameOptionsData result;
			if ((result = SaveManager.hostOptionsData) == null)
			{
				result = (SaveManager.hostOptionsData = SaveManager.LoadGameOptions("gameHostOptions_ce"));
			}
			return result;
		}
		set
		{
			SaveManager.hostOptionsData = value;
			SaveManager.SaveGameOptions(SaveManager.hostOptionsData, "gameHostOptions_ce");
		}
	}

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x0600094D RID: 2381 RVA: 0x00030CD0 File Offset: 0x0002EED0
	// (set) Token: 0x0600094E RID: 2382 RVA: 0x00007B09 File Offset: 0x00005D09
	public static GameOptionsData GameSearchOptions
	{
		get
		{
			GameOptionsData result;
			if ((result = SaveManager.searchOptionsData) == null)
			{
				result = (SaveManager.searchOptionsData = SaveManager.LoadGameOptions("gameSearchOptions_ce"));
			}
			return result;
		}
		set
		{
			SaveManager.searchOptionsData = value;
			SaveManager.SaveGameOptions(SaveManager.searchOptionsData, "gameSearchOptions_ce");
		}
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x00030CF8 File Offset: 0x0002EEF8
	private static GameOptionsData LoadGameOptions(string filename)
	{
		string path = Path.Combine(Application.persistentDataPath, filename);
		if (File.Exists(path))
		{
			using (FileStream fileStream = File.OpenRead(path))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					return GameOptionsData.Deserialize(binaryReader) ?? new GameOptionsData();
				}
			}
		}
		return new GameOptionsData();
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x00030D70 File Offset: 0x0002EF70
	private static void SaveGameOptions(GameOptionsData data, string filename)
	{
		using (FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, filename), FileMode.Create, FileAccess.Write))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				data.Serialize(binaryWriter);
			}
		}
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x00030DD0 File Offset: 0x0002EFD0
	private static void LoadAnnouncement()
	{
		if (SaveManager.loadedAnnounce)
		{
			return;
		}
		SaveManager.loadedAnnounce = true;
		string path = Path.Combine(Application.persistentDataPath, "announcement_ce");
		if (File.Exists(path))
		{
			string[] array = File.ReadAllText(path).Split(new char[1]);
			if (array.Length == 3)
			{
				Announcement announcement = default(Announcement);
				SaveManager.TryGetUint(array, 0, out announcement.Id);
				announcement.AnnounceText = array[1];
				SaveManager.TryGetInt(array, 2, out announcement.DateFetched);
				SaveManager.lastAnnounce = announcement;
				return;
			}
			SaveManager.lastAnnounce = default(Announcement);
		}
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x00030E5C File Offset: 0x0002F05C
	public static void SaveAnnouncement()
	{
		File.WriteAllText(Path.Combine(Application.persistentDataPath, "announcement_ce"), string.Join("\0", new object[]
		{
			SaveManager.lastAnnounce.Id,
			SaveManager.lastAnnounce.AnnounceText,
			SaveManager.lastAnnounce.DateFetched
		}));
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x00030EC0 File Offset: 0x0002F0C0
	private static void LoadPlayerPrefs()
	{
		if (SaveManager.loaded)
		{
			return;
		}
		SaveManager.loaded = true;
		string path = Path.Combine(Application.persistentDataPath, "playerPrefs_ce");
		if (File.Exists(path))
		{
			string[] array = File.ReadAllText(path).Split(new char[]
			{
				','
			});
			SaveManager.lastPlayerName = array[0];
			if (array.Length > 1)
			{
				int.TryParse(array[1], out SaveManager.touchConfig);
			}
			if (array.Length <= 2 || !uint.TryParse(array[2], out SaveManager.colorConfig))
			{
				SaveManager.colorConfig = (uint)((byte)(Palette.PlayerColors.RandomIdx<Color32>() << 16) | (byte)(Palette.PlayerColors.RandomIdx<Color32>() << 8) | (byte)Palette.PlayerColors.RandomIdx<Color32>());
			}
			SaveManager.TryGetBool(array, 4, out SaveManager.sendName);
			SaveManager.TryGetBool(array, 5, out SaveManager.sendTelemetry);
			SaveManager.TryGetBool(array, 6, out SaveManager.sendDataScreen);
			SaveManager.TryGetByte(array, 7, out SaveManager.showAdsScreen);
			SaveManager.TryGetBool(array, 8, out SaveManager.showMinPlayerWarning);
			SaveManager.TryGetBool(array, 9, out SaveManager.showOnlineHelp);
			SaveManager.TryGetUint(array, 10, out SaveManager.lastHat);
			SaveManager.TryGetByte(array, 11, out SaveManager.sfxVolume);
			SaveManager.TryGetByte(array, 12, out SaveManager.musicVolume);
			SaveManager.TryGetFloat(array, 13, out SaveManager.joyStickSize, 1f);
			SaveManager.TryGetDateTicks(array, 14, out SaveManager.lastGameStart);
			SaveManager.TryGetUint(array, 15, out SaveManager.lastSkin);
		}
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x00031008 File Offset: 0x0002F208
	private static void SavePlayerPrefs()
	{
		SaveManager.LoadPlayerPrefs();
		File.WriteAllText(Path.Combine(Application.persistentDataPath, "playerPrefs_ce"), string.Join(",", new object[]
		{
			SaveManager.lastPlayerName,
			SaveManager.touchConfig,
			SaveManager.colorConfig,
			1,
			SaveManager.sendName,
			SaveManager.sendTelemetry,
			SaveManager.sendDataScreen,
			SaveManager.showAdsScreen,
			SaveManager.showMinPlayerWarning,
			SaveManager.showOnlineHelp,
			SaveManager.lastHat,
			SaveManager.sfxVolume,
			SaveManager.musicVolume,
			SaveManager.joyStickSize.ToString(CultureInfo.InvariantCulture),
			SaveManager.lastGameStart.Ticks,
			SaveManager.lastSkin
		}));
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x00007B20 File Offset: 0x00005D20
	private static void TryGetBool(string[] parts, int index, out bool value)
	{
		value = false;
		if (parts.Length > index)
		{
			bool.TryParse(parts[index], out value);
		}
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x00007B35 File Offset: 0x00005D35
	private static void TryGetByte(string[] parts, int index, out byte value)
	{
		value = 0;
		if (parts.Length > index)
		{
			byte.TryParse(parts[index], out value);
		}
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x00007B4A File Offset: 0x00005D4A
	private static void TryGetFloat(string[] parts, int index, out float value, float @default = 0f)
	{
		value = @default;
		if (parts.Length > index)
		{
			float.TryParse(parts[index], NumberStyles.Number, CultureInfo.InvariantCulture, out value);
		}
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x00007B66 File Offset: 0x00005D66
	private static void TryGetInt(string[] parts, int index, out int value)
	{
		value = 0;
		if (parts.Length > index)
		{
			int.TryParse(parts[index], out value);
		}
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x00007B7B File Offset: 0x00005D7B
	private static void TryGetUint(string[] parts, int index, out uint value)
	{
		value = 0U;
		if (parts.Length > index)
		{
			uint.TryParse(parts[index], out value);
		}
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x00031118 File Offset: 0x0002F318
	private static void TryGetDateTicks(string[] parts, int index, out DateTime value)
	{
		value = DateTime.MinValue;
		long ticks;
		if (parts.Length > index && long.TryParse(parts[index], out ticks))
		{
			value = new DateTime(ticks, DateTimeKind.Utc);
		}
	}

	// Token: 0x040008DE RID: 2270
	private static bool loaded;

	// Token: 0x040008DF RID: 2271
	private static bool loadedStats;

	// Token: 0x040008E0 RID: 2272
	private static bool loadedAnnounce;

	// Token: 0x040008E1 RID: 2273
	private static string lastPlayerName;

	// Token: 0x040008E2 RID: 2274
	private static byte sfxVolume = byte.MaxValue;

	// Token: 0x040008E3 RID: 2275
	private static byte musicVolume = byte.MaxValue;

	// Token: 0x040008E4 RID: 2276
	private static bool showMinPlayerWarning = true;

	// Token: 0x040008E5 RID: 2277
	private static bool showOnlineHelp = true;

	// Token: 0x040008E6 RID: 2278
	private static bool sendDataScreen = false;

	// Token: 0x040008E7 RID: 2279
	private static byte showAdsScreen = 0;

	// Token: 0x040008E8 RID: 2280
	private static bool sendName = true;

	// Token: 0x040008E9 RID: 2281
	private static bool sendTelemetry = true;

	// Token: 0x040008EA RID: 2282
	private static int touchConfig;

	// Token: 0x040008EB RID: 2283
	private static float joyStickSize = 1f;

	// Token: 0x040008EC RID: 2284
	private static uint colorConfig;

	// Token: 0x040008ED RID: 2285
	private static uint lastHat;

	// Token: 0x040008EE RID: 2286
	private static uint lastSkin;

	// Token: 0x040008EF RID: 2287
	private static GameOptionsData hostOptionsData;

	// Token: 0x040008F0 RID: 2288
	private static GameOptionsData searchOptionsData;

	// Token: 0x040008F1 RID: 2289
	private static DateTime lastGameStart;

	// Token: 0x040008F2 RID: 2290
	private static Announcement lastAnnounce;

	// Token: 0x040008F3 RID: 2291
	private static SaveManager.SecureDataFile secure2 = new SaveManager.SecureDataFile(Path.Combine(Application.persistentDataPath, "secure2_ce"));

	// Token: 0x040008F4 RID: 2292
	private static DateTime lastStartDate = DateTime.MinValue;

	// Token: 0x040008F5 RID: 2293
	private static SaveManager.SecureDataFile purchaseFile = new SaveManager.SecureDataFile(Path.Combine(Application.persistentDataPath, "secureNew_ce"));

	// Token: 0x040008F6 RID: 2294
	private static HashSet<string> purchases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x020001B0 RID: 432
	private class SecureDataFile
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x0600095C RID: 2396 RVA: 0x00007B90 File Offset: 0x00005D90
		// (set) Token: 0x0600095D RID: 2397 RVA: 0x00007B98 File Offset: 0x00005D98
		public bool Loaded { get; private set; }

		// Token: 0x0600095E RID: 2398 RVA: 0x00007BA1 File Offset: 0x00005DA1
		public SecureDataFile(string filePath)
		{
			this.filePath = filePath;
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x000311EC File Offset: 0x0002F3EC
		public BinaryReader LoadData()
		{
			this.Loaded = true;
			Debug.Log("Loading secure: " + this.filePath);
			if (File.Exists(this.filePath))
			{
				byte[] array = File.ReadAllBytes(this.filePath);
				for (int i = 0; i < array.Length; i++)
				{
					byte[] array2 = array;
					int num = i;
					int num2 = num;
					array2[num2] ^= (byte)(i % 212);
				}
				try
				{
					bool flag = true;
					BinaryReader binaryReader = new BinaryReader(new MemoryStream(array));
					if (!(flag & binaryReader.ReadString().Equals(SystemInfo.deviceUniqueIdentifier)))
					{
						binaryReader.Dispose();
						binaryReader = null;
					}
					return binaryReader;
				}
				catch
				{
					Debug.Log("Deleted corrupt secure file");
					this.Delete();
				}
			}
			return null;
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x000312A8 File Offset: 0x0002F4A8
		public void SaveData(params object[] items)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(SystemInfo.deviceUniqueIdentifier);
					foreach (object obj in items)
					{
						if (obj is long)
						{
							binaryWriter.Write((long)obj);
						}
						else if (obj is HashSet<string>)
						{
							foreach (string value in ((HashSet<string>)obj))
							{
								binaryWriter.Write(value);
							}
						}
					}
					binaryWriter.Flush();
					memoryStream.Position = 0L;
					array = memoryStream.ToArray();
				}
			}
			for (int j = 0; j < array.Length; j++)
			{
				byte[] array2 = array;
				int num = j;
				int num2 = num;
				array2[num2] ^= (byte)(j % 212);
			}
			File.WriteAllBytes(this.filePath, array);
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x000313D0 File Offset: 0x0002F5D0
		public void Delete()
		{
			try
			{
				File.Delete(this.filePath);
			}
			catch
			{
			}
		}

		// Token: 0x040008F8 RID: 2296
		private string filePath;
	}
}
