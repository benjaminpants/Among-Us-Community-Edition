using System.IO;
using FaDe.Unity.Core;
using UnityEngine;
using Microsoft.Win32;
using Steam_acf_File_Reader;
using System.Collections.Generic;

public class CE_UIHelpers
{
	public static AudioClip HoverSound;

	public static AudioClip ClickSound;

	public static bool SuccesfullyLoadedCache;

	public static bool IsActive() //this code is dumb, fix it.
	{
		if (!CE_Intro.IsShown && !CE_DevMinigame.IsShown && !CE_DevTool.IsShown && !CE_GameSettingsUI.IsShown && !CE_GlobalSettingsUI.IsShown && !CE_ControlsUI.IsShown && !CE_ModUI.IsShown)
		{
			return CE_AnimationDebuger.IsShown;
		}
		return true;
	}
	public static void VerifyGamemodeGUICache(bool ignorecase)
    {
		SuccesfullyLoadedCache = true;
		return; //disable anti-piracy protection for now
		bool y = VGGUIC();
		if (!y)
        {
			Debug.LogWarning("Cache was unable to load!");
        }
		SuccesfullyLoadedCache = y;
    }
	private static bool VGGUIC() //haha actually the anti-piracy method
	{
		string Reg32 = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam", "InstallPath", null) as string;
		string Reg64 = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Valve\\Steam", "InstallPath", null) as string;

		if (Reg32 == null && Reg64 == null)
		{
			return false;
		}

		string DirToUse = Reg64 == null ? Reg32 : Reg64;

		if (Directory.Exists(Path.Combine(DirToUse, "steamapps")))
		{
			foreach (FileInfo fo in new DirectoryInfo(Path.Combine(DirToUse, "steamapps")).GetFiles("appmanifest_*.acf"))
			{
				string text = File.ReadAllText(fo.FullName);
				text = text.ToLowerInvariant();
				if (text.Contains("\"appid\"		\"945360\"".ToLowerInvariant()))
				{
					return true;
				}
			}
			FileInfo folderthing = null;
			try
			{
				folderthing = new DirectoryInfo(Path.Combine(DirToUse, "steamapps")).GetFiles("libraryfolders.vdf")[0];
				AcfReader read = new AcfReader(folderthing.FullName);
				ACF_Struct stru = read.ACFFileToStruct();
				bool foundallocals = false;
				int maxvalue = 0;
				List<string> strings = new List<string>();

				while (!foundallocals)
				{
					maxvalue++;
					if (stru.SubItems.TryGetValue(maxvalue.ToString(), out string dir))
					{
						strings.Add(dir);
					}
					else
					{
						foundallocals = true;
					}
				}

				if (maxvalue == 0)
				{
					return false;
				}

				foreach (string str in strings)
				{
					foreach (FileInfo fo in new DirectoryInfo(Path.Combine(str, "steamapps")).GetFiles("appmanifest_*.acf"))
					{
						string text = File.ReadAllText(fo.FullName);
						text = text.ToLowerInvariant();
						if (text.Contains("\"appid\"		\"945360\"".ToLowerInvariant()))
						{
							return true;
						}
					}
				}

				return false;

			}
			catch
			{
				return false;
			}
		}
		else
        {

        }
		return false;
	}

	public static void CollapseAll()
	{
		CE_Intro.IsShown = false;
		CE_GameSettingsUI.IsShown = false;
		CE_GlobalSettingsUI.IsShown = false;
		CE_ControlsUI.IsShown = false;
		CE_DevTool.IsShown = false;
		CE_DevMinigame.IsShown = false;
		CE_ModUI.IsShown = false;
	}

	static CE_UIHelpers()
	{
	}

	public static void LoadCommonAssets()
	{
		if (!ClickSound)
		{
			ClickSound = CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "UI_Select.wav"));
		}
		if (!HoverSound)
		{
			HoverSound = CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "UI_Hover.wav"));
		}
	}

	public static void LoadDebugConsole()
	{
		if (!DestroyableSingleton<ProblemTraceConsole>.InstanceExists)
		{
			Object.Instantiate(new GameObject()).AddComponent<ProblemTraceConsole>();
		}
		if (!DestroyableSingleton<CE_AnimationDebuger>.InstanceExists)
		{
			Object.Instantiate(new GameObject()).AddComponent<CE_AnimationDebuger>();
		}
		if (!DestroyableSingleton<CE_DevTool>.InstanceExists)
		{
			Object.Instantiate(new GameObject()).AddComponent<CE_DevTool>();
		}
		if (!DestroyableSingleton<CE_DevMinigame>.InstanceExists)
		{
			Object.Instantiate(new GameObject()).AddComponent<CE_DevMinigame>();
		}
	}

	public static void ForceLoadDebugUIs()
	{
		Object.Instantiate(new GameObject()).AddComponent<ProblemTraceConsole>();
		Object.Instantiate(new GameObject()).AddComponent<CE_AnimationDebuger>();
		Object.Instantiate(new GameObject()).AddComponent<CE_DevTool>();
		Object.Instantiate(new GameObject()).AddComponent<CE_DevMinigame>();
	}
}
