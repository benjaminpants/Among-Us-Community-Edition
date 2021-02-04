using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CE_MapInfo
{
    public string MapName;
    public string[] CustomLocationNames = new string[12];
    public bool IsCustom;

    public CE_MapInfo()
    {
        MapName = "Null Zone";
        IsCustom = false;
    }

    public CE_MapInfo(string mapname)
    {
        MapName = mapname;
        IsCustom = false;
    }
    public CE_MapInfo(string mapname,string[] customlocals)
    {
        MapName = mapname;
        IsCustom = true;
        CustomLocationNames = customlocals;
    }
}

