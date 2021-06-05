using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;


public static class CE_BinaryExtensions
{
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