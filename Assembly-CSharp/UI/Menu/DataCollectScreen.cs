using System.Collections;
using UnityEngine;

public class DataCollectScreen : MonoBehaviour
{
	public static DataCollectScreen Instance;

	public ToggleButtonBehaviour SendNameButton;

	public ToggleButtonBehaviour SendTelemButton;

	public AdDataCollectScreen AdPolicy;

	private void Start()
	{
		Instance = this;
		UpdateButtons();
	}

	public IEnumerator Show()
	{
		if (!SaveManager.SendDataScreen)
		{
			base.gameObject.SetActive(value: true);
			while (base.gameObject.activeSelf)
			{
				yield return null;
			}
		}
	}

	public void Close()
	{
		SaveManager.SendDataScreen = true;
	}

	public void ToggleSendTelemetry()
	{
		SaveManager.SendTelemetry = !SaveManager.SendTelemetry;
		UpdateButtons();
	}

	public void ToggleSendName()
	{
		SaveManager.SendName = !SaveManager.SendName;
		UpdateButtons();
	}

	public void UpdateButtons()
	{
		SendNameButton.UpdateText(SaveManager.SendName);
		SendTelemButton.UpdateText(SaveManager.SendTelemetry);
	}
}
