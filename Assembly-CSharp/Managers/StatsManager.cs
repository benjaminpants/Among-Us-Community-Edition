using System;
using System.IO;
using UnityEngine;

// Token: 0x02000206 RID: 518
public class StatsManager
{
	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x06000B1F RID: 2847 RVA: 0x00008A8A File Offset: 0x00006C8A
	// (set) Token: 0x06000B20 RID: 2848 RVA: 0x00008A98 File Offset: 0x00006C98
	public uint BodiesReported
	{
		get
		{
			this.LoadStats();
			return this.bodiesReported;
		}
		set
		{
			this.LoadStats();
			this.bodiesReported = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x06000B21 RID: 2849 RVA: 0x00008AAD File Offset: 0x00006CAD
	// (set) Token: 0x06000B22 RID: 2850 RVA: 0x00008ABB File Offset: 0x00006CBB
	public uint EmergenciesCalled
	{
		get
		{
			this.LoadStats();
			return this.emergenciesCalls;
		}
		set
		{
			this.LoadStats();
			this.emergenciesCalls = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x06000B23 RID: 2851 RVA: 0x00008AD0 File Offset: 0x00006CD0
	// (set) Token: 0x06000B24 RID: 2852 RVA: 0x00008ADE File Offset: 0x00006CDE
	public uint TasksCompleted
	{
		get
		{
			this.LoadStats();
			return this.tasksCompleted;
		}
		set
		{
			this.LoadStats();
			this.tasksCompleted = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x06000B25 RID: 2853 RVA: 0x00008AF3 File Offset: 0x00006CF3
	// (set) Token: 0x06000B26 RID: 2854 RVA: 0x00008B01 File Offset: 0x00006D01
	public uint CompletedAllTasks
	{
		get
		{
			this.LoadStats();
			return this.completedAllTasks;
		}
		set
		{
			this.LoadStats();
			this.completedAllTasks = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06000B27 RID: 2855 RVA: 0x00008B16 File Offset: 0x00006D16
	// (set) Token: 0x06000B28 RID: 2856 RVA: 0x00008B24 File Offset: 0x00006D24
	public uint SabsFixed
	{
		get
		{
			this.LoadStats();
			return this.sabsFixed;
		}
		set
		{
			this.LoadStats();
			this.sabsFixed = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06000B29 RID: 2857 RVA: 0x00008B39 File Offset: 0x00006D39
	// (set) Token: 0x06000B2A RID: 2858 RVA: 0x00008B47 File Offset: 0x00006D47
	public uint ImpostorKills
	{
		get
		{
			this.LoadStats();
			return this.impostorKills;
		}
		set
		{
			this.LoadStats();
			this.impostorKills = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06000B2B RID: 2859 RVA: 0x00008B5C File Offset: 0x00006D5C
	// (set) Token: 0x06000B2C RID: 2860 RVA: 0x00008B6A File Offset: 0x00006D6A
	public uint TimesMurdered
	{
		get
		{
			this.LoadStats();
			return this.timesMurdered;
		}
		set
		{
			this.LoadStats();
			this.timesMurdered = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06000B2D RID: 2861 RVA: 0x00008B7F File Offset: 0x00006D7F
	// (set) Token: 0x06000B2E RID: 2862 RVA: 0x00008B8D File Offset: 0x00006D8D
	public uint TimesEjected
	{
		get
		{
			this.LoadStats();
			return this.timesEjected;
		}
		set
		{
			this.LoadStats();
			this.timesEjected = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06000B2F RID: 2863 RVA: 0x00008BA2 File Offset: 0x00006DA2
	// (set) Token: 0x06000B30 RID: 2864 RVA: 0x00008BB0 File Offset: 0x00006DB0
	public uint CrewmateStreak
	{
		get
		{
			this.LoadStats();
			return this.crewmateStreak;
		}
		set
		{
			this.LoadStats();
			this.crewmateStreak = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06000B31 RID: 2865 RVA: 0x00008BC5 File Offset: 0x00006DC5
	// (set) Token: 0x06000B32 RID: 2866 RVA: 0x00008BD3 File Offset: 0x00006DD3
	public uint TimesImpostor
	{
		get
		{
			this.LoadStats();
			return this.timesImpostor;
		}
		set
		{
			this.LoadStats();
			this.timesImpostor = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x06000B33 RID: 2867 RVA: 0x00008BE8 File Offset: 0x00006DE8
	// (set) Token: 0x06000B34 RID: 2868 RVA: 0x00008BF6 File Offset: 0x00006DF6
	public uint TimesCrewmate
	{
		get
		{
			this.LoadStats();
			return this.timesCrewmate;
		}
		set
		{
			this.LoadStats();
			this.timesCrewmate = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06000B35 RID: 2869 RVA: 0x00008C0B File Offset: 0x00006E0B
	// (set) Token: 0x06000B36 RID: 2870 RVA: 0x00008C19 File Offset: 0x00006E19
	public uint GamesStarted
	{
		get
		{
			this.LoadStats();
			return this.gamesStarted;
		}
		set
		{
			this.LoadStats();
			this.gamesStarted = value;
			this.SaveStats();
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x06000B37 RID: 2871 RVA: 0x00008C2E File Offset: 0x00006E2E
	// (set) Token: 0x06000B38 RID: 2872 RVA: 0x00008C3C File Offset: 0x00006E3C
	public uint GamesFinished
	{
		get
		{
			this.LoadStats();
			return this.gamesFinished;
		}
		set
		{
			this.LoadStats();
			this.gamesFinished = value;
			this.SaveStats();
		}
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x00008C51 File Offset: 0x00006E51
	public void AddWinReason(GameOverReason reason)
	{
		this.LoadStats();
		this.WinReasons[(int)reason] += 1U;
		this.SaveStats();
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x00008C70 File Offset: 0x00006E70
	public uint GetWinReason(GameOverReason reason)
	{
		this.LoadStats();
		return this.WinReasons[(int)reason];
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x00008C80 File Offset: 0x00006E80
	public void AddLoseReason(GameOverReason reason)
	{
		this.LoadStats();
		this.LoseReasons[(int)reason] += 1U;
		this.SaveStats();
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x00008C9F File Offset: 0x00006E9F
	public uint GetLoseReason(GameOverReason reason)
	{
		this.LoadStats();
		return this.LoseReasons[(int)reason];
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x00037F9C File Offset: 0x0003619C
	protected virtual void LoadStats()
	{
		if (this.loadedStats)
		{
			return;
		}
		this.loadedStats = true;
		string path = Path.Combine(Application.persistentDataPath, "playerStats2_ce");
		if (File.Exists(path))
		{
			try
			{
				using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
				{
					binaryReader.ReadByte();
					this.bodiesReported = binaryReader.ReadUInt32();
					this.emergenciesCalls = binaryReader.ReadUInt32();
					this.tasksCompleted = binaryReader.ReadUInt32();
					this.completedAllTasks = binaryReader.ReadUInt32();
					this.sabsFixed = binaryReader.ReadUInt32();
					this.impostorKills = binaryReader.ReadUInt32();
					this.timesMurdered = binaryReader.ReadUInt32();
					this.timesEjected = binaryReader.ReadUInt32();
					this.crewmateStreak = binaryReader.ReadUInt32();
					this.timesImpostor = binaryReader.ReadUInt32();
					this.timesCrewmate = binaryReader.ReadUInt32();
					this.gamesStarted = binaryReader.ReadUInt32();
					this.gamesFinished = binaryReader.ReadUInt32();
					for (int i = 0; i < this.WinReasons.Length; i++)
					{
						this.WinReasons[i] = binaryReader.ReadUInt32();
					}
					for (int j = 0; j < this.LoseReasons.Length; j++)
					{
						this.LoseReasons[j] = binaryReader.ReadUInt32();
					}
				}
			}
			catch
			{
				Debug.LogError("Deleting corrupted stats file");
				File.Delete(path);
			}
		}
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x00038104 File Offset: 0x00036304
	protected virtual void SaveStats()
	{
		try
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(Path.Combine(Application.persistentDataPath, "playerStats2_ce"))))
			{
				binaryWriter.Write(1);
				binaryWriter.Write(this.bodiesReported);
				binaryWriter.Write(this.emergenciesCalls);
				binaryWriter.Write(this.tasksCompleted);
				binaryWriter.Write(this.completedAllTasks);
				binaryWriter.Write(this.sabsFixed);
				binaryWriter.Write(this.impostorKills);
				binaryWriter.Write(this.timesMurdered);
				binaryWriter.Write(this.timesEjected);
				binaryWriter.Write(this.crewmateStreak);
				binaryWriter.Write(this.timesImpostor);
				binaryWriter.Write(this.timesCrewmate);
				binaryWriter.Write(this.gamesStarted);
				binaryWriter.Write(this.gamesFinished);
				for (int i = 0; i < this.WinReasons.Length; i++)
				{
					binaryWriter.Write(this.WinReasons[i]);
				}
				for (int j = 0; j < this.LoseReasons.Length; j++)
				{
					binaryWriter.Write(this.LoseReasons[j]);
				}
			}
		}
		catch
		{
			Debug.Log("Failed to write out stats");
		}
	}

	// Token: 0x04000AC2 RID: 2754
	public static StatsManager Instance = new StatsManager();

	// Token: 0x04000AC3 RID: 2755
	private const byte StatsVersion = 1;

	// Token: 0x04000AC4 RID: 2756
	private bool loadedStats;

	// Token: 0x04000AC5 RID: 2757
	private uint bodiesReported;

	// Token: 0x04000AC6 RID: 2758
	private uint emergenciesCalls;

	// Token: 0x04000AC7 RID: 2759
	private uint tasksCompleted;

	// Token: 0x04000AC8 RID: 2760
	private uint completedAllTasks;

	// Token: 0x04000AC9 RID: 2761
	private uint sabsFixed;

	// Token: 0x04000ACA RID: 2762
	private uint impostorKills;

	// Token: 0x04000ACB RID: 2763
	private uint timesMurdered;

	// Token: 0x04000ACC RID: 2764
	private uint timesEjected;

	// Token: 0x04000ACD RID: 2765
	private uint crewmateStreak;

	// Token: 0x04000ACE RID: 2766
	private uint timesImpostor;

	// Token: 0x04000ACF RID: 2767
	private uint timesCrewmate;

	// Token: 0x04000AD0 RID: 2768
	private uint gamesStarted;

	// Token: 0x04000AD1 RID: 2769
	private uint gamesFinished;

	// Token: 0x04000AD2 RID: 2770
	private uint[] WinReasons = new uint[7];

	// Token: 0x04000AD3 RID: 2771
	private uint[] LoseReasons = new uint[7];
}
