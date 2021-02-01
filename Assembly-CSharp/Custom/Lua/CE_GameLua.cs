using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using UnityEngine;
using System.Linq;
using System;
using Hazel;


static class CE_GameLua
{
    public static void ActivateWin()
    {
        throw new NotImplementedException(); // TODO: implement this, seriously please
    }

    public static bool CheckIfInVent(CE_PlayerInfoLua plfo)
    {
        return plfo.refplayer.Object.inVent;
    }

    public static CE_PlayerInfoLua GetLocal()
    {
        return (CE_PlayerInfoLua)PlayerControl.LocalPlayer;
    }

    public static CE_PlayerInfoLua GetHost()
    {
        return (CE_PlayerInfoLua)GameData.Instance.GetHost();
    }

    public static bool SabSystem(string sysname, CE_PlayerInfoLua plfo, bool fix, int fixoverride = -1)
    {
        byte sab;
        SystemTypes systype;
        switch (sysname)
        {
            case "Reactor":
                sab = 3;
                systype = SystemTypes.Reactor;
                break;
            case "Oxy":
                sab = 8;
                systype = SystemTypes.LifeSupp;
                break;
            case "Comms":
                sab = 14;
                systype = SystemTypes.Comms;
                break;
            case "Lights":
                sab = 8;
                systype = SystemTypes.Electrical;
                break;
            default:
                Debug.LogError("Unknown System Type:" + sysname);
                return false;
        }
        if (!fix && systype != SystemTypes.Electrical)
        {
            SabotageSystemType.RepairDamageStatic(plfo.refplayer.Object, sab);
        }
        else if (fixoverride != -1)
        {
            ShipStatus.Instance.RepairSystem(systype, plfo.refplayer.Object, (byte)fixoverride);
        }
        else
        {
            ShipStatus.Instance.RepairSystem(systype, plfo.refplayer.Object, 0);
        }
        return true;
    }

    public static bool ShowCLMessage(string msg)
    {
        DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(msg);
        return true;
    }

    public static bool ClearCLMessage()
    {
        DestroyableSingleton<HudManager>.Instance.Notifier.ClearText();
        return true;
    }
    public static bool UpdatePlayerInfo(CE_PlayerInfoLua influa)
    {
        //Debug.Log("Attempting Data Change for:" + influa.PlayerName);
        influa.refplayer.Object.RpcSetColor(influa.ColorId);
        if (influa.refplayer.SkinId != influa.SkinId)
        {
            influa.refplayer.Object.RpcSetSkin(influa.SkinId);
        }
        if (influa.refplayer.HatId != influa.HatId)
        {
            influa.refplayer.Object.RpcSetHat(influa.HatId);
        }
        if (influa.refplayer.luavalue1 != influa.luavalue1)
        {
            influa.refplayer.Object.RpcSetLuaValue(influa.luavalue1, 1);
        }
        if (influa.refplayer.luavalue1 != influa.luavalue2)
        {
            influa.refplayer.Object.RpcSetLuaValue(influa.luavalue2, 2);
        }
        if (influa.refplayer.luavalue1 != influa.luavalue3)
        {
            influa.refplayer.Object.RpcSetLuaValue(influa.luavalue3, 3);
        }
        influa.refplayer.Object.RpcSetName(influa.PlayerName);
        return true;
    }


    public static bool DebugLogLua(string text)
    {
        Debug.Log(text); //debug log
        return true;
    }
    public static bool DebugErrorLua(string text)
    {
        Debug.LogError(text); //debug log
        return true;
    }


    public static CE_LuaSpawnableObject CreateObject(string id)
    {
        if (id == "DeadBody")
        {
            return new CE_LuaDeadBody();
        }
        return new CE_LuaSpawnableObject();
    }

    public static bool DoThing()
    {
        SendObject(new CE_LuaSpawnableObject());
        return true;
    }
    public static bool GameStarted()
    {
        return (InnerNet.InnerNetServer.Instance.CurrentGState() == InnerNet.GameStates.Started);
    }

    public static bool SendToHostSimple(byte id)
    {
        PlayerControl.LocalPlayer.RpcSendUpdate(id);
        return true;
    }

    public static bool SetRoles(Table plyrs, Table roles)
    {
        try
        {
            List<GameData.PlayerInfo> playerinfos = new List<GameData.PlayerInfo>();
            foreach (DynValue ply in plyrs.Values)
            {
                CE_PlayerInfoLua infolua = (CE_PlayerInfoLua)ply.UserData.Object;
                playerinfos.Add(infolua.refplayer);
            }
            List<byte> role = new List<byte>();
            foreach (DynValue ply in roles.Values)
            {
                byte infolua = CE_RoleManager.GetRoleFromUUID(ply.String);
                role.Add(infolua);
            }
            PlayerControl.LocalPlayer.RpcSetRole(playerinfos.ToArray(), role.ToArray());
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    public static float GetNumber(byte id)
    {
        if (CE_LuaLoader.CurrentSettings[id] == null)
        {
            return 0f;
        }
        if (CE_LuaLoader.CurrentSettings[id].IsNumber)
        {
            return CE_LuaLoader.CurrentSettings[id].NumValue;
        }
        return 0f;
    }

    public static string GetRoleNameFromId(byte id)
    {
        return CE_RoleManager.GetRoleFromID(id).RoleName;
    }

    public static bool CallMeeting(CE_PlayerInfoLua caller, CE_PlayerInfoLua target = null)
    {
        caller.refplayer.Object.CmdReportDeadBody(target.refplayer);
        return true;
    }

    public static bool Revive(CE_PlayerInfoLua player)
    {
        PlayerControl.LocalPlayer.RpcRevive(player.refplayer);
        return true;
    }

    public static bool GetBool(byte id)
    {
        if (CE_LuaLoader.CurrentSettings[id] == null)
        {
            return false;
        }
        if (CE_LuaLoader.CurrentSettings[id].DataType == CE_OptDataTypes.Toggle)
        {
            return CE_ConversionHelpers.FloatToBool(CE_LuaLoader.CurrentSettings[id].NumValue);
        }
        return false;
    }

    public static bool KillPlayer(CE_PlayerInfoLua pl,bool showplayer)
    {
        PlayerControl.LocalPlayer.RpcMurderPlayer(pl.refplayer.Object,showplayer);
        return true;
    }

    public static bool SabDoor(byte system)
    {
        ShipStatus.Instance.RpcCloseDoorsOfType((SystemTypes)system);
        return true;
    }

    public static bool AmHost()
    {
        return AmongUsClient.Instance.AmHost;
    }
    public static bool ActivateCustomWin(Table plyrs, string song)
    {
        try
        {
            List<GameData.PlayerInfo> playerinfos = new List<GameData.PlayerInfo>();
            foreach (DynValue ply in plyrs.Values)
            {
                CE_PlayerInfoLua infolua = (CE_PlayerInfoLua)ply.UserData.Object;
                playerinfos.Add(infolua.refplayer);
            }
            ShipStatus.RpcCustomEndGamePublic(playerinfos.ToArray(), song);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
        return true;
    }

    public static bool SnapPlayerToPos(float x, float y, CE_PlayerInfoLua pllua)
    {
        pllua.refplayer.Object.NetTransform.RpcSnapTo(new Vector2(x,y));
        return true;
    }

    public static bool SendObject(CE_LuaSpawnableObject obj)
    {
        ShipStatus.WriteRPCObjectPublic(obj);
        return true;
    }


    public static List<CE_PlayerInfoLua> GetAllPlayers()
    {
        List<CE_PlayerInfoLua> PlayFoLua = new List<CE_PlayerInfoLua>();
        foreach (GameData.PlayerInfo plrfo in GameData.Instance.AllPlayers)
        {
            if (!plrfo.Disconnected)
            {
                PlayFoLua.Add((CE_PlayerInfoLua)plrfo);
            }
        }
        return PlayFoLua;
    }

    public static uint GetHatIDFromProductID(string id)
    {
        foreach (HatBehaviour hat in HatManager.Instance.AllHats)
        {
            if (hat.ProdId == id)
            {
                return (uint)HatManager.Instance.AllHats.FindIndex(a => a == hat);
            }
        }
        return 0;
    }


    public static List<CE_PlayerInfoLua> GetAllPlayersComplex(bool alive, bool canbeimp)
    {
        List<CE_PlayerInfoLua> PlayFoLua = new List<CE_PlayerInfoLua>();
        foreach (GameData.PlayerInfo plrfo in GameData.Instance.AllPlayers)
        {
            if ((!plrfo.IsDead || alive) && (!plrfo.IsImpostor || canbeimp))
            {
                PlayFoLua.Add(new CE_PlayerInfoLua(plrfo));
            }
        }
        return PlayFoLua;
    }

   
    public static bool CreateRoleSimple(string Name, Table Color, string RoleText)
    {
        Color rolcolr = new Color((float)Color.Get(1).Number / 255f, (float)Color.Get(2).Number / 255f, (float)Color.Get(3).Number / 255f);
        return CE_RoleManager.AddRole(new CE_Role(Name, rolcolr, RoleText));
    }
    public static bool CreateRoleComplex(string Name, Table Color, string RoleText, List<CE_Specials> Specials, CE_WinWith Win, CE_RoleVisibility Vis, bool ImpVis, bool dotask = true, byte layer = 0, bool canseeimp = false, string faketask = "")
    {
        Color rolcolr = new Color((float)Color.Get(1).Number / 255f, (float)Color.Get(2).Number / 255f, (float)Color.Get(3).Number / 255f);
        return CE_RoleManager.AddRole(new CE_Role(Name, rolcolr, RoleText,Specials,Win,Vis,ImpVis,dotask,layer,canseeimp,faketask));
    }
}
