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
        throw new NotImplementedException(); // TODO: implement this
    }


    public static bool UpdatePlayerInfo(DynValue dynval)
    {
        CE_PlayerInfoLua influa = (CE_PlayerInfoLua)dynval.UserData.Object;
        Debug.Log("Attempting Data Change for:" + influa.PlayerName);
        influa.refplayer.Object.RpcSetColor(influa.ColorId);
        influa.refplayer.Object.RpcSetSkin(influa.SkinId);
        influa.refplayer.Object.RpcSetHat(influa.HatId);
        influa.refplayer.Object.RpcSetName(influa.PlayerName);
        return true;
    }


    public static bool DebugLogLua(string text)
    {
        Debug.Log(text); //debug log
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
                byte infolua = CE_RoleManager.GetRoleFromName(ply.String);
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

    public static List<CE_PlayerInfoLua> GetAllPlayers()
    {
        List<CE_PlayerInfoLua> PlayFoLua = new List<CE_PlayerInfoLua>();
        foreach (GameData.PlayerInfo plrfo in GameData.Instance.AllPlayers)
        {
            PlayFoLua.Add(new CE_PlayerInfoLua(plrfo));
        }
        return PlayFoLua;
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
    public static bool CreateRoleComplex(string Name, Table Color, string RoleText, List<CE_Specials> Specials, CE_WinWith Win, CE_RoleVisibility Vis, bool ImpVis)
    {
        Color rolcolr = new Color((float)Color.Get(1).Number / 255f, (float)Color.Get(2).Number / 255f, (float)Color.Get(3).Number / 255f);
        return CE_RoleManager.AddRole(new CE_Role(Name, rolcolr, RoleText,Specials,Win,Vis,ImpVis));
    }
}
