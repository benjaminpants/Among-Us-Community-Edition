using UnityEngine;

public class CE_Extensions
{
	private static bool hasPlayed;

	private static CE_Intro Intro;

	public static string GetGameDirectory()
    {
		return System.IO.Directory.GetParent(Application.dataPath).FullName;
    }

	public static string GetTexturesDirectory(string ExtraDir = null)
    {
		if (string.IsNullOrEmpty(ExtraDir)) return System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures");
		else return System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", ExtraDir);
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