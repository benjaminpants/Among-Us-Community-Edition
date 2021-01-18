using UnityEngine;
using System;
using System.IO;
using System.Reflection;

public class VersionShower : MonoBehaviour
{

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
			"Z",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			"0"

		};
    public TextRenderer text;

	static int GetDeterministicHashCode(string str)
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


	public void Start()
	{
		byte[] Bytes = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
		string CombinedByteListString = "";
		for (int i = 1; i != 600; i++)
        {
			CombinedByteListString = CombinedByteListString + Bytes[i].ToString();
		}
		CombinedByteListString += Bytes.Length.ToString();
		text.Text = "v0.5.3" + "\nBuild ID:" + CreateIDFromInt(GetDeterministicHashCode(CombinedByteListString),7);
		Screen.sleepTimeout = -1;
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
		CE_Extensions.UpdateWindowTitle();
	}

}
