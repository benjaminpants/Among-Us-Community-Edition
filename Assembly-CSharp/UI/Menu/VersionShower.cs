using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using Steam_acf_File_Reader;
using System.Collections.Generic;

public class VersionShower : MonoBehaviour
{

	private int lastlua;
	private int lasthat;
	private string LuaID;
	private string HatID;
	public static int buildhash;
	public static string BuildID;
	public static string ColorID;

	private GameObject FreeplayButton;

	private GameObject ModsButton;

	private static readonly string[] Characters = {
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
			"0",
			"!",
			"."

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
        text.Text = "when imp sus";
        Screen.sleepTimeout = -1;
        CE_Extensions.OnStartup();
        if (GameObject.Find("FreePlayButton") != null)
        {
            AddNewButtons();
        }
        //File.WriteAllText(Path.Combine(CE_Extensions.GetGameDirectory(),"colors.json"),Newtonsoft.Json.JsonConvert.SerializeObject(Palette.PLColors,Newtonsoft.Json.Formatting.Indented));
        CE_UIHelpers.VerifyGamemodeGUICache(true);

	}



	public void AddNewButtons()
    {
		FreeplayButton = GameObject.Find("FreePlayButton");
		GameObject newbutton = GameObject.Instantiate(FreeplayButton);
		newbutton.transform.localPosition = new Vector3(0f,-0.25f,0f);
		newbutton.transform.name = "ModButton";
		PassiveButton pasbut = newbutton.GetComponent<PassiveButton>();
		Destroy(newbutton.GetComponent<HostGameButton>());
		newbutton.GetComponent<SpriteRenderer>().sprite = CE_TextureNSpriteExtensions.ConvertToSprite(CE_CommonUI.ModsButton,new Vector2(0.5f,0.5f));
		pasbut.OnClick.RemoveAllListeners();
		pasbut.OnClick.AddListener(OpenModsMenu); //learned how to properly relink passive buttons??? maybe we could stop using unity ui??? pog???
		ModsButton = newbutton;
    }

	public void OpenModsMenu()
    {
		CE_ModUI.IsShown = true;
    }



	public static string CreateIDFromInt(int ID,byte length)
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
		//Debug.Log(ServerManager.Instance.availableServers.Length);
		text.Text = "v0.5.17.4 - [r0]A bit bigger update[]" + "\nBuild ID:[r1]" + BuildID + "[]\nLua ID:[r1]" + LuaID + "[]\nHats ID:[r1]" + HatID + "[]\nColors ID:[r1]" + ColorID;
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
		if ((bool)FreeplayButton && (bool)ModsButton)
		{
			ModsButton.SetActive(FreeplayButton.activeSelf);
		}
		CE_Extensions.UpdateWindowTitle();
	}

}
