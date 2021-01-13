using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public static class CE_Input
{
	private static Dictionary<KeyCode, bool[]> KeyDownStatus = new Dictionary<KeyCode, bool[]>();
	public static bool CE_GetKeyDown(KeyCode key)
	{
		bool isKey = Input.GetKey(key);
		if (!KeyDownStatus.ContainsKey(key)) KeyDownStatus.Add(key, new bool[2]);

		KeyDownStatus[key][0] = isKey;

		if (KeyDownStatus[key][0] && !KeyDownStatus[key][1])
		{
			KeyDownStatus[key][1] = true;
			return true;
		}
		if (!KeyDownStatus[key][0] && KeyDownStatus[key][1])
		{
			KeyDownStatus[key][1] = false;
		}

		return false;
	}
	public static bool CE_GetKeyUp(KeyCode key)
	{
		return Input.GetKeyUp(key);
	}
	public static void EscapeFunctionality()
    {
		if (CloseMinigame()) return;
		else if (CloseMapBehavior()) return;
		else if (ClosePlayerMenu()) return;
		else OpenOptionsMenu();

		/*
		if ((bool)Minigame.Instance)
		{
			Minigame.Instance.Close();
		}
		else if (DestroyableSingleton<HudManager>.InstanceExists && (bool)MapBehaviour.Instance && MapBehaviour.Instance.IsOpen)
		{
			MapBehaviour.Instance.Close();
		}
		else
		{
			CustomPlayerMenu customPlayerMenu = Object.FindObjectOfType<CustomPlayerMenu>();
			if ((bool)customPlayerMenu)
			{
				customPlayerMenu.Close(canMove: true);
			}
		}
		*/

		bool CloseMinigame()
        {
			bool exists = (bool)Minigame.Instance;
			if (exists) Minigame.Instance.Close();
			return exists;
		}

		bool CloseMapBehavior()
		{
			bool exists = DestroyableSingleton<HudManager>.InstanceExists && (bool)MapBehaviour.Instance && MapBehaviour.Instance.IsOpen;
			if (exists) MapBehaviour.Instance.Close();
			return exists;
		}

		bool OpenOptionsMenu()
		{
			bool exists = DestroyableSingleton<HudManager>.InstanceExists && !DestroyableSingleton<HudManager>.Instance.GameMenu.IsOpen;
			if (exists) DestroyableSingleton<HudManager>.Instance.GameMenu.Open();
			return exists;
		}

		bool ClosePlayerMenu()
        {
			CustomPlayerMenu customPlayerMenu = Object.FindObjectOfType<CustomPlayerMenu>();
			bool exists = (bool)customPlayerMenu;
			if (exists) customPlayerMenu.Close(canMove: true);
			return exists;
		}
	}
	public static void EscapeFunctionality_MainMenu(MainMenuManager mainMenuManager)
	{
		if (CloseOptionsMenu()) return;
		else ExitGame();

		bool CloseOptionsMenu()
		{
			bool exists = false;
			GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				OptionsMenuBehaviour component = array[i].GetComponent<OptionsMenuBehaviour>();
				if ((bool)component)
				{
					component.Close();
					exists = true;
				}
			}
			return exists;
		}

		void ExitGame()
        {
			mainMenuManager.Quit();
		}

	}
}
