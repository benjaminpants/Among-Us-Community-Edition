using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using UnityEngine;
using System.Linq;
using System;

public static class CE_LuaLoader
{
	public static Dictionary<byte, CE_GamemodeInfo> GamemodeInfos;

	public static bool CurrentGMLua => GameOptionsData.GamemodesAreLua[PlayerControl.GameOptions.Gamemode];

	public static void LoadLua()
	{
		UserData.RegisterAssembly();
		new List<string>();
		FileInfo[] files = new DirectoryInfo(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Lua"), "Gamemodes")).GetFiles("*.lua");
		for (int i = 0; i < files.Length; i++)
		{
			using StreamReader streamReader = files[i].OpenText();
			string code = streamReader.ReadToEnd();
			Script script = new Script();
            script.Globals["Game_ActivateCustomWin"] = (Func<Table, string, bool>)CE_GameLua.ActivateCustomWin;
			script.DoString(code);
			Table table = script.Call(script.Globals["InitializeGamemode"]).Table;
			byte b = (byte)table.Get(2).Number;
            string gamemodename = table.Get(1).String;
			gamemodename = new string((from c in gamemodename
			where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
			select c
			).ToArray()); //Removes non letter or number characters from Gamemode name
			Debug.Log("Loaded Gamemode: " + gamemodename);
			CE_GamemodeInfo value = new CE_GamemodeInfo(gamemodename, script, b);
			GamemodeInfos.Add(b, value);
		}
	}

	static CE_LuaLoader()
	{
		GamemodeInfos = new Dictionary<byte, CE_GamemodeInfo>();
	}

	public static DynValue GetGamemodeResult(string fn, params object[] obj)
	{
		GamemodeInfos.TryGetValue((byte)(PlayerControl.GameOptions.Gamemode + 1), out var value);
		Script script = value.script;
		return script.Call(script.Globals[fn], obj);
	}
}