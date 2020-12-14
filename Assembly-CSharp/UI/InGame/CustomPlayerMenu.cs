using UnityEngine;

public class CustomPlayerMenu : MonoBehaviour
{
	public TabButton[] Tabs;

	public Sprite NormalColor;

	public Sprite SelectedColor;

	public void Start()
	{
		PlayerControl.LocalPlayer.moveable = false;
	}

	public void OpenTab(GameObject tab)
	{
		for (int i = 0; i < Tabs.Length; i++)
		{
			TabButton tabButton = Tabs[i];
			if (tabButton.Tab == tab)
			{
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
