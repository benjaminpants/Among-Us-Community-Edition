using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

static class CE_CustomOptionsDataManager
{

    static public void Serialize(BinaryWriter writer, byte gamemodeid, byte gmv, List<CE_CustomOptionsData> Data)
    {
        writer.Write((byte)0); //Shows which version of the Serializer its serialized in.(UPDATE THIS IF YOU CHANGE THE FORMAT)
        writer.Write(gamemodeid); //writes the ID of the gamemode this is designed for, so when switching gamemodes, it'll know whether to discard or use the current settings
        writer.Write(gmv); //writes the gamemode's version, will be used by gamemodes for distinguishing old versions from new
        List<CE_CustomOptionsData> CompressableData = (from dt in Data
                                                       where dt.CanBeCompressed
                                                       select dt).ToList();
        List<CE_CustomOptionsData> UnCompressableData = (from dt in Data
                                                       where !dt.CanBeCompressed
                                                       select dt).ToList();
        CompressableData.Sort(delegate (CE_CustomOptionsData x, CE_CustomOptionsData y)
        {
            if ((int)x.OptionType > (int)y.OptionType) return 1;
            else if ((int)x.OptionType < (int)y.OptionType) return -1;
            else return 0;
        });
        CE_OptDataTypes prevtype = CE_OptDataTypes.None;
        foreach (CE_CustomOptionsData data in CompressableData)
        { 
            if (prevtype != data.OptionType)
            {
                writer.Write((byte)data.OptionType);
            }
            writer.Write((byte)255); //placeholder
        }
    }
}


public enum CE_OptDataTypes : byte
{
    None,
    Toggle,
    String,
    FloatRange,
    IntRange,
    BoolArray,
    FloatArray,
    IntArray,
    StringArray,
    ByteRange,
    ByteArray
}
class CE_CustomOptionsData
{
    public CE_CustomOptionsData()
    {

    }

    public CE_OptDataTypes OptionType;
    public bool CanBeCompressed; //Should only be set to true for non-array data types
    public object DefaultValue;
    public CE_CustomOptionsData(CE_OptDataTypes type)
    {
        OptionType = type;
        CanBeCompressed = (type == CE_OptDataTypes.Toggle || type == CE_OptDataTypes.String || type == CE_OptDataTypes.FloatRange || type == CE_OptDataTypes.IntRange || type == CE_OptDataTypes.ByteRange);
        if (OptionType == CE_OptDataTypes.Toggle)
        {
            DefaultValue = false;
        }
        if (OptionType == CE_OptDataTypes.String)
        {
            DefaultValue = "Default";
        }
        if (OptionType == CE_OptDataTypes.FloatRange)
        {
            DefaultValue = 0.5f;
        }
        if (OptionType == CE_OptDataTypes.IntRange)
        {
            DefaultValue = 5;
        }
        if (OptionType == CE_OptDataTypes.ByteRange)
        {
            DefaultValue = (byte)255;
        }
    }
}
