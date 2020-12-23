using System.IO;
using FaDe.Unity.Core;
using UnityEngine;

public class CE_UIHelpers
{
	public static AudioClip HoverSound;

	public static AudioClip ClickSound;

	public static bool IsActive()
	{
		if (!CE_Intro.IsShown && !CE_DevTool.IsShown && !CE_GameSettingsUI.IsShown && !CE_GlobalSettingsUI.IsShown && !CE_ControlsUI.IsShown)
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
	}

	static CE_UIHelpers()
	{
	}

	public static void LoadCommonAssets()
	{
		if (!ClickSound)
		{
			ClickSound = WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "UI_Select.wav"));
		}
		if (!HoverSound)
		{
			HoverSound = WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "UI_Hover.wav"));
		}
	}

	public static bool isProblemTraceConsoleActive()
	{
		return false;
	}

	private static GUIStyle UpDownSettingButtons()
	{
		return new GUIStyle(GUI.skin.button)
		{
			fixedWidth = 50f,
			fixedHeight = 50f,
			fontSize = 25,
			normal = 
			{
				textColor = Color.white
			}
		};
	}

	private static GUIStyle UpDownSettingLabel(float width = 250f)
	{
		if (width == 0f)
		{
			return new GUIStyle(GUI.skin.label)
			{
				fixedHeight = 50f,
				fixedWidth = width,
				fontSize = 25,
				normal = 
				{
					textColor = Color.white
				}
			};
		}
		return new GUIStyle(GUI.skin.label)
		{
			alignment = TextAnchor.MiddleCenter,
			fixedHeight = 50f,
			fixedWidth = width,
			fontSize = 25,
			normal = 
			{
				textColor = Color.white
			}
		};
	}

	private static GUIStyle WindowStyle(int w, int h)
	{
		return new GUIStyle(GUI.skin.window)
		{
			normal = 
			{
				background = CE_TextureNSpriteExtensions.MakeTex(1, 1, Color.black)
			}
		};
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
	}

	public static void ForceLoadDebugUIs()
	{
		Object.Instantiate(new GameObject()).AddComponent<ProblemTraceConsole>();
		Object.Instantiate(new GameObject()).AddComponent<CE_AnimationDebuger>();
		Object.Instantiate(new GameObject()).AddComponent<CE_DevTool>();
	}
}
