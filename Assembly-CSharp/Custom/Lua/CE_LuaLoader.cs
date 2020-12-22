using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using UnityEngine;
using System.Linq;
using System;

public static class CE_LuaLoader
{
	public static Dictionary<byte, CE_GamemodeInfo> GamemodeInfos;


	public static string TheOmegaString;

	public static int TheOmegaHash;
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
			TheOmegaString += code; //nothing is done with this atm
			Script script = new Script();
            script.Globals["Game_ActivateCustomWin"] = (Func<Table, string, bool>)CE_GameLua.ActivateCustomWin;
            script.Globals["Game_GetAllPlayers"] = (Func<List<CE_PlayerInfoLua>>)CE_GameLua.GetAllPlayers; //TODO: Automate the adding of functions
			script.Globals["Game_GetAllPlayersComplex"] = (Func<bool,bool,List<CE_PlayerInfoLua>>)CE_GameLua.GetAllPlayersComplex;
			script.Globals["Game_CreateRoleSimple"] = (Func<string, Table, string, bool>)CE_GameLua.CreateRoleSimple;
            script.Globals["Game_CreateRole"] = (Func<string, Table, string, List<CE_Specials>, CE_WinWith, CE_RoleVisibility, bool, bool>)CE_GameLua.CreateRoleComplex;
			script.Globals["Game_GetRoleIDFromName"] = (Func<string, byte>)CE_RoleManager.GetRoleFromName;
            script.Globals["Game_UpdatePlayerInfo"] = (Func<DynValue, bool>)CE_GameLua.UpdatePlayerInfo;
			script.Globals["Game_SetRoles"] = (Func<Table,Table, bool>)CE_GameLua.SetRoles;
            script.Globals["Net_SendMessageToHostSimple"] = (Func<byte, bool>)CE_GameLua.SendToHostSimple;
			script.Globals["Net_AmHost"] = (Func<bool>)CE_GameLua.AmHost;
			script.Globals["Debug_Log"] = (Func<string,bool>)CE_GameLua.DebugLogLua;
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
		TheOmegaHash = TheOmegaString.GetHashCode();
	}

	static CE_LuaLoader()
	{
		GamemodeInfos = new Dictionary<byte, CE_GamemodeInfo>();
	}

	public static DynValue GetGamemodeResult(string fn, params object[] obj)
	{
		if (GamemodeInfos.TryGetValue((byte)(PlayerControl.GameOptions.Gamemode + 1), out var value))
		{
			Script script = value.script;
			try
			{
				return script.Call(script.Globals[fn], obj);
			}
			catch(Exception E)
            {
				Debug.LogError(E.Message + "\nUnable to find function:" + fn + "\nAttempting to call function in base lua...");
                if (GamemodeInfos.TryGetValue(1, out var value2))
                {
					try
					{
						return value2.script.Call(script.Globals[fn], obj);
					}
					catch(Exception E2)
                    {
						Debug.LogError(E2.Message + "\nScript 1 doesn't have a definition for this function!\nReturning new DynValue...");
						return new DynValue();
					}
				}
                else
                {
                    Debug.LogError("No script with base ID 1... THIS SHOULDN'T BE POSSIBLE");
                }
				return new DynValue();
			}
		}
		else
        {
			Debug.LogError("Current Gamemode marked as Lua even though it isn't!\nGamemode ID(Raw):" + PlayerControl.GameOptions.Gamemode + "\nGamemode ID:" + (PlayerControl.GameOptions.Gamemode + 1));
			return new DynValue();
        }
	}
}