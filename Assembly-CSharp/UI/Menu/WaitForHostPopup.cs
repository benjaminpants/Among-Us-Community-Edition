using UnityEngine;

public class WaitForHostPopup : DestroyableSingleton<WaitForHostPopup>
{
	public GameObject Content;

	public void Show()
	{
		if ((bool)AmongUsClient.Instance && AmongUsClient.Instance.ClientId > 0)
		{
			Content.SetActive(value: true);
		}
	}

	public void ExitGame()
	{
		AmongUsClient.Instance.ExitGame();
		Content.SetActive(value: false);
	}

	public void Hide()
	{
		Content.SetActive(value: false);
	}
}
