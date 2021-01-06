using UnityEngine;

public class MMOnlineManager : DestroyableSingleton<MMOnlineManager>
{
	public GameObject HelpMenu;

	public void Start()
	{
		if ((bool)HelpMenu)
		{
			if (SaveManager.ShowOnlineHelp)
			{
				SaveManager.ShowOnlineHelp = false;
			}
			else
			{
				HelpMenu.gameObject.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		if (CE_Input.CE_GetKeyUp(KeyCode.Escape))
		{
			SceneChanger.ChangeScene("MainMenu");
		}
	}
}
