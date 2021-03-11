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
    public string ContentFolder;
    public CEM_Map Map;


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
    public CE_MapInfo(string mapname, bool iscustom)
    {
        MapName = mapname;
        IsCustom = iscustom;
    }

    public CE_MapInfo(string mapname, CEM_Map map, string content_folder)
    {
        MapName = mapname;
        IsCustom = true;
        Map = map;
        ContentFolder = content_folder;
    }
    public CE_MapInfo(string mapname,string[] customlocals)
    {
        MapName = mapname;
        IsCustom = true;
        CustomLocationNames = customlocals;
    }
}

