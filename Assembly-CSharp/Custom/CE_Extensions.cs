using UnityEngine;
using System.Runtime.InteropServices;
public class CE_Extensions
{
	private static bool hasPlayed;

	private static CE_Intro Intro;

	private static bool TitleChanged = false;

	//Import the following.
	[DllImport("user32.dll", EntryPoint = "SetWindowText")]
	public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);
	[DllImport("user32.dll", EntryPoint = "FindWindow")]
	public static extern System.IntPtr FindWindow(System.String className, System.String windowName);
	[DllImport("user32.dll")]
	private static extern System.IntPtr GetActiveWindow();

	public static string GetGameDirectory()
    {
		return System.IO.Directory.GetParent(Application.dataPath).FullName;
    }

	public static string GetTexturesDirectory(string ExtraDir = null)
    {
		if (string.IsNullOrEmpty(ExtraDir)) return System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures");
		else return System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", ExtraDir);
	}

	public static void UpdateWindowTitle()
    {
		if (!TitleChanged)
        {
			bool isFocused = Application.isFocused;
			var windowPtr = FindWindow(null, "Among Us");
			if (windowPtr == GetActiveWindow() && isFocused)
			{
				// title changing script
				SetWindowText(windowPtr, "Among Us: Town of modders");
				TitleChanged = true;
			}
		}
	}

	private static void PlayIntro()
	{
		if (!SaveManager.HideIntro)
		{
			Intro = Object.Instantiate(new GameObject()).AddComponent<CE_Intro>();
			CE_Intro.IsShown = true;
		}

	}

	public static void OnStartup()
	{
		CE_UIHelpers.LoadDebugConsole();
		if (!hasPlayed)
        {
			ResolutionManager.SetVSync(SaveManager.EnableVSync);
			CE_RoleManager.AddRole(new CE_Role());
			CE_LuaLoader.LoadLua();
			PlayIntro();
			hasPlayed = true;
		}

	}
}
