using System.IO;
using UnityEngine;

public class StatsManager
{
	public static StatsManager Instance;

	private const byte StatsVersion = 1;

	private bool loadedStats;

	private uint bodiesReported;

	private uint emergenciesCalls;

	private uint tasksCompleted;

	private uint completedAllTasks;

	private uint sabsFixed;

	private uint impostorKills;

	private uint timesMurdered;

	private uint timesEjected;

	private uint crewmateStreak;

	private uint timesImpostor;

	private uint timesCrewmate;

	private uint gamesStarted;

	private uint gamesFinished;

	private uint[] WinReasons;

	private uint[] LoseReasons;

	public uint BodiesReported
	{
		get
		{
			LoadStats();
			return bodiesReported;
		}
		set
		{
			LoadStats();
			bodiesReported = value;
			SaveStats();
		}
	}

	public uint EmergenciesCalled
	{
		get
		{
			LoadStats();
			return emergenciesCalls;
		}
		set
		{
			LoadStats();
			emergenciesCalls = value;
			SaveStats();
		}
	}

	public uint TasksCompleted
	{
		get
		{
			LoadStats();
			return tasksCompleted;
		}
		set
		{
			LoadStats();
			tasksCompleted = value;
			SaveStats();
		}
	}

	public uint CompletedAllTasks
	{
		get
		{
			LoadStats();
			return completedAllTasks;
		}
		set
		{
			LoadStats();
			completedAllTasks = value;
			SaveStats();
		}
	}

	public uint SabsFixed
	{
		get
		{
			LoadStats();
			return sabsFixed;
		}
		set
		{
			LoadStats();
			sabsFixed = value;
			SaveStats();
		}
	}

	public uint ImpostorKills
	{
		get
		{
			LoadStats();
			return impostorKills;
		}
		set
		{
			LoadStats();
			impostorKills = value;
			SaveStats();
		}
	}

	public uint TimesMurdered
	{
		get
		{
			LoadStats();
			return timesMurdered;
		}
		set
		{
			LoadStats();
			timesMurdered = value;
			SaveStats();
		}
	}

	public uint TimesEjected
	{
		get
		{
			LoadStats();
			return timesEjected;
		}
		set
		{
			LoadStats();
			timesEjected = value;
			SaveStats();
		}
	}

	public uint CrewmateStreak
	{
		get
		{
			LoadStats();
			return crewmateStreak;
		}
		set
		{
			LoadStats();
			crewmateStreak = value;
			SaveStats();
		}
	}

	public uint TimesImpostor
	{
		get
		{
			LoadStats();
			return timesImpostor;
		}
		set
		{
			LoadStats();
			timesImpostor = value;
			SaveStats();
		}
	}

	public uint TimesCrewmate
	{
		get
		{
			LoadStats();
			return timesCrewmate;
		}
		set
		{
			LoadStats();
			timesCrewmate = value;
			SaveStats();
		}
	}

	public uint GamesStarted
	{
		get
		{
			LoadStats();
			return gamesStarted;
		}
		set
		{
			LoadStats();
			gamesStarted = value;
			SaveStats();
		}
	}

	public uint GamesFinished
	{
		get
		{
			LoadStats();
			return gamesFinished;
		}
		set
		{
			LoadStats();
			gamesFinished = value;
			SaveStats();
		}
	}

	public void AddWinReason(GameOverReason reason)
	{
		LoadStats();
		WinReasons[(int)reason]++;
		SaveStats();
	}

	public uint GetWinReason(GameOverReason reason)
	{
		LoadStats();
		return WinReasons[(int)reason];
	}

	public void AddLoseReason(GameOverReason reason)
	{
		LoadStats();
		LoseReasons[(int)reason]++;
		SaveStats();
	}

	public uint GetLoseReason(GameOverReason reason)
	{
		LoadStats();
		return LoseReasons[(int)reason];
	}

	protected virtual void LoadStats()
	{
		if (loadedStats)
		{
			return;
		}
		loadedStats = true;
		string path = Path.Combine(Application.persistentDataPath, "playerStats2_ce");
		if (!File.Exists(path))
		{
			return;
		}
		try
		{
			using BinaryReader binaryReader = new BinaryReader(File.OpenRead(path));
			binaryReader.ReadByte();
			bodiesReported = binaryReader.ReadUInt32();
			emergenciesCalls = binaryReader.ReadUInt32();
			tasksCompleted = binaryReader.ReadUInt32();
			completedAllTasks = binaryReader.ReadUInt32();
			sabsFixed = binaryReader.ReadUInt32();
			impostorKills = binaryReader.ReadUInt32();
			timesMurdered = binaryReader.ReadUInt32();
			timesEjected = binaryReader.ReadUInt32();
			crewmateStreak = binaryReader.ReadUInt32();
			timesImpostor = binaryReader.ReadUInt32();
			timesCrewmate = binaryReader.ReadUInt32();
			gamesStarted = binaryReader.ReadUInt32();
			gamesFinished = binaryReader.ReadUInt32();
			for (int i = 0; i < WinReasons.Length; i++)
			{
				WinReasons[i] = binaryReader.ReadUInt32();
			}
			for (int j = 0; j < LoseReasons.Length; j++)
			{
				LoseReasons[j] = binaryReader.ReadUInt32();
			}
		}
		catch
		{
			Debug.LogError("Deleting corrupted stats file");
			File.Delete(path);
		}
	}

	protected virtual void SaveStats()
	{
		try
		{
			using BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(Path.Combine(Application.persistentDataPath, "playerStats2_ce")));
			binaryWriter.Write(1);
			binaryWriter.Write(bodiesReported);
			binaryWriter.Write(emergenciesCalls);
			binaryWriter.Write(tasksCompleted);
			binaryWriter.Write(completedAllTasks);
			binaryWriter.Write(sabsFixed);
			binaryWriter.Write(impostorKills);
			binaryWriter.Write(timesMurdered);
			binaryWriter.Write(timesEjected);
			binaryWriter.Write(crewmateStreak);
			binaryWriter.Write(timesImpostor);
			binaryWriter.Write(timesCrewmate);
			binaryWriter.Write(gamesStarted);
			binaryWriter.Write(gamesFinished);
			for (int i = 0; i < WinReasons.Length; i++)
			{
				binaryWriter.Write(WinReasons[i]);
			}
			for (int j = 0; j < LoseReasons.Length; j++)
			{
				binaryWriter.Write(LoseReasons[j]);
			}
		}
		catch
		{
			Debug.Log("Failed to write out stats");
		}
	}

	public StatsManager()
	{
		WinReasons = new uint[8];
		LoseReasons = new uint[8];
	}

	static StatsManager()
	{
		Instance = new StatsManager();
	}
}
