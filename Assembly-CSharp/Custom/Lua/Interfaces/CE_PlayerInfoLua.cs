using System.Collections.Generic;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class CE_PlayerInfoLua
{
	[MoonSharpHidden]
	public GameData.PlayerInfo refplayer;

	public byte PlayerId
	{
		get;
		private set;
	}

	public string PlayerName
	{
		get;
		set;
	} = string.Empty;


	public byte ColorId
	{
		get;
		set;
	}

	public uint HatId
	{
		get;
		set;
	}

	public uint SkinId
	{
		get;
		set;
	}

	public bool Disconnected
	{
		get;
		private set;
	}

	public List<GameData.TaskInfo> Tasks
	{
		get;
		private set;
	}

	public bool IsImpostor
	{
		get;
		private set;
	}

	public bool IsDead
	{
		get;
		private set;
	}

	public byte role
	{
		get;
		private set;
	}

	public CE_PlayerInfoLua()
	{
		throw new System.Exception("Attempted to create a PlayerInfo without a reference!");
	}

	public CE_PlayerInfoLua(GameData.PlayerInfo plf)
	{
		PlayerId = plf.PlayerId;
		PlayerName = plf.PlayerName;
		ColorId = plf.ColorId;
		HatId = plf.HatId;
		SkinId = plf.SkinId;
		Disconnected = plf.Disconnected;
		Tasks = plf.Tasks;
		IsDead = plf.IsDead;
		IsImpostor = plf.IsImpostor;
		role = plf.role;
		refplayer = plf;
	}
}
