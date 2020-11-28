using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class RoleManager
{

    static Dictionary<GameData.PlayerInfo.Role, RoleInfo> Roles = new Dictionary<GameData.PlayerInfo.Role, RoleInfo>()
    {
        { GameData.PlayerInfo.Role.None, new RoleInfo(
        GameData.PlayerInfo.Role.None,
        "Undefined",
        Palette.DisabledGrey,
        WhoCanSee.None,
        new List<SpecialFeatures>(),
        0,
        255
        )}, //Although the NONE role is technically not a role, it still needs to be defined here because lazyness.
        { GameData.PlayerInfo.Role.Sheriff, new RoleInfo(
        GameData.PlayerInfo.Role.Sheriff,
        "Sheriff",
        Palette.SheriffYellow,
        WhoCanSee.None,
        new List<SpecialFeatures>(){SpecialFeatures.Kill},
        5,
        1
        )}

    };
}
