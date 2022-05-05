using System.Collections.Generic;
using System.IO;
using System.Text;
using InnerNet;
using UnityEngine;
using System;

public class GameOptionsData : IBytesSerializable
{
	private const byte GameDataVersion = 3; // make it go up.

	public static readonly string[] MapNames = new string[]
	{
		"The Skeld",
		"Mira HQ",
		"Polus",
		"Airship"
	};

	public static readonly float[] KillDistances;

    public static readonly string[] KillDistanceStrings;

	public static readonly string[] SneakStrings = new string[]
	{
		"Everyone",
		"Impostors Only",
		"No-one",
		"Lua Defined Only",
		"Ghosts Only"
	};

	public int MaxPlayers;

	public GameKeywords Keywords;

	public int MapId; // map id now can be more haha

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

	public bool ImpOnlyChat;

	public bool ShowOtherVision;

	public float MeetingCooldown;

	private static readonly int[] RecommendedKillCooldown;

	private static readonly int[] RecommendedImpostors;

	private static readonly int[] MaxImpostors;

	public static readonly int[] MinPlayers;

    public static readonly string[] TaskBarUpStrings;

    public static readonly string[] BodySett;

    public static readonly string[] BodyDecayTimes;

	public static readonly float[] BodyDecayMul;

    public static readonly string[] CanSeeGhostsStrings;

    public static readonly string[] TaskDifficultyNames;

	public static readonly float[] TaskDifficultyMult;

	public byte CanSeeGhosts;

	public byte TaskBarUpdates;

	public byte Venting;

	public static readonly string[] VentModeStrings;

	public byte VentMode;

	public static readonly string[] VentMode2Strings;

	public bool AnonVotes;

	public bool ConfirmEject;

	public bool Visuals;

	public byte Gamemode;

    public static string[] Gamemodes;

	public static string[] PluginNames;

	public byte SabControl;

	public static readonly string[] SabControlStrings;

	public static bool[] GamemodesAreLua;

	public int MapRot;

	public float MapScaleX;

	public float MapScaleY;

	public byte BodyEffect;

	public byte BodyDecayTime;

	public byte TaskDifficulty;

    public bool GhostsSeeRoles;

	public bool VisionInVents;

	public List<byte> Plugins;

	public List<CE_InterpretedSetting> CustomSettingsRep = new List<CE_InterpretedSetting>();

	public byte Brightness;

	public bool CanSeeOtherImps;

	public float SprintMultipler;

	public byte SneakAllowance;
	
	public byte SprintAllowance;

	public void ToggleMapFilter(byte newId)
	{
			MapId = newId; // map id thing aka mira support
	}

	public bool FilterContainsMap(byte newId)
	{
		int num = 100 << (int)newId;
		return (MapId & num) == num;
	}

    public void WriteByteList(BinaryWriter writer, List<byte> Lis)
    {
		if (Lis == null)
        {
			writer.Write((byte)0);
			return;
        }
        writer.Write((byte)Lis.Count);
		if (Lis.Count != 0)
		{
			byte[] ArrayVersion = Lis.ToArray();
			for (int i = 0; i < ArrayVersion.Length; i++)
			{
				writer.Write(ArrayVersion[i]);
			}
		}
    }
	public static List<byte> ReadByteList(BinaryReader reader)
	{
		byte listlength = reader.ReadByte();
        List<byte> ByteList = new List<byte>();
		if (listlength != 0)
		{
			for (int i = 1; i < (listlength + 1); i++)
			{
				ByteList.Add(reader.ReadByte());
			}
		}
		return ByteList;
	}

	public void WriteCustomSettings(BinaryWriter writer, List<CE_CustomLuaSetting> settings)
    {
		if (!CE_LuaLoader.CurrentGMLua)
		{
			writer.Write((byte)0);
			writer.Write(Gamemode);
			return;
		}
		writer.Write((byte)settings.Count);
		writer.Write(Gamemode);
		foreach (CE_CustomLuaSetting setting in settings)
		{
			writer.Write((byte)setting.DataType);
			switch (setting.DataType)
            {
				case CE_OptDataTypes.String:
                    {
                        writer.Write(setting.StringValue);
                        break;
                    }
				case CE_OptDataTypes.Toggle:
					{
						writer.Write(CE_ConversionHelpers.FloatToBool(setting.NumValue));
						break;
					}
				case CE_OptDataTypes.ByteRange:
                    {
                        writer.Write((byte)setting.NumValue);
                        break;
                    }
				case CE_OptDataTypes.FloatRange:
                    {
                        writer.Write((float)setting.NumValue);
                        break;
                    }
				case CE_OptDataTypes.IntRange:
					{
						writer.Write((int)setting.NumValue);
						break;
					}
			}
		}
	}

	public static List<CE_InterpretedSetting> ReadCustomSettings(BinaryReader reader)
    {
		List<CE_InterpretedSetting> set = new List<CE_InterpretedSetting>();
		try
		{
			byte length = reader.ReadByte();
			if (length == (byte)0)
			{
				return set;
			}
			reader.ReadByte();
			for (byte i = 0; i < length; i++)
			{
				CE_OptDataTypes type = (CE_OptDataTypes)reader.ReadByte();
				CE_InterpretedSetting curset = new CE_InterpretedSetting();
				curset.DataType = type;
				switch (type)
				{
					case CE_OptDataTypes.String:
						{
							curset.StringValue = reader.ReadString();
							break;
						}
					case CE_OptDataTypes.ByteRange:
						{
							curset.NumValue = reader.ReadByte();
							break;
						}
					case CE_OptDataTypes.Toggle:
						{
							curset.NumValue = CE_ConversionHelpers.BoolToFloat(reader.ReadBoolean());
							break;
						}
					case CE_OptDataTypes.FloatRange:
						{
							curset.NumValue = reader.ReadSingle();
							break;
						}
					case CE_OptDataTypes.IntRange:
						{
							curset.NumValue = reader.ReadInt32();
							break;
						}
				}
				set.Add(curset);
			}
		}
		catch
        {

        }
		return set;
	}

	public List<CE_CustomLuaSetting> InterpretSettings()
    {
		List<CE_CustomLuaSetting> setto = CE_LuaLoader.CustomGMSettings[CE_LuaLoader.GamemodeInfos[Gamemode]];

		if (CustomSettingsRep.Count == 0)
        {
			return new List<CE_CustomLuaSetting>();
        }
		for (byte i = 0; i < CustomSettingsRep.Count; i++)
		{
			try
			{
                if (CustomSettingsRep[i].DataType == CE_OptDataTypes.String)
                {
					setto[i].StringValue = CustomSettingsRep[i].StringValue;
                }
                else
                {
					setto[i].NumValue = CustomSettingsRep[i].NumValue;
                }
			}
			catch(Exception E)
            {
				Debug.LogError("Error caught!\n " + E.Message);
            }
		}
		return CE_LuaLoader.CurrentSettings;
	}

	public void SetRecommendations(int numPlayers, GameModes modes)
	{
		numPlayers = Mathf.Clamp(numPlayers, 4, 21);
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
		SabControl = 0;
		MapScaleX = 1f;
		MapScaleY = 1f;
		MapRot = 0;
		TaskBarUpdates = 0;
        GhostsDoTasks = true;
		CanSeeGhosts = 0;
		BodyEffect = 0;
		BodyDecayTime = 1;
		ImpOnlyChat = false;
        ShowOtherVision = false;
		TaskDifficulty = 1;
		GhostsSeeRoles = false;
		VisionInVents = true;
		Plugins = new List<byte>();
		Brightness = 70;
		CanSeeOtherImps = true;
		MapId = 2;
		MeetingCooldown = RecommendedKillCooldown[numPlayers];
		SprintMultipler = 0.5f;
		SneakAllowance = 1;
	 // SprintAllowance = 1;
	}

	public void Serialize(BinaryWriter writer)
	{
		byte value = GameDataVersion;
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
		writer.Write(MapScaleX);
		writer.Write(MapScaleY);
		writer.Write(MapRot);
		writer.Write(TaskBarUpdates);
		writer.Write(GhostsDoTasks);
		writer.Write(CanSeeGhosts);
		writer.Write(BodyEffect);
        writer.Write(BodyDecayTime);
		writer.Write(ImpOnlyChat);
		writer.Write(ShowOtherVision);
		writer.Write(TaskDifficulty);
		writer.Write(GhostsSeeRoles);
		writer.Write(VisionInVents);
		WriteByteList(writer,Plugins);
        writer.Write(Brightness);
		writer.Write(CanSeeOtherImps);
		writer.Write(MeetingCooldown);
        writer.Write(SprintMultipler);
		writer.Write(SneakAllowance);
		WriteCustomSettings(writer,CE_LuaLoader.CurrentSettings);
	}

	public static GameOptionsData Deserialize(BinaryReader reader)
	{
		try
		{
			if (reader.ReadByte() != GameDataVersion)
            {
				return new GameOptionsData();
            }
			GameOptionsData gamedat = new GameOptionsData
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
				SabControl = reader.ReadByte(),
				MapScaleX = reader.ReadSingle(),
				MapScaleY = reader.ReadSingle(),
				MapRot = reader.ReadInt32(),
				TaskBarUpdates = reader.ReadByte(),
				GhostsDoTasks = reader.ReadBoolean(),
				CanSeeGhosts = reader.ReadByte(),
				BodyEffect = reader.ReadByte(),
				BodyDecayTime = reader.ReadByte(),
				ImpOnlyChat = reader.ReadBoolean(),
				ShowOtherVision = reader.ReadBoolean(),
				TaskDifficulty = reader.ReadByte(),
				GhostsSeeRoles = reader.ReadBoolean(),
				VisionInVents = reader.ReadBoolean(),
				Plugins = ReadByteList(reader),
				Brightness = reader.ReadByte(),
				CanSeeOtherImps = reader.ReadBoolean(),
				MeetingCooldown = reader.ReadSingle(),
				SprintMultipler = reader.ReadSingle(),
				SneakAllowance = reader.ReadByte(),
				CustomSettingsRep = ReadCustomSettings(reader)
			};
            gamedat.InterpretSettings();
			
			return gamedat;
		}
		catch(Exception E)
		{
			Debug.LogError(E.Message);
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
		GameOptionsData gamdat = Deserialize(reader) ?? new GameOptionsData();
		return gamdat;
	}

	public override string ToString()
	{
		return ToHudString(20);
	}

	public string ToHudString(int numPlayers)
	{
		try
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
			stringBuilder.AppendLine("Confirm Ejects: " + ConfirmEject);
            stringBuilder.AppendLine("Emergency Meetings: " + NumEmergencyMeetings);
			stringBuilder.AppendLine("Anonymous Votes: " + AnonVotes);
			stringBuilder.AppendLine("Meeting Cooldown: " + MeetingCooldown);
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
			stringBuilder.AppendLine($"Sneak Multiplier: {SprintMultipler}x");
			stringBuilder.AppendLine($"Sneak Usage: " + SneakStrings[SneakAllowance]);
			//stringBuilder.AppendLine("[FF0000FF]Sneaking is disabled this update![]");
			stringBuilder.AppendLine("[FF0000FF]Sprinting is disabled in this update![]");
			if (CrewLightMod == 0f)
			{
				stringBuilder.AppendLine($"Crewmate Vision: {Constants.InfinitySymbol}x");
			}
			else
			{
				stringBuilder.AppendLine($"Crewmate Vision: {CrewLightMod}x");
			}
			if (ImpostorLightMod == 0f)
			{
				stringBuilder.AppendLine($"Impostor Vision: {Constants.InfinitySymbol}x");
			}
			else
			{
				stringBuilder.AppendLine($"Impostor Vision: {ImpostorLightMod}x");
			}
			stringBuilder.AppendLine($"Kill Cooldown: {KillCooldown}s");
            stringBuilder.AppendLine("Kill Distance: " + KillDistanceStrings[KillDistance]);
			stringBuilder.AppendLine("Taskbar Updates: " + TaskBarUpStrings[TaskBarUpdates]);
			stringBuilder.AppendLine("Visual Tasks: " + Visuals);
			stringBuilder.AppendLine("Common Tasks: " + NumCommonTasks);
			stringBuilder.AppendLine("Long Tasks: " + NumLongTasks);
			stringBuilder.AppendLine("Short Tasks: " + NumShortTasks);
			stringBuilder.AppendLine("Vents: " + VentModeStrings[Venting]);
            stringBuilder.AppendLine("Vent Movement: " + VentMode2Strings[VentMode]);
			stringBuilder.AppendLine("Vision In Vents: " + VisionInVents);
            stringBuilder.AppendLine("Gamemode: " + Gamemodes[Gamemode]);
			string pluginlist = string.Empty;
			foreach (byte b in Plugins)
			{
				string beg = string.Empty;
				string end = string.Empty;
				if (CE_LuaLoader.IsPluginAnOverride((byte)(b)))
				{
					beg = "[00FF00FF]";
					end = "[]";
				}
				pluginlist = pluginlist + beg + PluginNames[b] + end + ",";
			}
			if (Plugins.Count != 0)
			{
				pluginlist = pluginlist.Remove(pluginlist.Length - 1);
			}
			else
			{
				pluginlist = "[0000FFFF]None[]";
			}
			stringBuilder.AppendLine("Plugins: " + pluginlist);
			stringBuilder.AppendLine("Map X Scale: " + MapScaleX);
			stringBuilder.AppendLine("Map Y Scale: " + MapScaleY);
            stringBuilder.AppendLine("Map Rotation: " + MapRot);
			stringBuilder.AppendLine("Sabotages: " + SabControlStrings[SabControl]);
			stringBuilder.AppendLine("Ghosts Do Tasks: " + GhostsDoTasks);
			stringBuilder.AppendLine("Ghost Visibility: " + CanSeeGhostsStrings[CanSeeGhosts]);
			stringBuilder.AppendLine("Body Effect: " + BodySett[BodyEffect]);
			if (BodyEffect == 1)
			{
				stringBuilder.AppendLine("Body Decay Time: " + BodyDecayTimes[BodyDecayTime]);
			}
            stringBuilder.AppendLine("Allow Impostor Only Chat: " + ImpOnlyChat);
			stringBuilder.AppendLine("Impostors Know Eachother: " + CanSeeOtherImps);
			stringBuilder.AppendLine("Show All Vision: " + ShowOtherVision);
			stringBuilder.AppendLine("Task Difficulty: " + TaskDifficultyNames[TaskDifficulty]);
			stringBuilder.AppendLine("Ghosts See Roles: " + GhostsSeeRoles);
            stringBuilder.AppendLine("Brightness: " + Brightness);
			string settingstring = string.Empty;
			if (CE_LuaLoader.CurrentGMLua)
			{
				foreach (CE_CustomLuaSetting setting in CE_LuaLoader.CurrentSettings)
				{
					if (setting.DataType == CE_OptDataTypes.String)
					{
						settingstring += setting.Name + ": " + setting.StringValue;
					}
					else if (setting.DataType == CE_OptDataTypes.FloatRange || setting.DataType == CE_OptDataTypes.ByteRange || setting.DataType == CE_OptDataTypes.IntRange)
					{
						settingstring += setting.Name + ": " + setting.NumValue;
					}
					else if (setting.DataType == CE_OptDataTypes.Toggle)
                    {
						settingstring += setting.Name + ": " + CE_ConversionHelpers.FloatToBool(setting.NumValue);
					}
					settingstring += "\n";
				}
			}
			stringBuilder.AppendLine(settingstring);
			return stringBuilder.ToString();
		}
		catch(Exception E)
        {
			return "[FF0000FF]Error caught!\n " + E.Message + "\n " + E.StackTrace;
		}
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
		TaskBarUpdates = 0;
		CanSeeGhosts = 0;
		BodyDecayTime = 1;
		BodyEffect = 0;
		MapScaleX = 1f;
		MapScaleY = 1f;
		MapRot = 0;
		TaskBarUpdates = 0;
		GhostsDoTasks = true;
		CanSeeGhosts = 0;
		BodyEffect = 0;
		BodyDecayTime = 1;
		TaskDifficulty = 1;
		GhostsSeeRoles = false;
		VisionInVents = true;
		Brightness = 70;
        for (int i = 0; i < CE_LuaLoader.GamemodeInfos.Count; i++)
        {
            Gamemodes.SetValue(CE_LuaLoader.GamemodeInfos[i].name, i);
            GamemodesAreLua.SetValue(true, i);
        }
        for (int i = 0; i < CE_LuaLoader.PluginInfos.Count; i++)
        {
            PluginNames.SetValue(CE_LuaLoader.PluginInfos[i].name, i);
        }
		List<string> names = new List<string>();
		for (int i = 0; i < CE_CustomMapManager.MapInfos.Count; i++)
		{
			names.Add(CE_CustomMapManager.MapInfos[i].MapName);
		}
//		MapNames = names.ToArray();
		CanSeeOtherImps = true;
		MeetingCooldown = 15f;
	}

	static GameOptionsData()
	{
		KillDistances = new float[7]
		{
			0.5f,
			1f,
			1.8f,
			2.5f,
			4f,
			4.8f,
			300f
		};
		KillDistanceStrings = new string[7]
		{
			"Tiny",
			"Short",
			"Normal",
			"Long",
			"XL",
			"Extreme",
			"∞"
		};

        BodySett = new string[3]
        {
            "Vanilla",
            "Decay",
            "Anon"
        };

        BodyDecayTimes = new string[3]
        {
            "Short",
            "Medium",
            "Long"
        };


        TaskDifficultyNames = new string[4]
        {
            "Easy",
            "Normal",
            "Hard",
            "Insane"
        };

		TaskDifficultyMult = new float[4]
		{
			0.5f,
			1f,
			2f,
			4f
		};


		BodyDecayMul = new float[3]
		{
			0.5f,
			1f,
			1.5f
		};

		TaskBarUpStrings = new string[3]
		{
			"Always",
			"Meetings",
			"Never"
		};

		CanSeeGhostsStrings = new string[4]
		{
			"Dead Only",
			"Impostors Only",
			"Everyone",
			"Nobody"
		};

		Gamemodes = new string[256];
		PluginNames = new string[256];
		GamemodesAreLua = new bool[256];

		VentModeStrings = new string[4]
		{
			"Impostors Only",
			"Everyone",
			"Crewmates Hide Only",
			"Nobody"
		};
		VentMode2Strings = new string[6]
		{
			"Default",
			"Linked",
			"Pairs",
			"Locked",
			"Randomized(Client)",
			"Randomized(One-Way Client)"
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
			1,
			1,
			1,
			1,
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
			1,
			1,
			1,
			1,
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
