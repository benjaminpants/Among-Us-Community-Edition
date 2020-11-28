using System;
using UnityEngine;
using System.Collections.Generic;

public enum WhoCanSee
{
	None,
	All,
	Impostors,
	Crewmates,
	RolesOfSameType,
	Custom
}

public enum SpecialFeatures
{ 
	Kill,
	Sabotage,
	Vent
}


public class RoleInfo
{
	public RoleInfo(GameData.PlayerInfo.Role Enum, string Name, Color Color, WhoCanSee VisibleToWho, List<SpecialFeatures> SpecialsEnabled, byte priority, byte maxamount)
    {
		RoleEnum = Enum;
		RoleName = Name;
		RoleColor = Color;
		VisibleTo = VisibleToWho;
		Priority = priority;
		MaxAmount = maxamount;
		
    }

	public GameData.PlayerInfo.Role RoleEnum = GameData.PlayerInfo.Role.None;
	public string RoleName = "Undefined";
	public Color RoleColor = Palette.DisabledGrey;
	public WhoCanSee VisibleTo = WhoCanSee.None;
	public List<SpecialFeatures> EnabledFeatures = new List<SpecialFeatures>();
	public byte Priority = 1;
	public byte MaxAmount = 1; //Max amount of this role allowed
	public bool CanSee(PlayerControl pc)
	{
		switch (this.VisibleTo)
		{
			case WhoCanSee.None:
				return false;
			case WhoCanSee.All:
				return true;
			case WhoCanSee.Impostors:
				return pc.Data.IsImpostor;
			case WhoCanSee.Crewmates:
				return !pc.Data.IsImpostor;
			case WhoCanSee.RolesOfSameType:
				return pc.Data.role == this.RoleEnum;
			case WhoCanSee.Custom:
				throw new NotImplementedException(); //NOTE: Implement this
			default:
				return false;
		}
	}

}
