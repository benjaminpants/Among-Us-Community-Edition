using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class SaveManager
{
	private class SecureDataFile
	{
		private string filePath;

		public bool Loaded
		{
			get;
			private set;
		}

		public SecureDataFile(string filePath)
		{
			this.filePath = filePath;
		}

		public BinaryReader LoadData()
		{
			Loaded = true;
			Debug.Log("Loading secure: " + filePath);
			if (File.Exists(filePath))
			{
				byte[] array = File.ReadAllBytes(filePath);
				for (int i = 0; i < array.Length; i++)
				{
					int num = i;
					array[num] ^= (byte)(i % 212);
				}
				try
				{
					BinaryReader binaryReader = new BinaryReader(new MemoryStream(array));
					if ((1u & (binaryReader.ReadString().Equals(SystemInfo.deviceUniqueIdentifier) ? 1u : 0u)) == 0)
					{
						binaryReader.Dispose();
						binaryReader = null;
					}
					return binaryReader;
				}
				catch
				{
					Debug.Log("Deleted corrupt secure file");
					Delete();
				}
			}
			return null;
		}

		public void SaveData(params object[] items)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write(SystemInfo.deviceUniqueIdentifier);
				foreach (object obj in items)
				{
					if (obj is long)
					{
						binaryWriter.Write((long)obj);
					}
					else
					{
						if (!(obj is HashSet<string>))
						{
							continue;
						}
						foreach (string item in (HashSet<string>)obj)
						{
							binaryWriter.Write(item);
						}
					}
				}
				binaryWriter.Flush();
				memoryStream.Position = 0L;
				array = memoryStream.ToArray();
			}
			for (int j = 0; j < array.Length; j++)
			{
				int num = j;
				array[num] ^= (byte)(j % 212);
			}
			File.WriteAllBytes(filePath, array);
		}

		public void Delete()
		{
			try
			{
				File.Delete(filePath);
			}
			catch
			{
			}
		}
	}

	private static bool loaded;

	private static bool loadedStats;

	private static bool hideIntro;

	private static bool loadedAnnounce;

	private static string lastPlayerName;

	private static byte sfxVolume;

	private static byte musicVolume;

	private static bool showMinPlayerWarning;

	private static bool showOnlineHelp;

	private static bool sendDataScreen;

	private static byte showAdsScreen;

	private static bool sendName;

	private static bool sendTelemetry;

	private static int touchConfig;

	private static float joyStickSize;

	private static uint colorConfig;

	private static uint lastHat;

	private static uint lastSkin;

	private static GameOptionsData hostOptionsData;

	private static GameOptionsData searchOptionsData;

	private static DateTime lastGameStart;

	private static Announcement lastAnnounce;

	private static SecureDataFile secure2;

	private static DateTime lastStartDate;

	private static SecureDataFile purchaseFile;

	private static HashSet<string> purchases;

	private static bool colorBlindMode;

	private static bool lobbyShake;

	private static bool enableProHUDMode;

	private static bool animationTestingMode;

	private static bool enableVSync = false;

	private static bool useLegacyVoteIcons = false;

	public static bool AmBanned => (DateTime.UtcNow - LastGameStart).TotalMinutes < 5.0;

	public static int BanMinutesLeft
	{
		get
		{
			float num = (float)(DateTime.UtcNow - LastGameStart).TotalMinutes;
			if (num > 6f)
			{
				return 0;
			}
			return Mathf.CeilToInt(5f - num);
		}
	}

	public static DateTime LastGameStart
	{
		get
		{
			LoadPlayerPrefs();
			return lastGameStart;
		}
		set
		{
			lastGameStart = value;
			SavePlayerPrefs();
		}
	}

	public static Announcement LastAnnouncement
	{
		get
		{
			LoadAnnouncement();
			return lastAnnounce;
		}
		set
		{
			lastAnnounce = value;
			SaveAnnouncement();
		}
	}

	public static DateTime LastStartDate
	{
		get
		{
			LoadSecureData2();
			return lastStartDate;
		}
		set
		{
			LoadSecureData2();
			if (lastStartDate < value)
			{
				lastStartDate = value;
				SaveSecureData2();
			}
		}
	}

	public static int Month => LastStartDate.Month;

	public static bool BoughtNoAds
	{
		get
		{
			LoadSecureData();
			return purchases.Contains("bought_ads");
		}
	}

	public static ShowAdsState ShowAdsScreen
	{
		get
		{
			LoadPlayerPrefs();
			return (ShowAdsState)showAdsScreen;
		}
		set
		{
			showAdsScreen = (byte)value;
			SavePlayerPrefs();
		}
	}

	public static bool SendDataScreen
	{
		get
		{
			LoadPlayerPrefs();
			return sendDataScreen;
		}
		set
		{
			sendDataScreen = value;
			SavePlayerPrefs();
		}
	}

	public static bool SendName
	{
		get
		{
			LoadPlayerPrefs();
			if (sendName)
			{
				return SendTelemetry;
			}
			return false;
		}
		set
		{
			sendName = value;
			SavePlayerPrefs();
		}
	}

	public static bool SendTelemetry
	{
		get
		{
			LoadPlayerPrefs();
			return sendTelemetry;
		}
		set
		{
			sendTelemetry = value;
			SavePlayerPrefs();
		}
	}

	public static bool ShowMinPlayerWarning
	{
		get
		{
			LoadPlayerPrefs();
			return showMinPlayerWarning;
		}
		set
		{
			showMinPlayerWarning = value;
			SavePlayerPrefs();
		}
	}

	public static bool ShowOnlineHelp
	{
		get
		{
			LoadPlayerPrefs();
			return showOnlineHelp;
		}
		set
		{
			showOnlineHelp = value;
			SavePlayerPrefs();
		}
	}

	public static float SfxVolume
	{
		get
		{
			LoadPlayerPrefs();
			return (float)(int)sfxVolume / 255f;
		}
		set
		{
			sfxVolume = (byte)(value * 255f);
			SavePlayerPrefs();
		}
	}

	public static float MusicVolume
	{
		get
		{
			LoadPlayerPrefs();
			return (float)(int)musicVolume / 255f;
		}
		set
		{
			musicVolume = (byte)(value * 255f);
			SavePlayerPrefs();
		}
	}

	public static int TouchConfig
	{
		get
		{
			LoadPlayerPrefs();
			return touchConfig;
		}
		set
		{
			touchConfig = value;
			SavePlayerPrefs();
		}
	}

	public static float JoystickSize
	{
		get
		{
			LoadPlayerPrefs();
			return joyStickSize;
		}
		set
		{
			joyStickSize = value;
			SavePlayerPrefs();
		}
	}

	public static string PlayerName
	{
		get
		{
			LoadPlayerPrefs();
			return lastPlayerName ?? "Enter Name";
		}
		set
		{
			lastPlayerName = value;
			SavePlayerPrefs();
		}
	}

	public static uint LastHat
	{
		get
		{
			LoadPlayerPrefs();
			return lastHat;
		}
		set
		{
			lastHat = value;
			SavePlayerPrefs();
		}
	}

	public static uint LastSkin
	{
		get
		{
			LoadPlayerPrefs();
			return lastSkin;
		}
		set
		{
			lastSkin = value;
			SavePlayerPrefs();
		}
	}

	public static byte BodyColor
	{
		get
		{
			LoadPlayerPrefs();
			return (byte)(colorConfig & 0xFFu);
		}
		set
		{
			colorConfig = (colorConfig & 0xFFFF00u) | (value & 0xFFu);
			SavePlayerPrefs();
		}
	}

	public static GameOptionsData GameHostOptions
	{
		get
		{
			GameOptionsData result;
			if ((result = hostOptionsData) == null)
			{
				result = (hostOptionsData = LoadGameOptions("gameHostOptions_ce"));
			}
			return result;
		}
		set
		{
			hostOptionsData = value;
			SaveGameOptions(hostOptionsData, "gameHostOptions_ce");
		}
	}

	public static GameOptionsData GameSearchOptions
	{
		get
		{
			GameOptionsData result;
			if ((result = searchOptionsData) == null)
			{
				result = (searchOptionsData = LoadGameOptions("gameSearchOptions_ce"));
			}
			return result;
		}
		set
		{
			searchOptionsData = value;
			SaveGameOptions(searchOptionsData, "gameSearchOptions_ce");
		}
	}

	public static bool ColorBlindMode
	{
		get
		{
			LoadPlayerPrefs();
			return colorBlindMode;
		}
		set
		{
			if (colorBlindMode != value)
			{
				colorBlindMode = value;
				SavePlayerPrefs();
			}
		}
	}

	public static bool LobbyShake
	{
		get
		{
			LoadPlayerPrefs();
			return lobbyShake;
		}
		set
		{
			if (lobbyShake != value)
			{
				lobbyShake = value;
				SavePlayerPrefs();
			}
		}
	}

	public static bool HideIntro
    {
		get
        {
			LoadPlayerPrefs();
			return hideIntro;
        }
		set
        {
			if (hideIntro != value)
            {
				hideIntro = value;
				SavePlayerPrefs();
            }
        }
    }

	public static bool EnableProHUDMode
	{
		get
		{
			LoadPlayerPrefs();
			return enableProHUDMode;
		}
		set
		{
			if (enableProHUDMode != value)
			{
				enableProHUDMode = value;
				SavePlayerPrefs();
			}
		}
	}

	public static bool EnableAnimationTestingMode
    {
        get
        {
            LoadPlayerPrefs();
            return animationTestingMode;
        }
        set
        {
            if (animationTestingMode != value)
            {
                animationTestingMode = value;
                SavePlayerPrefs();
            }
        }
    }

	private static bool usehdshadows;
	public static bool UseHDSHadows
    {
        get
        {
            LoadPlayerPrefs();
            return usehdshadows;
        }
        set
        {
            if (usehdshadows != value)
            {
                usehdshadows = value;
                SavePlayerPrefs();
            }
        }
    }

	private static bool fastanims;
	public static bool FastKillAnims
	{
		get
		{
			LoadPlayerPrefs();
			return fastanims;
		}
		set
		{
			if (fastanims != value)
			{
				fastanims = value;
				SavePlayerPrefs();
			}
		}
	}

	public static bool EnableVSync
    {
		get
        {
			LoadPlayerPrefs();
			return enableVSync;
        }
		set
        {
			enableVSync = value;
			SavePlayerPrefs();
		}
    }

	public static bool UseLegacyVoteIcons
    {
        get
        {
            LoadPlayerPrefs();
            return useLegacyVoteIcons;
        }
        set
        {
            if (useLegacyVoteIcons != value)
            {
                useLegacyVoteIcons = value;
                SavePlayerPrefs();
            }
        }
    }


	private static int CamRes = 256;

	private static bool LSkins = true;

	public static bool LoadSkins
	{
		get
		{
			LoadPlayerPrefs();
			return LSkins;
		}
		set
		{
			if (LSkins != value)
			{
				LSkins = value;
				SavePlayerPrefs();
			}
		}
	}

	public static int CameraRes
	{
		get
		{
			LoadPlayerPrefs();
			return CamRes;
		}
		set
		{
			if (CamRes != value)
			{
				CamRes = value;
				SavePlayerPrefs();
			}
		}
	}

	private static void SaveSecureData2()
	{
		secure2.SaveData(lastStartDate.Ticks);
	}

	private static void LoadSecureData2()
	{
		if (secure2.Loaded)
		{
			return;
		}
		try
		{
			using BinaryReader binaryReader = secure2.LoadData();
			lastStartDate = new DateTime(binaryReader.ReadInt64());
		}
		catch
		{
		}
	}

	public static bool GetPurchase(string key)
	{
		LoadSecureData();
		return purchases.Contains(key);
	}

	public static void SetPurchased(string key)
	{
		LoadSecureData();
		purchases.Add(key);
		if (key == "bought_ads")
		{
			ShowAdsScreen = ShowAdsState.Purchased;
		}
		SaveSecureData();
	}

	private static void LoadSecureData()
	{
		if (purchaseFile.Loaded)
		{
			return;
		}
		try
		{
			using BinaryReader binaryReader = purchaseFile.LoadData();
			while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
			{
				purchases.Add(binaryReader.ReadString());
			}
		}
		catch
		{
			Debug.Log("Deleted corrupt secure file");
			purchaseFile.Delete();
		}
	}

	private static void SaveSecureData()
	{
		purchaseFile.SaveData(purchases);
	}

	private static GameOptionsData LoadGameOptions(string filename)
	{
		string path = Path.Combine(Application.persistentDataPath, filename);
		if (File.Exists(path))
		{
			using (FileStream input = File.OpenRead(path))
			{
				using BinaryReader reader = new BinaryReader(input);
				GameOptionsData gamdat = GameOptionsData.Deserialize(reader) ?? new GameOptionsData();
				return gamdat;
			}
		}
		return new GameOptionsData();
	}

	private static void SaveGameOptions(GameOptionsData data, string filename)
	{
		using FileStream output = new FileStream(Path.Combine(Application.persistentDataPath, filename), FileMode.Create, FileAccess.Write);
		using BinaryWriter writer = new BinaryWriter(output);
		data.Serialize(writer);
	}

	private static void LoadAnnouncement()
	{
		if (loadedAnnounce)
		{
			return;
		}
		loadedAnnounce = true;
		string path = Path.Combine(Application.persistentDataPath, "announcement_ce");
		if (File.Exists(path))
		{
			string[] array = File.ReadAllText(path).Split(default(char));
			if (array.Length == 3)
			{
				Announcement announcement = default(Announcement);
				TryGetUint(array, 0, out announcement.Id);
				announcement.AnnounceText = array[1];
				TryGetInt(array, 2, out announcement.DateFetched);
				lastAnnounce = announcement;
			}
			else
			{
				lastAnnounce = default(Announcement);
			}
		}
	}

	public static void SaveAnnouncement()
	{
		File.WriteAllText(Path.Combine(Application.persistentDataPath, "announcement_ce"), string.Join("\0", lastAnnounce.Id, lastAnnounce.AnnounceText, lastAnnounce.DateFetched));
	}

	public static void ForcePrefLoad()
    {
		LoadPlayerPrefs();
    }

	private static void LoadPlayerPrefs()
	{
		if (loaded)
		{
			return;
		}
		loaded = true;
		string path = Path.Combine(Application.persistentDataPath, "playerPrefs_ce");
		if (File.Exists(path))
		{
			string[] array = File.ReadAllText(path).Split(',');
			lastPlayerName = array[0];
			if (array.Length > 1)
			{
				int.TryParse(array[1], out touchConfig);
			}
			if (array.Length <= 2 || !uint.TryParse(array[2], out colorConfig))
			{
				colorConfig = (uint)((byte)(Palette.PLColors.RandomIdx() << 16) | (byte)(Palette.PLColors.RandomIdx() << 8) | (byte)Palette.PLColors.RandomIdx());
			}
			TryGetBool(array, 4, out sendName);
			TryGetBool(array, 5, out sendTelemetry);
			TryGetBool(array, 6, out sendDataScreen);
			TryGetByte(array, 7, out showAdsScreen);
			TryGetBool(array, 8, out showMinPlayerWarning);
			TryGetBool(array, 9, out showOnlineHelp);
			TryGetUint(array, 10, out lastHat);
			TryGetByte(array, 11, out sfxVolume);
			TryGetByte(array, 12, out musicVolume);
			TryGetFloat(array, 13, out joyStickSize, 1f);
			TryGetDateTicks(array, 14, out lastGameStart);
			TryGetUint(array, 15, out lastSkin);
			TryGetBool(array, 16, out colorBlindMode);
			TryGetBool(array, 17, out animationTestingMode);
			TryGetBool(array, 18, out lobbyShake);
			TryGetBool(array, 19, out enableProHUDMode);
			TryGetBool(array, 20, out hideIntro);
			TryGetBool(array, 21, out enableVSync);
			TryGetBool(array, 22, out useLegacyVoteIcons);
			TryGetInt(array, 23, out CamRes);
            TryGetBool(array, 24, out usehdshadows,true);
			TryGetBool(array, 25, out LSkins,true);
			TryGetBool(array, 26, out fastanims, false);
			if (CamRes == 0)
            {
				CamRes = 256;
            }
		}
	}

	private static void SavePlayerPrefs()
	{
		LoadPlayerPrefs();
		List<object> options = new List<object>();
		options.Add(lastPlayerName); 
		options.Add(touchConfig);
		options.Add(colorConfig);
		options.Add(1);
		options.Add(sendName);
		options.Add(sendTelemetry);
		options.Add(sendDataScreen);
		options.Add(showAdsScreen);
		options.Add(showMinPlayerWarning);
		options.Add(showOnlineHelp);
		options.Add(lastHat);
		options.Add(sfxVolume);
		options.Add(musicVolume);
		options.Add(joyStickSize.ToString(CultureInfo.InvariantCulture));
		options.Add(lastGameStart.Ticks);
		options.Add(lastSkin);
		options.Add(colorBlindMode);
		options.Add(animationTestingMode);
		options.Add(lobbyShake);
		options.Add(enableProHUDMode);
		options.Add(hideIntro);
		options.Add(enableVSync);
		options.Add(useLegacyVoteIcons);
		options.Add(CamRes);
		options.Add(usehdshadows);
		options.Add(LSkins);
		options.Add(fastanims);
		File.WriteAllText(Path.Combine(Application.persistentDataPath, "playerPrefs_ce"), string.Join(",", options));
	}

	private static void TryGetBool(string[] parts, int index, out bool value, bool def = false)
	{
		value = def;
		if (parts.Length > index)
		{
			bool.TryParse(parts[index], out value);
		}
	}

	private static void TryGetByte(string[] parts, int index, out byte value)
	{
		value = 0;
		if (parts.Length > index)
		{
			byte.TryParse(parts[index], out value);
		}
	}

	private static void TryGetFloat(string[] parts, int index, out float value, float @default = 0f)
	{
		value = @default;
		if (parts.Length > index)
		{
			float.TryParse(parts[index], NumberStyles.Number, CultureInfo.InvariantCulture, out value);
		}
	}

	private static void TryGetInt(string[] parts, int index, out int value)
	{
		value = 0;
		if (parts.Length > index)
		{
			int.TryParse(parts[index], out value);
		}
	}

	private static void TryGetUint(string[] parts, int index, out uint value)
	{
		value = 0u;
		if (parts.Length > index)
		{
			uint.TryParse(parts[index], out value);
		}
	}

	private static void TryGetDateTicks(string[] parts, int index, out DateTime value)
	{
		value = DateTime.MinValue;
		if (parts.Length > index && long.TryParse(parts[index], out var result))
		{
			value = new DateTime(result, DateTimeKind.Utc);
		}
	}

	static SaveManager()
	{
		lobbyShake = true;
		enableProHUDMode = false;
		sfxVolume = byte.MaxValue;
		musicVolume = byte.MaxValue;
		showMinPlayerWarning = true;
		showOnlineHelp = true;
		sendDataScreen = false;
		showAdsScreen = 0;
		sendName = true;
		sendTelemetry = true;
		joyStickSize = 1f;
		secure2 = new SecureDataFile(Path.Combine(Application.persistentDataPath, "secure2_ce"));
		lastStartDate = DateTime.MinValue;
		purchaseFile = new SecureDataFile(Path.Combine(Application.persistentDataPath, "secureNew_ce"));
		purchases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
	}
}
