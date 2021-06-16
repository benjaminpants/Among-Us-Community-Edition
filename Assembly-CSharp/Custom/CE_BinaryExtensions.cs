using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using MoonSharp.Interpreter;
using Hazel;


public static class CE_BinaryExtensions
{

    public static void WriteLuaTableNet(MessageWriter writer, Table table)
    {
        writer.WritePacked(table.Length);
        foreach (DynValue val in table.Values)
        {
            switch (val.Type)
            {
                default:
                    writer.Write((byte)0);
                    break;
                case DataType.Number:
                    if ((Math.Floor(val.Number) == Math.Ceiling(val.Number)))
                    {
                        writer.Write((byte)1);
                        writer.WritePacked((int)val.Number);
                    }
                    else
                    {
                        writer.Write((byte)2);
                        writer.Write((float)val.Number);
                    }
                    break;
                case DataType.String:
                    writer.Write((byte)3);
                    writer.Write(val.String);
                    break;
                case DataType.Boolean:
                    writer.Write((byte)4);
                    writer.Write(val.Boolean);
                    break;
            }
        }
    }

    public static Table ReadLuaTableNet(MessageReader reader)
    {
        int length = 0;
        try
        {
            length = reader.ReadPackedInt32();
        }
        catch
        {
            return new Table(CE_LuaLoader.CurrentGM.script, new DynValue[0]);
        }
        Table tab = new Table(CE_LuaLoader.CurrentGM.script, new DynValue[length]);
        for (int i = 0; i < length; i++)
        {
            try
            {
                switch (reader.ReadByte())
                {
                    default:
                        tab.Append(DynValue.NewNil());
                        break;
                    case 1:
                        tab.Append(DynValue.NewNumber(reader.ReadPackedInt32()));
                        break;
                    case 2:
                        tab.Append(DynValue.NewNumber(reader.ReadSingle()));
                        break;
                    case 3:
                        tab.Append(DynValue.NewString(reader.ReadString()));
                        break;
                    case 4:
                        tab.Append(DynValue.NewBoolean(reader.ReadBoolean()));
                        break;
                }
            }
            catch(Exception E)
            {
                UnityEngine.Debug.LogError("Error handling reader:" + E.Message + "\n" + E.StackTrace);
                tab.Append(DynValue.NewNil());
            }
        }
        return tab;
    }




    public static void WriteStringUIntDictionary(BinaryWriter writer, Dictionary<string, uint> dict)
    {
        writer.Write(dict.Count);

        foreach (KeyValuePair<string, uint> kvp in dict)
        {
            writer.Write(kvp.Key);
            writer.Write(kvp.Value);
        }
    }

    public static Dictionary<string, uint> ReadStringUIntDictionary(BinaryReader reader)
    {
        int count = 0;
        try
        {
            count = reader.ReadInt32();
        }
        catch
        {
            return new Dictionary<string, uint>();
        }

        Dictionary<string, uint> dict = new Dictionary<string, uint>();

        for (int i = 0; i < count; i++)
        {
            try
            {
                if (!dict.TryAdd(reader.ReadString(), reader.ReadUInt32()))
                {
                    UnityEngine.Debug.LogWarning("Failed to add value, possibly duplicate or broken!");
                }
            }
            catch
            {
                UnityEngine.Debug.LogWarning("Reading failed!");
            }
        }

        return dict;
    }

    /*public static void WriteLiterallyAnyClass(BinaryWriter writer, object yes)
    {
        MemberInfo[] members = yes.GetType().GetMembers();
        for (int i = 0; i < members.Length; i++)
        {
            writer.Write(VersionShower.GetDeterministicHashCode(members[i].Name));
        }
    }*/
}