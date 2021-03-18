using System.Collections.Generic;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class CE_PlayerInfoLua
{

	public static explicit operator CE_PlayerInfoLua(GameData.PlayerInfo b) => new CE_PlayerInfoLua(b);

	public static explicit operator CE_PlayerInfoLua(PlayerControl b) => new CE_PlayerInfoLua(b.Data);
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


	public uint ColorId
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

    public byte luavalue1;

    public byte luavalue2;

	public byte luavalue3;

	public float PosX
	{
		get;
		private set;
	}
	public float PosY
	{
		get;
		private set;
	}

	public CE_PlayerInfoLua(GameData.PlayerInfo plf, bool revealimp = true)
	{
		PlayerId = plf.PlayerId;
		PlayerName = plf.PlayerName;
		ColorId = plf.ColorId;
		HatId = plf.HatId;
		SkinId = plf.SkinId;
		Disconnected = plf.Disconnected;
		Tasks = plf.Tasks;
		IsDead = plf.IsDead;
		IsImpostor = plf.IsImpostor && revealimp;
		role = plf.role;
        luavalue1 = plf.luavalue1;
        luavalue2 = plf.luavalue2;
		luavalue3 = plf.luavalue3;
        PosX = plf.Object.transform.position.x;
		PosY = plf.Object.transform.position.y;
		refplayer = plf;
	}
}
