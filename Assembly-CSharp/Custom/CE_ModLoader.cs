using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class CE_ModLoader
{
    public static List<CE_Mod> LMods = new List<CE_Mod>();
    public static string ColorString;
    public static int ColorHash;
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
        ColorString = "";
        ColorHash = 0;

        string AttemptedDir = Path.Combine(CE_Extensions.GetGameDirectory(), "Mods");

        string[] DisabledModNames = new string[0];

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
                string[] txtcontent = File.ReadAllText(txts[0]).Split(split, StringSplitOptions.RemoveEmptyEntries);
                if (txtcontent[0] == "Rainbow") //no sneaky overrides!
                {
                    continue;
                }

                CE_Mod CURM = new CE_Mod(Directory.Exists(Path.Combine(DS, "Hats")) ? Path.Combine(DS, "Hats") : "",
                    Directory.Exists(Path.Combine(DS, "Skins")) ? Path.Combine(DS, "Skins") : "",
                    Directory.Exists(Path.Combine(DS, "Lua")) ? Path.Combine(DS, "Lua") : "",
                    txtcontent[0], txtcontent[1] + "\n" + txtcontent[2], Directory.Exists(Path.Combine(DS, "SFC")) ? Path.Combine(DS, "SFC") : "");


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

                if (CURM.SFCDirectory != "")
                {
                    string colorfile = Path.Combine(CURM.SFCDirectory,"colors.json");
                    if (File.Exists(colorfile))
                    {
                        List<CE_PlayerColor> PLCLS = JsonConvert.DeserializeObject<List<CE_PlayerColor>>(File.ReadAllText(colorfile));
                        Palette.PLColors.AddRange(PLCLS);
                        Palette.PLColors.Sort(delegate (CE_PlayerColor c1, CE_PlayerColor c2)
                        {
                            if (c1.IsSpecial || c2.IsSpecial)
                            {
                                return (c1.IsSpecial.CompareTo(c2.IsSpecial));
                            }
                            Color.RGBToHSV(c1.Base, out float H1, out float S1, out float V1);
                            Color.RGBToHSV(c2.Base, out float H2, out float S2, out float V2);
                            return (H1).CompareTo(H2);

                        });
                    }
                }

            }
        }
        else
        {
            Application.Quit(2); //CE needs 1 base mod to function! Without it, it will break, so lets just close with an error instead.
        }
        LMods.Add(new CE_Mod("Rainbow","Adds the rainbow color"));
        if (!DisabledModNames.Contains("Rainbow"))
        {
            Palette.PLColors.Add(new CE_PlayerColor(new Color32(255, 255, 255, byte.MaxValue), "Rainbow", true));
        }
        foreach (CE_PlayerColor pc in Palette.PLColors)
        {
            ColorString += (pc.Name + pc.Base.ToString());
        }
        ColorHash = VersionShower.GetDeterministicHashCode(ColorString);
        VersionShower.ColorID = VersionShower.CreateIDFromInt(ColorHash,7);
        ColorString = "";
    }

}


public class CE_Mod
{
    public string HatsDirectory;
    public string SkinsDirectory;
    public string LuaDirectory;
    public string SFCDirectory;
    public string ModName;
    public string ModDesc;
    public bool Enabled = true;

    public CE_Mod()
    {
        ModName = "Null";
        ModDesc = "Oopsie! Someone forgot to put a description!";
    }

    public CE_Mod(string title, string desc)
    {
        ModName = title;
        ModDesc = desc;
    }


    public override string ToString()
    {
        return ModName == "" ? "Unnamed Mod" : ModName;
    }

    public CE_Mod(string hd, string sd, string ld, string mn, string md, string sfd, bool en = true)
    {
        HatsDirectory = hd;
        SkinsDirectory = sd;
        LuaDirectory = ld;
        ModName = mn;
        ModDesc = md;
        Enabled = en;
        SFCDirectory = sfd;
    }



}
