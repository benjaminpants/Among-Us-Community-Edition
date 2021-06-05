using UnityEngine;
using System.Collections;

public class CustomPlayerMenu : MonoBehaviour
{
	public TabButton[] Tabs;

	public Sprite NormalColor;

	public Sprite SelectedColor;

	private int LastTab = 0;

	public void Start()
	{
		PlayerControl.LocalPlayer.moveable = false;
		GameObject.Destroy(Tabs[3].Tab);
		Tabs[3].Tab = new GameObject();
		PassiveButton butt = Tabs[3].Button.GetComponent<PassiveButton>();
		butt.OnClick.RemoveAllListeners();
		butt.OnClick.AddListener(OpenTheFunnyMenu);
	}

	public void OpenTheFunnyMenu()
    {
		DestroyableSingleton<CE_GameSettingsUI>.Instance.enabled = true;
		CE_GameSettingsUI.IsShown = true;
		OpenTab(Tabs[LastTab].Tab);
	}

	public void OpenTab(GameObject tab)
	{
		for (int i = 0; i < Tabs.Length; i++)
		{
			TabButton tabButton = Tabs[i];
			if (tabButton.Tab == tab)
			{
				LastTab = i;
				tabButton.Tab.SetActive(value: true);
				tabButton.Button.sprite = SelectedColor;
			}
			else
			{
				tabButton.Tab.SetActive(value: false);
				tabButton.Button.sprite = NormalColor;
			}
		}
	}

	public void Close(bool canMove)
	{
		CE_UIHelpers.CollapseAll();
		PlayerControl.LocalPlayer.moveable = canMove;
		Object.Destroy(base.gameObject);
		Object.Destroy(DestroyableSingleton<CE_GameSettingsUI>.Instance.gameObject);
	}
}
