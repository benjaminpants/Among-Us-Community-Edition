using System;
using System.IO;
using System.Text;
using InnerNet;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class GameOptionsData : IBytesSerializable
{
	// Token: 0x06000999 RID: 2457 RVA: 0x00031F40 File Offset: 0x00030140
	public void ToggleMapFilter(byte newId)
	{
		byte b = (byte)(((int)this.MapId ^ 1 << (int)newId) & 3);
		if (b != 0)
		{
			this.MapId = b;
		}
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x00031F68 File Offset: 0x00030168
	public bool FilterContainsMap(byte newId)
	{
		int num = 1 << (int)newId;
		return ((int)this.MapId & num) == num;
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x00031F88 File Offset: 0x00030188
	public void SetRecommendations(int numPlayers, GameModes modes)
	{
		numPlayers = Mathf.Clamp(numPlayers, 4, 10);
		this.PlayerSpeedMod = 1f;
		this.CrewLightMod = 1f;
		this.ImpostorLightMod = 1.5f;
		this.KillCooldown = (float)GameOptionsData.RecommendedKillCooldown[numPlayers];
		this.NumCommonTasks = 1;
		this.NumLongTasks = 1;
		this.NumShortTasks = 2;
		this.NumEmergencyMeetings = 1;
		if (modes != GameModes.OnlineGame)
		{
			this.NumImpostors = GameOptionsData.RecommendedImpostors[numPlayers];
		}
		this.KillDistance = 1;
		this.DiscussionTime = 15;
		this.VotingTime = 120;
		this.isDefaults = true;
		this.Venting = 0;
		this.VentMode = 0;
		this.AnonVotes = false;
		this.ConfirmEject = true;
		this.Visuals = true;
		this.Gamemode = 0;
		this.SabControl = 0;
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x0003204C File Offset: 0x0003024C
	public void Serialize(BinaryWriter writer)
	{
		byte value = 1;
		writer.Write(value);
		writer.Write((byte)this.MaxPlayers);
		writer.Write((uint)this.Keywords);
		writer.Write(this.MapId);
		writer.Write(this.PlayerSpeedMod);
		writer.Write(this.CrewLightMod);
		writer.Write(this.ImpostorLightMod);
		writer.Write(this.KillCooldown);
		writer.Write((byte)this.NumCommonTasks);
		writer.Write((byte)this.NumLongTasks);
		writer.Write((byte)this.NumShortTasks);
		writer.Write(this.NumEmergencyMeetings);
		writer.Write((byte)this.NumImpostors);
		writer.Write((byte)this.KillDistance);
		writer.Write(this.DiscussionTime);
		writer.Write(this.VotingTime);
		writer.Write(this.isDefaults);
		writer.Write(this.Venting);
		writer.Write(this.VentMode);
		writer.Write(this.AnonVotes);
		writer.Write(this.ConfirmEject);
		writer.Write(this.Visuals);
		writer.Write(this.Gamemode);
		writer.Write(this.SabControl);
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x0003217C File Offset: 0x0003037C
	public static GameOptionsData Deserialize(BinaryReader reader)
	{
		try
		{
			reader.ReadByte();
			return new GameOptionsData
			{
				MaxPlayers = (int)reader.ReadByte(),
				Keywords = (GameKeywords)reader.ReadUInt32(),
				MapId = reader.ReadByte(),
				PlayerSpeedMod = reader.ReadSingle(),
				CrewLightMod = reader.ReadSingle(),
				ImpostorLightMod = reader.ReadSingle(),
				KillCooldown = reader.ReadSingle(),
				NumCommonTasks = (int)reader.ReadByte(),
				NumLongTasks = (int)reader.ReadByte(),
				NumShortTasks = (int)reader.ReadByte(),
				NumEmergencyMeetings = reader.ReadInt32(),
				NumImpostors = (int)reader.ReadByte(),
				KillDistance = (int)reader.ReadByte(),
				DiscussionTime = reader.ReadInt32(),
				VotingTime = reader.ReadInt32(),
				isDefaults = reader.ReadBoolean(),
				Venting = reader.ReadByte(),
				VentMode = reader.ReadByte(),
				AnonVotes = reader.ReadBoolean(),
				ConfirmEject = reader.ReadBoolean(),
				Visuals = reader.ReadBoolean(),
				Gamemode = reader.ReadByte(),
				SabControl = reader.ReadByte()
			};
		}
		catch
		{
		}
		return null;
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x000322D0 File Offset: 0x000304D0
	public byte[] ToBytes()
	{
		byte[] result;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				this.Serialize(binaryWriter);
				binaryWriter.Flush();
				memoryStream.Position = 0L;
				result = memoryStream.ToArray();
			}
		}
		return result;
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x0003233C File Offset: 0x0003053C
	public static GameOptionsData FromBytes(byte[] bytes)
	{
		GameOptionsData result;
		using (MemoryStream memoryStream = new MemoryStream(bytes))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				result = (GameOptionsData.Deserialize(binaryReader) ?? new GameOptionsData());
			}
		}
		return result;
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x00007D10 File Offset: 0x00005F10
	public override string ToString()
	{
		return this.ToHudString(10);
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x0003239C File Offset: 0x0003059C
	public string ToHudString(int numPlayers)
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		stringBuilder.AppendLine(this.isDefaults ? "Recommended Settings" : "Custom Settings");
		int num = GameOptionsData.MaxImpostors[numPlayers];
		stringBuilder.AppendLine("Map: " + GameOptionsData.MapNames[(int)this.MapId]);
		stringBuilder.Append(string.Format("Impostors: {0}", this.NumImpostors));
		if (this.NumImpostors > num)
		{
			stringBuilder.Append(string.Format(" (Limit: {0})", num));
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Emergency Meetings: " + this.NumEmergencyMeetings);
		stringBuilder.AppendLine(string.Format("Discussion Time: {0}s", this.DiscussionTime));
		if (this.VotingTime > 0)
		{
			stringBuilder.AppendLine(string.Format("Voting Time: {0}s", this.VotingTime));
		}
		else
		{
			stringBuilder.AppendLine("Voting Time: ∞s");
		}
		stringBuilder.AppendLine(string.Format("Player Speed: {0}x", this.PlayerSpeedMod));
		stringBuilder.AppendLine(string.Format("Crew Light: {0}x", this.CrewLightMod));
		stringBuilder.AppendLine(string.Format("Impostor Light: {0}x", this.ImpostorLightMod));
		stringBuilder.AppendLine(string.Format("Kill Cooldown: {0}s", this.KillCooldown));
		stringBuilder.AppendLine("Kill Distance: " + GameOptionsData.KillDistanceStrings[this.KillDistance]);
		stringBuilder.AppendLine("Common Tasks: " + this.NumCommonTasks);
		stringBuilder.AppendLine("Long Tasks: " + this.NumLongTasks);
		stringBuilder.AppendLine("Short Tasks: " + this.NumShortTasks);
		stringBuilder.AppendLine("Vents: " + GameOptionsData.VentModeStrings[(int)this.Venting]);
		stringBuilder.AppendLine("Vent Movement: " + GameOptionsData.VentMode2Strings[(int)this.VentMode]);
		stringBuilder.AppendLine("Anonymous Votes: " + this.AnonVotes.ToString());
		stringBuilder.AppendLine("Confirm Ejects: " + this.ConfirmEject.ToString());
		stringBuilder.AppendLine("Visual Tasks: " + this.Visuals.ToString());
		stringBuilder.AppendLine("Gamemode: " + GameOptionsData.Gamemodes[(int)this.Gamemode]);
		stringBuilder.AppendLine("Sabotages: " + GameOptionsData.SabControlStrings[(int)this.SabControl]);
		return stringBuilder.ToString();
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00032654 File Offset: 0x00030854
	public bool Validate(int numPlayers)
	{
		bool result = false;
		if (this.NumCommonTasks + this.NumLongTasks + this.NumShortTasks == 0)
		{
			this.NumShortTasks = 1;
			result = true;
		}
		int num = GameOptionsData.MaxImpostors[numPlayers];
		if (this.NumImpostors > num)
		{
			this.NumImpostors = num;
			result = true;
		}
		return result;
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x000326A0 File Offset: 0x000308A0
	public GameOptionsData()
	{
		this.MaxPlayers = 10;
		this.Keywords = GameKeywords.English;
		this.PlayerSpeedMod = 1f;
		this.CrewLightMod = 1f;
		this.ImpostorLightMod = 1.5f;
		this.KillCooldown = 15f;
		this.NumCommonTasks = 1;
		this.NumLongTasks = 1;
		this.NumShortTasks = 2;
		this.NumEmergencyMeetings = 1;
		this.NumImpostors = 1;
		this.GhostsDoTasks = true;
		this.KillDistance = 1;
		this.DiscussionTime = 15;
		this.VotingTime = 120;
		this.isDefaults = true;
		this.Venting = 0;
		this.VentMode = 0;
		this.AnonVotes = false;
		this.ConfirmEject = true;
		this.Visuals = true;
		this.Gamemode = 0;
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00032770 File Offset: 0x00030970
	static GameOptionsData()
	{
		GameOptionsData.VentModeStrings = new string[]
		{
			"Impostors Only",
			"Everyone",
			"Crewmates Hide Only",
			"Nobody"
		};
		GameOptionsData.VentMode2Strings = new string[]
		{
			"Default",
			"Linked",
			"Pairs",
			"Locked"
		};
		GameOptionsData.SabControlStrings = new string[]
		{
			"Normal",
			"Systems Only",
			"Doors Only",
			"Random",
			"None"
		};
		GameOptionsData.RecommendedKillCooldown = new int[]
		{
			0,
			0,
			0,
			0,
			45,
			30,
			15,
			35,
			30,
			25,
			20,
			20,
			20,
			20,
			20,
			10,
			10,
			10,
			10,
			10,
			10,
			10
		};
		GameOptionsData.RecommendedImpostors = new int[]
		{
			0,
			0,
			0,
			0,
			1,
			1,
			1,
			2,
			2,
			2,
			2,
			2,
			2,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3
		};
		GameOptionsData.MaxImpostors = new int[]
		{
			0,
			0,
			0,
			0,
			1,
			1,
			1,
			2,
			2,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3
		};
		GameOptionsData.MinPlayers = new int[]
		{
			4,
			4,
			7,
			9
		};
	}

	// Token: 0x04000923 RID: 2339
	private const byte GameDataVersion = 1;

	// Token: 0x04000924 RID: 2340
	public static readonly string[] MapNames = new string[]
	{
		"The Skeld",
		"Mira HQ(Alpha)"
	};

	// Token: 0x04000925 RID: 2341
	public static readonly float[] KillDistances = new float[]
	{
		1f,
		1.8f,
		2.5f
	};

	// Token: 0x04000926 RID: 2342
	public static readonly string[] KillDistanceStrings = new string[]
	{
		"Short",
		"Normal",
		"Long"
	};

	// Token: 0x04000927 RID: 2343
	public int MaxPlayers;

	// Token: 0x04000928 RID: 2344
	public GameKeywords Keywords;

	// Token: 0x04000929 RID: 2345
	public byte MapId;

	// Token: 0x0400092A RID: 2346
	public float PlayerSpeedMod;

	// Token: 0x0400092B RID: 2347
	public float CrewLightMod;

	// Token: 0x0400092C RID: 2348
	public float ImpostorLightMod;

	// Token: 0x0400092D RID: 2349
	public float KillCooldown;

	// Token: 0x0400092E RID: 2350
	public int NumCommonTasks;

	// Token: 0x0400092F RID: 2351
	public int NumLongTasks;

	// Token: 0x04000930 RID: 2352
	public int NumShortTasks;

	// Token: 0x04000931 RID: 2353
	public int NumEmergencyMeetings;

	// Token: 0x04000932 RID: 2354
	public int NumImpostors;

	// Token: 0x04000933 RID: 2355
	public bool GhostsDoTasks;

	// Token: 0x04000934 RID: 2356
	public int KillDistance;

	// Token: 0x04000935 RID: 2357
	public int DiscussionTime;

	// Token: 0x04000936 RID: 2358
	public int VotingTime;

	// Token: 0x04000937 RID: 2359
	public bool isDefaults;

	// Token: 0x04000938 RID: 2360
	private static readonly int[] RecommendedKillCooldown;

	// Token: 0x04000939 RID: 2361
	private static readonly int[] RecommendedImpostors;

	// Token: 0x0400093A RID: 2362
	private static readonly int[] MaxImpostors;

	// Token: 0x0400093B RID: 2363
	public static readonly int[] MinPlayers;

	// Token: 0x0400093C RID: 2364
	public byte Venting;

	// Token: 0x0400093D RID: 2365
	public static readonly string[] VentModeStrings;

	// Token: 0x0400093E RID: 2366
	public byte VentMode;

	// Token: 0x0400093F RID: 2367
	public static readonly string[] VentMode2Strings;

	// Token: 0x04000940 RID: 2368
	public bool AnonVotes;

	// Token: 0x04000941 RID: 2369
	public bool ConfirmEject = true;

	// Token: 0x04000942 RID: 2370
	public bool Visuals = true;

	// Token: 0x04000943 RID: 2371
	public byte Gamemode;

	// Token: 0x04000944 RID: 2372
	public static readonly string[] Gamemodes = new string[]
	{
		"Classic",
		"Zombies",
		"Murder"
	};

	// Token: 0x04000945 RID: 2373
	public byte SabControl;

	// Token: 0x04000946 RID: 2374
	public static readonly string[] SabControlStrings;
}
