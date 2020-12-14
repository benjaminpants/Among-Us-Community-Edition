using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using UnityEngine;
using System.Linq;
using System;


static class CE_GameLua
{
    public static void ActivateWin()
    {
        throw new NotImplementedException(); // TODO: implement this
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

    public static void GetAllPlayers()
    {
        throw new NotImplementedException(); // TODO: implement this
    }
}
