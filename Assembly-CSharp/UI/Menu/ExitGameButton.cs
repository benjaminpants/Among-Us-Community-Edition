using UnityEngine;

public class ExitGameButton : MonoBehaviour
{
	public void Start()
	{
		if (!DestroyableSingleton<HudManager>.InstanceExists)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void OnClick()
	{
		if ((bool)AmongUsClient.Instance)
		{
			AmongUsClient.Instance.ExitGame();
		}
		else
		{
			SceneChanger.ChangeScene("MainMenu");
		}
	}
}
