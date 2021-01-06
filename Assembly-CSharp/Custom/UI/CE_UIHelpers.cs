using System.IO;
using FaDe.Unity.Core;
using UnityEngine;

public class CE_UIHelpers
{
	public static AudioClip HoverSound;

	public static AudioClip ClickSound;

	public static bool IsActive()
	{
		if (!CE_Intro.IsShown && !CE_DevMinigame.IsShown && !CE_DevTool.IsShown && !CE_GameSettingsUI.IsShown && !CE_GlobalSettingsUI.IsShown && !CE_ControlsUI.IsShown)
		{
			return CE_AnimationDebuger.IsShown;
		}
		return true;
	}

	public static void CollapseAll()
	{
		CE_Intro.IsShown = false;
		CE_GameSettingsUI.IsShown = false;
		CE_GlobalSettingsUI.IsShown = false;
		CE_ControlsUI.IsShown = false;
		CE_DevTool.IsShown = false;
		CE_DevMinigame.IsShown = false;
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
