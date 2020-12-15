﻿using System;
using UnityEngine;
using System.Collections.Generic;




public enum CE_Specials
{
	Kill,
	Sabotage,
	Vent
}

public enum CE_RoleVisibility
{ 
	None,
	Impostors,
	Crewmates,
	RolesOfSameType,
	Lua
}

public enum CE_WinWith
{ 
	Impostors,
	Crewmates,
	Neither,
    All
}




//set RoleText to "default_text" to use the default "There are _ impostors among us" text.

public static class CE_RoleManager
{
    public static Dictionary<byte, CE_Role> Roles = new Dictionary<byte, CE_Role>();


    public static bool AddRole(CE_Role roletoadd)
    {
        UnityEngine.Debug.Log("Adding role with name:" + roletoadd.RoleName + ", id:" + Roles.Count);
        return Roles.TryAdd((byte)Roles.Count, roletoadd);
    }


    public static byte GetRoleFromName(string Name)
    {
        foreach (KeyValuePair<byte,CE_Role> kvp in Roles)
        {
            if (kvp.Value.RoleName == Name)
            {
                return kvp.Key;
            }
        }
        return 0; //return the null role, AKA assign no role.
    }

    public static CE_Role GetRoleFromID(byte id)
    {
        CE_Role returnvalue;
        if (Roles.TryGetValue(id,out returnvalue))
        {
            return returnvalue;
        }
        Debug.LogError("Could not find role with ID: " + id + "!\n Returning New Role...");
        return new CE_Role();
    }

    public static int GetRoleCount(byte id)
    {
        int count = 0;
        foreach (GameData.PlayerInfo plyfo in GameData.Instance.AllPlayers)
        {
            if (plyfo.role == id)
            {
                count++;
            }
        }
        return count;
    }

}


public class CE_Role
{
	public CE_Role() // Don't actually use this constructor for anything besides creating the "None" role please
    {
		RoleName = "Undefined";
		RoleColor = Palette.CrewmateBlue;
		AvailableSpecials = new List<CE_Specials>();
		RoleWinWith = CE_WinWith.All;
		RoleVisibility = CE_RoleVisibility.None;
        UseImpVision = false;
        RoleText = "Undefined";
    }

    public bool CanDo(CE_Specials special)
    {
        return AvailableSpecials.Contains(special);
    }

    public CE_Role(string Name, Color Color)
    {
        RoleName = Name;
        RoleColor = Color;
        AvailableSpecials = new List<CE_Specials>();
        RoleWinWith = CE_WinWith.Neither;
        RoleVisibility = CE_RoleVisibility.None;
        UseImpVision = false;
        RoleText = "default_text";
    }

    public CE_Role(string Name, Color Color, string RoleTxT)
    {
        RoleName = Name;
        RoleColor = Color;
        AvailableSpecials = new List<CE_Specials>();
        RoleWinWith = CE_WinWith.Neither;
        RoleVisibility = CE_RoleVisibility.None;
        UseImpVision = false;
        RoleText = RoleTxT;
    }

    public CE_Role(string Name, Color Color, string RoleTxt, List<CE_Specials> Specials, CE_WinWith WinWith = CE_WinWith.Neither, CE_RoleVisibility Visibility = CE_RoleVisibility.None, bool ImpVision = false)
    {
        RoleName = Name;
        RoleColor = Color;
        AvailableSpecials = Specials;
        RoleWinWith = WinWith;
        RoleVisibility = Visibility;
        UseImpVision = ImpVision;
        RoleText = RoleTxt;
    }
    public string RoleName;

	public Color RoleColor;

	public List<CE_Specials> AvailableSpecials;

	public CE_RoleVisibility RoleVisibility;

	public CE_WinWith RoleWinWith;

    public string RoleText;

	public bool UseImpVision;
}