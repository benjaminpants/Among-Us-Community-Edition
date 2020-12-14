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
		private set;
	} = string.Empty;


	public byte ColorId
	{
		get;
		private set;
	}

	public uint HatId
	{
		get;
		private set;
	}

	public uint SkinId
	{
		get;
		private set;
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

	public GameData.PlayerInfo.Role role
	{
		get;
		private set;
	}

	public CE_PlayerInfoLua()
	{
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
