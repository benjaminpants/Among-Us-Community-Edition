using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class StatsManager
{
	public static StatsManager Instance;

	private const byte StatsVersion = 255; //make this count backwards for every version, ik its weird but idc

	private bool loadedStats;

	private uint tasksCompleted;

	private uint completedAllTasks;

	private uint sabsFixed;

	private uint timesMurdered;

	private uint gamesFinished;

	private uint stalemates; //revivedplayers
	
	private uint revivedplayers; // POKE!!!

    private Dictionary<string, uint> rolewins = new Dictionary<string, uint>();

    private Dictionary<string, uint> roleloses = new Dictionary<string, uint>();

    private Dictionary<string, uint> gamemodestarts = new Dictionary<string, uint>();

    private Dictionary<string, uint> roleejects = new Dictionary<string, uint>();

    private Dictionary<string, uint> rolekills = new Dictionary<string, uint>();

	private Dictionary<string, uint> roleabilities = new Dictionary<string, uint>();



    public void AddKill(string rolename)
    {
        if (rolekills.ContainsKey(rolename))
        {
            rolekills[rolename]++;
        }
        else
        {
            rolekills.Add(rolename, 1);
        }
        SaveStats();
    }

	public void AddAbilityUse(string rolename)
	{
		if (roleabilities.ContainsKey(rolename))
		{
			roleabilities[rolename]++;
		}
		else
		{
			roleabilities.Add(rolename, 1);
		}
		SaveStats();
	}

	public void AddGameStart(string gamemodename)
	{
		if (gamemodestarts.ContainsKey(gamemodename))
		{
			gamemodestarts[gamemodename]++;
		}
		else
		{
			gamemodestarts.Add(gamemodename, 1);
		}
		SaveStats();
	}


    public void AddWin(string rolename)
    {
        if (rolewins.ContainsKey(rolename))
        {
            rolewins[rolename]++;
        }
        else
        {
            rolewins.Add(rolename, 1);
        }
        if (!roleloses.ContainsKey(rolename))
        {
            roleloses.Add(rolename, 0);
        }
        SaveStats();
    }

	public void AddEject(string rolename)
	{
		if (roleejects.ContainsKey(rolename))
		{
			roleejects[rolename]++;
		}
		else
		{
			roleejects.Add(rolename, 1);
		}
		SaveStats();
	}

	public void AddLose(string rolename)
	{
		if (roleloses.ContainsKey(rolename))
		{
			roleloses[rolename]++;
		}
		else
		{
			roleloses.Add(rolename, 1);
		}
        if (!rolewins.ContainsKey(rolename))
        {
            rolewins.Add(rolename, 0);
        }
		SaveStats();
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
	public uint Stalemates
	{
		get
		{
			LoadStats();
			return stalemates;
		}
		set
		{
			LoadStats();
			stalemates = value;
			SaveStats();
		}
	}

	public Dictionary<string, uint> RoleWins
    {
        get
        {
            LoadStats();
            return rolewins;
        }
        set
        {
            LoadStats();
            rolewins = value;
            SaveStats();
        }
    }

	public Dictionary<string, uint> AbilityUses
    {
        get
        {
            LoadStats();
            return roleabilities;
        }
        set
        {
            LoadStats();
            roleabilities = value;
            SaveStats();
        }
    }

	public Dictionary<string, uint> RoleKills
	{
		get
		{
			LoadStats();
			return rolekills;
		}
		set
		{
			LoadStats();
			rolekills = value;
			SaveStats();
		}
	}

	public Dictionary<string, uint> RoleEjects
	{
		get
		{
			LoadStats();
			return roleejects;
		}
		set
		{
			LoadStats();
			roleejects = value;
			SaveStats();
		}
	}

	public Dictionary<string, uint> GameStarts
	{
		get
		{
			LoadStats();
			return gamemodestarts;
		}
		set
		{
			LoadStats();
			rolewins = gamemodestarts;
			SaveStats();
		}
	}

	public Dictionary<string, uint> RoleLoses
	{
		get
		{
			LoadStats();
			return roleloses;
		}
		set
		{
			LoadStats();
			roleloses = value;
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

		public uint RevivedPlayers
	{
		get
		{
			LoadStats();
			return revivedplayers;
		}
		set
		{
			LoadStats();
			revivedplayers = value;
			SaveStats();
		}
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
			if (binaryReader.ReadByte() != StatsVersion)
            {
				File.Delete(path);
				return;
            }
			tasksCompleted = binaryReader.ReadUInt32();
			completedAllTasks = binaryReader.ReadUInt32();
			sabsFixed = binaryReader.ReadUInt32();
			timesMurdered = binaryReader.ReadUInt32();
                        gamesFinished = binaryReader.ReadUInt32();
			stalemates = binaryReader.ReadUInt32();
			rolewins = CE_BinaryExtensions.ReadStringUIntDictionary(binaryReader);
			roleloses = CE_BinaryExtensions.ReadStringUIntDictionary(binaryReader);
			gamemodestarts = CE_BinaryExtensions.ReadStringUIntDictionary(binaryReader);
			roleejects = CE_BinaryExtensions.ReadStringUIntDictionary(binaryReader);
			rolekills = CE_BinaryExtensions.ReadStringUIntDictionary(binaryReader);
			roleabilities = CE_BinaryExtensions.ReadStringUIntDictionary(binaryReader);
			revivedplayers = binaryReader.ReadUInt32();
		}
		catch
		{
			Debug.LogError("Deleting corrupted stats file");
			try
			{
				File.Delete(path);
			}
			catch
            {
				Debug.LogWarning("Deletion failed, most likely this is a debug session.");
            }
		}
	}

	protected virtual void SaveStats()
	{
		try
		{
			File.Delete(Path.Combine(Application.persistentDataPath, "playerStats2_ce"));
			using BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(Path.Combine(Application.persistentDataPath, "playerStats2_ce")));
			binaryWriter.Write(StatsVersion);
			binaryWriter.Write(tasksCompleted);
			binaryWriter.Write(completedAllTasks);
			binaryWriter.Write(sabsFixed);
			binaryWriter.Write(timesMurdered);
			binaryWriter.Write(gamesFinished);
			binaryWriter.Write(stalemates);
			CE_BinaryExtensions.WriteStringUIntDictionary(binaryWriter,rolewins);
			CE_BinaryExtensions.WriteStringUIntDictionary(binaryWriter,roleloses);
			CE_BinaryExtensions.WriteStringUIntDictionary(binaryWriter,gamemodestarts);
                        CE_BinaryExtensions.WriteStringUIntDictionary(binaryWriter, roleejects);
                        CE_BinaryExtensions.WriteStringUIntDictionary(binaryWriter, rolekills);
			CE_BinaryExtensions.WriteStringUIntDictionary(binaryWriter, roleabilities);
			binaryWriter.Write(revivedplayers);
			// Debug.Log("Stuff");
		}
		catch
		{
			Debug.Log("Failed to write out stats");
		}
	}

	public StatsManager()
	{
	}

	static StatsManager()
	{
		Instance = new StatsManager();
	}
}
