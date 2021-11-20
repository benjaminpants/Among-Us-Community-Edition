using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using UnityEngine;
using System.Linq;
using System;

public static class CE_LuaLoader
{
	public static List<CE_GamemodeInfo> GamemodeInfos = new List<CE_GamemodeInfo>();

    public static List<CE_PluginInfo> PluginInfos = new List<CE_PluginInfo>();

	public static Dictionary<CE_GamemodeInfo, List<CE_CustomLuaSetting>> CustomGMSettings = new Dictionary<CE_GamemodeInfo, List<CE_CustomLuaSetting>>();

	public static string TheOmegaString;

	public static bool IsInitializing;

	public static string CurrentGMName;

	private static List<CE_CustomLuaSetting> TempSetting = new List<CE_CustomLuaSetting>();

	private static CE_Language TempLang;

	public static int TheOmegaHash;

    public static bool CurrentGMLua => (GameOptionsData.GamemodesAreLua[PlayerControl.GameOptions.Gamemode] && (!DestroyableSingleton<TutorialManager>.InstanceExists)); //this should always be true outside of freeplay if it isn't i will ask how the fuck because there are legit 0 non-lua gamemodes now

	public static CE_GamemodeInfo CurrentGM => GamemodeInfos[PlayerControl.GameOptions.Gamemode];
	public static List<CE_CustomLuaSetting> CurrentSettings => CustomGMSettings.GetValueOrDefault(GamemodeInfos[PlayerControl.GameOptions.Gamemode]);


	private static bool AddLangEntry(string key, string text)
    {
        return TempLang.AddEntry(key, text);
    }

    private static bool TempAddCustomByteSetting(string name, byte min, byte max, byte def, byte inc = 1)
    {
        TempSetting.Add(new CE_CustomLuaSetting(min, max, def, name, inc));
        return true;
    }

    private static bool TempAddCustomIntSetting(string name, int min, int max, int def, int inc = 1)
    {
        TempSetting.Add(new CE_CustomLuaSetting(min, max, def, name, inc));
        return true;
    }

    private static bool TempAddCustomFloatSetting(string name, float min, float max, float def, float inc = 1)
    {
        TempSetting.Add(new CE_CustomLuaSetting(min, max, def, name, inc));
        return true;
    }

    private static bool TempAddCustomStringSetting(string name, string def)
    {
        TempSetting.Add(new CE_CustomLuaSetting(def, name));
        return true;
    }

	private static bool TempAddCustomBoolSetting(string name, bool def)
	{
		TempSetting.Add(new CE_CustomLuaSetting(name,def));
		return true;
	}

	private static void GiveAPICalls(Script script,bool isgm = true)
	{
        script.Globals["Game_ActivateCustomWin"] = (Func<Table, string, bool>)CE_GameLua.ActivateCustomWin;
		script.Globals["Game_ActivateCustomRolesWin"] = (Func<Table, string, bool>)CE_GameLua.ActivateWinForRoles;
		script.Globals["Game_GetAllPlayers"] = (Func<List<CE_PlayerInfoLua>>)CE_GameLua.GetAllPlayers; //TODO: Automate the adding of functions
		script.Globals["Game_GetAllPlayersComplex"] = (Func<bool, bool, List<CE_PlayerInfoLua>>)CE_GameLua.GetAllPlayersComplex;
		if (isgm)
		{
			script.Globals["Game_CreateRoleSimple"] = (Func<string, Table, string, bool>)CE_GameLua.CreateRoleSimple;
			script.Globals["Game_CreateRole"] = (Func<string, Table, string, List<CE_Specials>, CE_WinWith, CE_RoleVisibility, bool, bool, byte, bool, string, bool>)CE_GameLua.CreateRoleComplex;
		}
		script.Globals["Game_GetRoleIDFromName"] = (Func<string, byte>)CE_RoleManager.GetRoleFromName;
		script.Globals["Game_GetRoleIDFromUUID"] = (Func<string, byte>)CE_RoleManager.GetRoleFromUUID;
		script.Globals["Game_UpdatePlayerInfo"] = (Func<CE_PlayerInfoLua, bool>)CE_GameLua.UpdatePlayerInfo;
		script.Globals["Game_SetRoles"] = (Func<Table, Table, bool>)CE_GameLua.SetRoles;
        script.Globals["Game_GetHatIDFromProductID"] = (Func<string, uint>)CE_GameLua.GetHatIDFromProductID;
		script.Globals["Game_GetSkinIDFromProductID"] = (Func<string, uint>)CE_GameLua.GetSkinIDFromProductID;
		script.Globals["Client_ShowMessage"] = (Func<string, bool>)CE_GameLua.ShowCLMessage;
		script.Globals["Vector3_Lerp"] = (Func<Vector3,Vector3,float>) Vector3.Lerp;
        script.Globals["Client_ClearMessages"] = (Func<bool>)CE_GameLua.ClearCLMessage;
        script.Globals["Game_CallMeeting"] = (Func<CE_PlayerInfoLua, CE_PlayerInfoLua, bool>)CE_GameLua.CallMeeting;
		script.Globals["Game_GetGlobalNum"] = (Func<string,int>)CE_GameLua.GetGlobalValue;
		script.Globals["Game_SabSystem"] = (Func<string, CE_PlayerInfoLua, bool, int, bool>)CE_GameLua.SabSystem;
		script.Globals["Player_SnapPosTo"] = (Func<float, float, CE_PlayerInfoLua, bool>)CE_GameLua.SnapPlayerToPos;
        script.Globals["Net_InGame"] = (Func<bool>)CE_GameLua.GameStarted;
		script.Globals["Net_GetHost"] = (Func<CE_PlayerInfoLua>)CE_GameLua.GetHost;
        script.Globals["Net_SendMessageToHostSimple"] = (Func<byte, bool>)CE_GameLua.SendToHostSimple;
		script.Globals["Net_SendMessageToHost"] = (Func<byte,Table, bool>)CE_GameLua.SendToHostComplex;
		script.Globals["Net_AmHost"] = (Func<bool>)CE_GameLua.AmHost;
		script.Globals["Debug_Log"] = (Func<string, bool>)CE_GameLua.DebugLogLua;
        script.Globals["Debug_Error"] = (Func<string, bool>)CE_GameLua.DebugErrorLua;
		script.Globals["Game_ActivateWin"] = (Func<string,bool>)CE_GameLua.ActivateWin;
		if (isgm)
		{
			script.Globals["UI_AddLangEntry"] = (Func<string, string, bool>)AddLangEntry;
		}
        script.Globals["Game_CheckPlayerInVent"] = (Func<CE_PlayerInfoLua, bool>)CE_GameLua.CheckIfInVent;
        script.Globals["Client_GetLocalPlayer"] = (Func<CE_PlayerInfoLua>)CE_GameLua.GetLocal;
        script.Globals["Game_KillPlayer"] = (Func<CE_PlayerInfoLua, bool, bool>)CE_GameLua.KillPlayer;
		script.Globals["Game_SabDoors"] = (Func<byte, bool>)CE_GameLua.SabDoor;
		//script.Globals["Game_RevivePlayer"] = (Func<CE_PlayerInfoLua, bool>)CE_GameLua.Revive;
		if (isgm)
		{
			script.Globals["Settings_CreateByte"] = (Func<string, byte, byte, byte, byte, bool>)TempAddCustomByteSetting;
			script.Globals["Settings_CreateInt"] = (Func<string, int, int, int, int, bool>)TempAddCustomIntSetting;
			script.Globals["Settings_CreateFloat"] = (Func<string, float, float, float, float, bool>)TempAddCustomFloatSetting;
			script.Globals["Settings_CreateBool"] = (Func<string, bool, bool>)TempAddCustomBoolSetting;
		}
        script.Globals["Settings_GetNumber"] = (Func<byte, float>)CE_GameLua.GetNumber;
		script.Globals["Game_GetPlayerFromID"] = (Func<byte, CE_PlayerInfoLua>)CE_GameLua.GetPlayerFromID;
        script.Globals["Game_GetRoleNameFromID"] = (Func<byte, string>)CE_GameLua.GetRoleNameFromId;
        script.Globals["Game_GetColorFromName"] = (Func<string, uint>)CE_GameLua.GetColorFromName;
		script.Globals["Misc_GetCurrentTime"] = (Func<long>)CE_GameLua.GetCurTimeInMS;
		script.Globals["Settings_GetBool"] = (Func<byte, bool>)CE_GameLua.GetBool;
	}


	private static void LoadGamemodes(string path)
    {
		FileInfo[] files = new DirectoryInfo(Path.Combine(path, "Gamemodes")).GetFiles("*.lua");
		for (int i = 0; i < files.Length; i++)
		{
			using StreamReader streamReader = files[i].OpenText();
			string code = streamReader.ReadToEnd();
			TheOmegaString += code;
			try
			{
				CurrentGMName = files[i].Name.Remove(files[i].Name.Length - 4);
				Script script = new Script();
				TempLang = new CE_Language(true);
				TempSetting = new List<CE_CustomLuaSetting>();
				GiveAPICalls(script, true);
				script.DoString(code);
				Table table = script.Call(script.Globals["InitializeGamemode"]).Table;
				byte b = (byte)table.Get(2).Number;
				CE_LanguageManager.AddGMLanguage(CurrentGMName, TempLang);
				string gamemodename = table.Get(1).String;
				gamemodename = new string((from c in gamemodename
										   where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
										   select c
				).ToArray()); //Removes non letter or number characters from Gamemode name
				Debug.Log("Loaded Gamemode: " + gamemodename);
				CE_GamemodeInfo value = new CE_GamemodeInfo(gamemodename, script, b,CurrentGMName);
				CustomGMSettings.Add(value, TempSetting);
				GamemodeInfos.Add(value);
			}
			catch (Exception E)
			{
                Debug.LogError("Error encountered when trying to load gamemode with filename:" + files[i].Name + "\nException Message:" + E.Message);
				CE_ModErrorUI.AddError(new CE_Error("Error encountered when trying to load gamemode with filename: " + files[i].Name + "\nException Message:" + E.Message, "The gamemode will not be enabled.", ErrorTypes.Error));
			}
		}
		GamemodeInfos = GamemodeInfos.OrderBy(o => o.id).ToList();
	}

	private static void LoadPlugins(string path)
    {
		FileInfo[] plfiles = new DirectoryInfo(Path.Combine(path, "Plugins")).GetFiles("*.lua");
		for (int i = 0; i < plfiles.Length; i++)
		{
			using StreamReader streamReader = plfiles[i].OpenText();
			string code = streamReader.ReadToEnd();
			TheOmegaString += code; //nothing is done with this atm
			try
			{
				CurrentGMName = plfiles[i].Name.Remove(plfiles[i].Name.Length - 4);
				Script script = new Script();
				TempLang = new CE_Language(false);
				GiveAPICalls(script, false);
				script.DoString(code);
				Table table = script.Call(script.Globals["InitializePlugin"]).Table;
				byte b = (byte)table.Get(2).Number;
				string gamemodename = table.Get(1).String;
				bool isprior = table.Get(3).Boolean;
				gamemodename = new string((from c in gamemodename
										   where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
										   select c
				).ToArray()); //Removes non letter or number characters from Gamemode name
				Debug.Log("Loaded Plugin: " + gamemodename);
				CE_PluginInfo value = new CE_PluginInfo(gamemodename, script, b, isprior);
				PluginInfos.Add(value);
			}
			catch (Exception E)
			{
				Debug.LogError("Error encountered when trying to load plugin with filename:" + plfiles[i].Name + "\nException Message:" + E.Message);
				CE_ModErrorUI.AddError(new CE_Error("Error encountered when trying to load plugin with filename: " + plfiles[i].Name + "\nException Message:" + E.Message, "The plugin will not be enabled.", ErrorTypes.Error));
			}
		}
		PluginInfos = PluginInfos.OrderBy(o => o.id).ToList();
	}
	public static void LoadLua(string path)
	{
		UserData.RegisterAssembly();
		IsInitializing = true;
        if (Directory.Exists(Path.Combine(path, "Gamemodes")))
        {
            LoadGamemodes(path);
        }
		if (Directory.Exists(Path.Combine(path, "Plugins")))
		{
			LoadPlugins(path);
		}

		CurrentGMName = null;
		IsInitializing = false;
		TheOmegaHash = VersionShower.GetDeterministicHashCode(TheOmegaString);
	}

	public static bool IsPluginAnOverride(byte id)
	{
		if (PluginInfos[id] != null)
		{
			return PluginInfos[id].highprior;
		}
		else
		{
			return false;
		}
	}

	public static List<CE_PluginInfo> GetEnabledPlugins()
    {
		List<CE_PluginInfo> PlugFoList = new List<CE_PluginInfo>();
		for (int i=0; i <= PlayerControl.GameOptions.Plugins.Count - 1; i++)
        {
			CE_PluginInfo fo = PluginInfos[PlayerControl.GameOptions.Plugins[i]];
			PlugFoList.Add(fo);
        }
		return PlugFoList;
    }

	public static DynValue GetGamemodeResult(string fn, params object[] obj)
	{
		if (GamemodeInfos[PlayerControl.GameOptions.Gamemode] != null)
		{
			CE_GamemodeInfo value = GamemodeInfos[PlayerControl.GameOptions.Gamemode];
			Script script = value.script;
			DynValue dumbvalue = new DynValue();
			bool ExecOG = true;
			foreach (CE_PluginInfo plgf in GetEnabledPlugins()) //if there are any override plugins, set execOG to false, cancelling the running of the original function
			{
                if (plgf.highprior && plgf.script.Globals[fn] != null)
                {
                    ExecOG = false;
                }
			}
			try
			{
				if (ExecOG)
				{
					dumbvalue = script.Call(script.Globals[fn], obj);
				}
			}
			catch(Exception E)
            {
				if (script.Globals[fn] != null)
				{
					Debug.LogWarning(E.Message + "\nStack trace:" + E.StackTrace + "\nUnable to find valid function:" + fn + "\nAttempting to call function in base lua...");
				}
                if (GamemodeInfos[0] != null)
                {
					try
					{
						if (GamemodeInfos[0].name != "Classic")
                        {
							throw new Exception("Tampering Attempted!");
                        }						
						dumbvalue = GamemodeInfos[0].script.Call(GamemodeInfos[0].script.Globals[fn], obj);
					}
					catch(Exception E2)
                    {
						Debug.LogError(E2.Message + "\nScript 1 doesn't have a definition for this function!\nReturning new DynValue...");
					}
				}
                else
                {
                    Debug.LogError("No script with base ID 1... THIS SHOULDN'T BE POSSIBLE");
                }
			}
			foreach (CE_PluginInfo plgf in GetEnabledPlugins()) //for every plugin, if its just a normal plugin, run it, otherwise try to return the first successful function output
			{
				if (!plgf.highprior)
                {
					try
					{
						plgf.script.Call(plgf.script.Globals[fn], obj);
					}
					catch
                    {

                    }
                }
				else
                {
					try
                    {
						if (plgf.script.Globals[fn] != null)
						{
							return plgf.script.Call(plgf.script.Globals[fn], obj);
						}
					}
					catch
					{
						//carry on
					}
                }
			}
			return dumbvalue;
		}
		else
        {
			Debug.LogError("Current Gamemode marked as Lua even though it isn't!\nGamemode ID(Raw):" + PlayerControl.GameOptions.Gamemode + "\nGamemode ID:" + (PlayerControl.GameOptions.Gamemode + 1));
			return new DynValue();
        }
	}
}
