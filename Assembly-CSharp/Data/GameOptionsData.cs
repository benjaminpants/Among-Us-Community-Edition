using System.Collections.Generic;
using System.IO;
using System.Text;
using InnerNet;
using UnityEngine;

public class GameOptionsData : IBytesSerializable
{
	private const byte GameDataVersion = 1;

	public static readonly string[] MapNames;

	public static readonly float[] KillDistances;

	public static readonly string[] KillDistanceStrings;

	public int MaxPlayers;

	public GameKeywords Keywords;

	public byte MapId;

	public float PlayerSpeedMod;

	public float CrewLightMod;

	public float ImpostorLightMod;

	public float KillCooldown;

	public int NumCommonTasks;

	public int NumLongTasks;

	public int NumShortTasks;

	public int NumEmergencyMeetings;

	public int NumImpostors;

	public bool GhostsDoTasks;

	public int KillDistance;

	public int DiscussionTime;

	public int VotingTime;

	public bool isDefaults;

	private static readonly int[] RecommendedKillCooldown;

	private static readonly int[] RecommendedImpostors;

	private static readonly int[] MaxImpostors;

	public static readonly int[] MinPlayers;

	public byte Venting;

	public static readonly string[] VentModeStrings;

	public byte VentMode;

	public static readonly string[] VentMode2Strings;

	public bool AnonVotes;

	public bool ConfirmEject;

	public bool Visuals;

	public byte Gamemode;

	public static string[] Gamemodes;

	public byte SabControl;

	public static readonly string[] SabControlStrings;

	public static bool[] GamemodesAreLua;

	public void ToggleMapFilter(byte newId)
	{
		byte b = (byte)((uint)(MapId ^ (1 << (int)newId)) & 3u);
		if (b != 0)
		{
			MapId = b;
		}
	}

	public bool FilterContainsMap(byte newId)
	{
		int num = 1 << (int)newId;
		return (MapId & num) == num;
	}

	public void SetRecommendations(int numPlayers, GameModes modes)
	{
		numPlayers = Mathf.Clamp(numPlayers, 4, 10);
		PlayerSpeedMod = 1f;
		CrewLightMod = 1f;
		ImpostorLightMod = 1.5f;
		KillCooldown = RecommendedKillCooldown[numPlayers];
		NumCommonTasks = 1;
		NumLongTasks = 1;
		NumShortTasks = 2;
		NumEmergencyMeetings = 1;
		if (modes != GameModes.OnlineGame)
		{
			NumImpostors = RecommendedImpostors[numPlayers];
		}
		KillDistance = 1;
		DiscussionTime = 15;
		VotingTime = 120;
		isDefaults = true;
		Venting = 0;
		VentMode = 0;
		AnonVotes = false;
		ConfirmEject = true;
		Visuals = true;
		Gamemode = 0;
		SabControl = 0;
	}

	public void Serialize(BinaryWriter writer)
	{
		byte value = 1;
		writer.Write(value);
		writer.Write((byte)MaxPlayers);
		writer.Write((uint)Keywords);
		writer.Write(MapId);
		writer.Write(PlayerSpeedMod);
		writer.Write(CrewLightMod);
		writer.Write(ImpostorLightMod);
		writer.Write(KillCooldown);
		writer.Write((byte)NumCommonTasks);
		writer.Write((byte)NumLongTasks);
		writer.Write((byte)NumShortTasks);
		writer.Write(NumEmergencyMeetings);
		writer.Write((byte)NumImpostors);
		writer.Write((byte)KillDistance);
		writer.Write(DiscussionTime);
		writer.Write(VotingTime);
		writer.Write(isDefaults);
		writer.Write(Venting);
		writer.Write(VentMode);
		writer.Write(AnonVotes);
		writer.Write(ConfirmEject);
		writer.Write(Visuals);
		writer.Write(Gamemode);
		writer.Write(SabControl);
	}

	public static GameOptionsData Deserialize(BinaryReader reader)
	{
		try
		{
			reader.ReadByte();
			return new GameOptionsData
			{
				MaxPlayers = reader.ReadByte(),
				Keywords = (GameKeywords)reader.ReadUInt32(),
				MapId = reader.ReadByte(),
				PlayerSpeedMod = reader.ReadSingle(),
				CrewLightMod = reader.ReadSingle(),
				ImpostorLightMod = reader.ReadSingle(),
				KillCooldown = reader.ReadSingle(),
				NumCommonTasks = reader.ReadByte(),
				NumLongTasks = reader.ReadByte(),
				NumShortTasks = reader.ReadByte(),
				NumEmergencyMeetings = reader.ReadInt32(),
				NumImpostors = reader.ReadByte(),
				KillDistance = reader.ReadByte(),
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

	public byte[] ToBytes()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		Serialize(binaryWriter);
		binaryWriter.Flush();
		memoryStream.Position = 0L;
		return memoryStream.ToArray();
	}

	public static GameOptionsData FromBytes(byte[] bytes)
	{
		using MemoryStream input = new MemoryStream(bytes);
		using BinaryReader reader = new BinaryReader(input);
		return Deserialize(reader) ?? new GameOptionsData();
	}

	public override string ToString()
	{
		return ToHudString(20);
	}

	public string ToHudString(int numPlayers)
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		stringBuilder.AppendLine(isDefaults ? "Recommended Settings" : "Custom Settings");
		int num = MaxImpostors[numPlayers];
		stringBuilder.AppendLine("Map: " + MapNames[MapId]);
		stringBuilder.Append($"Impostors: {NumImpostors}");
		if (NumImpostors > num)
		{
			stringBuilder.Append($" (Limit: {num})");
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Emergency Meetings: " + NumEmergencyMeetings);
		stringBuilder.AppendLine($"Discussion Time: {DiscussionTime}s");
		if (VotingTime > 0)
		{
			stringBuilder.AppendLine($"Voting Time: {VotingTime}s");
		}
		else
		{
			stringBuilder.AppendLine("Voting Time: ∞s");
		}
		stringBuilder.AppendLine($"Player Speed: {PlayerSpeedMod}x");
		stringBuilder.AppendLine($"Crew Light: {CrewLightMod}x");
		stringBuilder.AppendLine($"Impostor Light: {ImpostorLightMod}x");
		stringBuilder.AppendLine($"Kill Cooldown: {KillCooldown}s");
		stringBuilder.AppendLine("Kill Distance: " + KillDistanceStrings[KillDistance]);
		stringBuilder.AppendLine("Common Tasks: " + NumCommonTasks);
		stringBuilder.AppendLine("Long Tasks: " + NumLongTasks);
		stringBuilder.AppendLine("Short Tasks: " + NumShortTasks);
		stringBuilder.AppendLine("Vents: " + VentModeStrings[Venting]);
		stringBuilder.AppendLine("Vent Movement: " + VentMode2Strings[VentMode]);
		stringBuilder.AppendLine("Anonymous Votes: " + AnonVotes);
		stringBuilder.AppendLine("Confirm Ejects: " + ConfirmEject);
		stringBuilder.AppendLine("Visual Tasks: " + Visuals);
		stringBuilder.AppendLine("Gamemode: " + Gamemodes[Gamemode]);
		stringBuilder.AppendLine("Sabotages: " + SabControlStrings[SabControl]);
		return stringBuilder.ToString();
	}

	public bool Validate(int numPlayers)
	{
		bool result = false;
		if (NumCommonTasks + NumLongTasks + NumShortTasks == 0)
		{
			NumShortTasks = 1;
			result = true;
		}
		int num = MaxImpostors[numPlayers];
		if (NumImpostors > num)
		{
			NumImpostors = num;
			result = true;
		}
		return result;
	}

	public GameOptionsData()
	{
		ConfirmEject = true;
		Visuals = true;
		MaxPlayers = 20;
		Keywords = GameKeywords.English;
		PlayerSpeedMod = 1f;
		CrewLightMod = 1f;
		ImpostorLightMod = 1.5f;
		KillCooldown = 15f;
		NumCommonTasks = 1;
		NumLongTasks = 1;
		NumShortTasks = 2;
		NumEmergencyMeetings = 1;
		NumImpostors = 1;
		GhostsDoTasks = true;
		KillDistance = 2;
		DiscussionTime = 15;
		VotingTime = 120;
		isDefaults = true;
		Venting = 0;
		VentMode = 0;
		AnonVotes = false;
		ConfirmEject = true;
		Visuals = true;
		Gamemode = 0;
		foreach (KeyValuePair<byte, CE_GamemodeInfo> gamemodeInfo in CE_LuaLoader.GamemodeInfos)
		{
			CE_GamemodeInfo value = gamemodeInfo.Value;
			Gamemodes.SetValue(value.name, value.id - 1);
			GamemodesAreLua.SetValue(true, value.id - 1);
		}
	}

	static GameOptionsData()
	{
		MapNames = new string[2]
		{
			"The Skeld",
			"Mira HQ(Alpha)"
		};
		KillDistances = new float[6]
		{
			0.5f,
			1f,
			1.8f,
			2.5f,
			4f,
			200f
		};
		KillDistanceStrings = new string[6]
		{
			"Tiny",
			"Short",
			"Normal",
			"Long",
			"XL",
			"∞"
		};
		Gamemodes = new string[25]
		{
			"Classic",
			"Zombies",
			"Murder",
			"Hot Potato",
			"Classic+Joker",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			""
		};
		GamemodesAreLua = new bool[25];
		VentModeStrings = new string[4]
		{
			"Impostors Only",
			"Everyone",
			"Crewmates Hide Only",
			"Nobody"
		};
		VentMode2Strings = new string[4]
		{
			"Default",
			"Linked",
			"Pairs",
			"Locked"
		};
		SabControlStrings = new string[5]
		{
			"Normal",
			"Systems Only",
			"Doors Only",
			"Random",
			"None"
		};
		RecommendedKillCooldown = new int[22]
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
		RecommendedImpostors = new int[21]
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
			4,
			4,
			4,
			4,
			4
		};
		MaxImpostors = new int[21]
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
			4,
			4,
			4,
			4,
			4,
			4,
			4,
			4,
			4
		};
		MinPlayers = new int[4]
		{
			4,
			4,
			7,
			9
		};
	}
}
