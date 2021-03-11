using UnityEngine;
using System;
using System.IO;
using System.Reflection;

public class VersionShower : MonoBehaviour
{

	private int lastlua;
	private int lasthat;
	private string LuaID;
	private string HatID;
	public static int buildhash;
	public static string BuildID;

	private readonly string[] Characters = {
			"A",
			"B",
			"C",
			"D",
			"E",
			"F",
			"G",
			"H",
			"I",
			"J",
			"K",
			"L",
			"M",
			"N",
			"O",
			"P",
			"Q",
			"R",
			"S",
			"T",
			"U",
			"V",
			"W",
			"X",
			"Y",
			"Z"

		};
    public TextRenderer text;

    public static int GetDeterministicHashCode(string str)
    {
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }

	public static int GetDeterministicHashCodeBytes(byte[] bytes)
	{
		unchecked
		{
			int hash1 = (5381 << 16) + 5381;
			int hash2 = hash1;

			for (int i = 0; i < bytes.Length - 1; i += 2)
			{
				hash1 = ((hash1 << 5) + hash1) ^ bytes[i];
				if (i == bytes.Length - 1)
					break;
				hash2 = ((hash2 << 5) + hash2) ^ bytes[i + 1];
			}

			return hash1 + (hash2 * 1566083941);
		}
	}


	public void Start()
	{
		byte[] Bytes = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
		buildhash = GetDeterministicHashCodeBytes(Bytes);
		BuildID = CreateIDFromInt(GetDeterministicHashCodeBytes(Bytes), 7);
		text.Text = "v0.5.6 - Deserves to be 0.6.0" + "\nBuild ID:" + BuildID;
		Screen.sleepTimeout = -1;
		CE_CustomMapManager.Initialize();
		CE_Extensions.OnStartup();
	}

	public string CreateIDFromInt(int ID,byte length)
    {
		System.Random RNG = new System.Random(ID);
		string StringID = "";
		for (int i = 1; i != length; i++)
		{
			StringID = StringID + Characters[RNG.Next(0,Characters.Length)];
		}
		return StringID;
    }

	public void Update()
    {
		text.Text = "v0.6.0 - MAPS AND PAIN" + "\nBuild ID:" + BuildID + "\nLua ID:" + LuaID + "\nHats ID:" + HatID;
		if (CE_LuaLoader.TheOmegaHash != lastlua)
        {
			lastlua = CE_LuaLoader.TheOmegaHash;
            LuaID = CreateIDFromInt(CE_LuaLoader.TheOmegaHash, 7);
		}
		if (CE_WardrobeManager.HatHash != lasthat)
		{
			lasthat = CE_WardrobeManager.HatHash;
			HatID = CreateIDFromInt(CE_WardrobeManager.HatHash, 7);
		}
		CE_Extensions.UpdateWindowTitle();
	}

}
