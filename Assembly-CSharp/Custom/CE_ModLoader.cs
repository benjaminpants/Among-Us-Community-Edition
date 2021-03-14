using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

public static class CE_ModLoader
{
    public static List<CE_Mod> LMods = new List<CE_Mod>();

    public static void UpdateDisabledMods()
    {
        string disablednames = "";
        foreach (CE_Mod mod in LMods)
        {
            if (!mod.Enabled)
            {
                disablednames += (mod.ModName + "\n");
            }
        }
        File.WriteAllText(Path.Combine(CE_Extensions.GetGameDirectory(), "disabledmods.txt"), disablednames);

    }


    public static void LoadMods()
    {
        string AttemptedDir = Path.Combine(CE_Extensions.GetGameDirectory(), "Mods");

        string[] DisabledModNames = new string[1];

        char[] split = new char[2]
                {
                    '\r','\n'
                };
        if (File.Exists(Path.Combine(CE_Extensions.GetGameDirectory(), "disabledmods.txt")))
        {
            DisabledModNames = File.ReadAllText(Path.Combine(CE_Extensions.GetGameDirectory(), "disabledmods.txt")).Split(split,StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            File.WriteAllText(Path.Combine(CE_Extensions.GetGameDirectory(), "disabledmods.txt"),"");
        }
        if (Directory.Exists(AttemptedDir))
        {
            foreach (string DS in Directory.GetDirectories(AttemptedDir))
            {
                string[] txts = Directory.GetFiles(DS, "modinfo.txt");
                if (txts.Length != 1)
                {
                    continue;
                }
                string[] txtcontent = File.ReadAllText(txts[0]).Split(split,StringSplitOptions.RemoveEmptyEntries);


                CE_Mod CURM = new CE_Mod(Directory.Exists(Path.Combine(DS, "Hats")) ? Path.Combine(DS, "Hats") : "",
                    Directory.Exists(Path.Combine(DS, "Skins")) ? Path.Combine(DS, "Skins") : "",
                    Directory.Exists(Path.Combine(DS, "Lua")) ? Path.Combine(DS, "Lua") : "",
                    txtcontent[0], txtcontent[1] + "\n" + txtcontent[2]);


                LMods.Add(CURM);
                if (DisabledModNames.Any(CURM.ModName.Contains))
                {
                    CURM.Enabled = false;
                }
                if (!CURM.Enabled)
                {
                    Debug.Log("Mod " + CURM.ModName + " found in disabled list, not loading.");
                    continue;
                }
                if (CURM.LuaDirectory != "")
                {
                    CE_LuaLoader.LoadLua(CURM.LuaDirectory);
                }

                if (CURM.HatsDirectory != "")
                {
                    HatManager.Instance.AddHats(CURM.HatsDirectory);
                }

                if (CURM.SkinsDirectory != "")
                {
                    HatManager.Instance.AddSkins(CURM.SkinsDirectory);
                }

            }
        }
        else
        {
            Application.Quit(2); //CE needs 1 base mod to function! Without it, it will break, so lets just close with an error instead.
        }
    }

}


public class CE_Mod
{
    public string HatsDirectory;
    public string SkinsDirectory;
    public string LuaDirectory;
    public string ModName;
    public string ModDesc;
    public bool Enabled = true;

    public CE_Mod()
    {
        ModName = "Null";
        ModDesc = "Oopsie! Someone forgot to put a description!";
    }


    public override string ToString()
    {
        return ModName == "" ? "Unnamed Mod" : ModName;
    }

    public CE_Mod(string hd, string sd, string ld, string mn, string md, bool en = true)
    {
        HatsDirectory = hd;
        SkinsDirectory = sd;
        LuaDirectory = ld;
        ModName = mn;
        ModDesc = md;
        Enabled = en;
    }



}
