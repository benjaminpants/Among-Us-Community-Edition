using UnityEngine;

public class VersionShower : MonoBehaviour
{
	public TextRenderer text;

	private static bool IntroPlayed;

	private CE_Intro Intro;

	public void Start()
	{
		text.Text = "v0.4.1 - Lua API Beta";
		Screen.sleepTimeout = -1;
		CE_UIHelpers.LoadDebugConsole();
		if (!IntroPlayed)
		{
			CE_RoleManager.AddRole(new CE_Role());
			CE_LuaLoader.LoadLua();
			if (!SaveManager.HideIntro)
			{
                Intro = Object.Instantiate(new GameObject()).AddComponent<CE_Intro>();
				CE_Intro.IsShown = true;
			}
		}
		IntroPlayed = true;
	}
}
