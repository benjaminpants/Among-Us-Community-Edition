using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class CE_PlayerInfoLua
{

	public static explicit operator CE_PlayerInfoLua(GameData.PlayerInfo b) => new CE_PlayerInfoLua(b);

	public static explicit operator CE_PlayerInfoLua(PlayerControl b) => new CE_PlayerInfoLua(b.Data);
	[MoonSharpHidden]
	public GameData.PlayerInfo refplayer;

	[MoonSharpHidden]
	public static GameData.PlayerInfo ShitHolder = new GameData.PlayerInfo(0);

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


	public int ColorId
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
		if (plf == null)
        {
			PlayerId = 0;
			PlayerName = "NULL";
			ColorId = 0;
			HatId = 0;
			SkinId = 0;
			Disconnected = false;
			Tasks = new List<GameData.TaskInfo>();
			IsDead = true;
			IsImpostor = false;
			role = 0;
			luavalue1 = 0;
			luavalue2 = 0;
			luavalue3 = 0;
			PosX = 0f;
			PosY = 0f;
			refplayer = ShitHolder;
			Debug.LogWarning("PL is null! Using Placeholder refplayer... hope shitholder doesn't actually get edited...");
			return;
        }
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
