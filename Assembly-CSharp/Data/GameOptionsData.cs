using System.IO;
using System.Text;
using InnerNet;
using UnityEngine;

public class GameOptionsData : IBytesSerializable
{
	private const byte GameDataVersion = 1;

	public static readonly string[] MapNames = new string[2]
	{
		"The Skeld",
		"???"
	};

	public static readonly float[] KillDistances = new float[3]
	{
		1f,
		1.8f,
		2.5f
	};

	public static readonly string[] KillDistanceStrings = new string[3]
	{
		"Short",
		"Normal",
		"Long"
	};

	public int MaxPlayers = 10;

	public GameKeywords Keywords = GameKeywords.English;

	public byte MapId;

	public float PlayerSpeedMod = 1f;

	public float CrewLightMod = 1f;

	public float ImpostorLightMod = 1.5f;

	public float KillCooldown = 15f;

	public int NumCommonTasks = 1;

	public int NumLongTasks = 1;

	public int NumShortTasks = 2;

	public int NumEmergencyMeetings = 1;

	public int NumImpostors = 1;

	public bool GhostsDoTasks = true;

	public int KillDistance = 1;

	public int DiscussionTime = 15;

	public int VotingTime = 120;

	public bool isDefaults = true;

	private static readonly int[] RecommendedKillCooldown = new int[11]
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
		20
	};

	private static readonly int[] RecommendedImpostors = new int[11]
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
		2
	};

	private static readonly int[] MaxImpostors = new int[11]
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
		3
	};

	public static readonly int[] MinPlayers = new int[4]
	{
		4,
		4,
		7,
		9
	};

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
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write((byte)1);
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
	}

	public static GameOptionsData Deserialize(BinaryReader reader)
	{
		try
		{
			if (reader.ReadByte() != 1)
			{
				return null;
			}
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
				isDefaults = reader.ReadBoolean()
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
		return ToHudString(10);
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
		stringBuilder.Append("Short Tasks: " + NumShortTasks);
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
}
