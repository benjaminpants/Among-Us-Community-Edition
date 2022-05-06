using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class CE_PlayerColor
{
    public Color32 Base = Color.white;
    public Color32 Shadow = Color.black;
    public Color32 Visor = Color.blue;
    public bool IsSpecial = false;
    public string Name = "Unnamed Color";
    [JsonIgnore]
    public bool IsFunnyRainbowColor;

    public CE_PlayerColor()
    {

    }

    public CE_PlayerColor(Color Colorr, string CName, bool israinbow = false)
    {
        Base = Colorr;
        Shadow = Colorr * Color.gray;
        Visor = Colorr * Color.gray;
        Name = CName;
        IsFunnyRainbowColor = israinbow;
        IsSpecial = israinbow;
    }

    public CE_PlayerColor(Color BaseColor, Color ShadowColor, Color VisorColor, bool IsSColor, string CName)
    {
        Base = BaseColor;
        Shadow = ShadowColor;
        Visor = VisorColor;
        Name = CName;
        IsSpecial = IsSColor;
    }
    public CE_PlayerColor(Color BaseColor, Color ShadowColor, Color VisorColor, string CName)
    {
        Base = BaseColor;
        Shadow = ShadowColor;
        Visor = VisorColor;
        IsSpecial = false;
        Name = CName;
    }

}
