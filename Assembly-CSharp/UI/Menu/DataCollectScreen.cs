using System.Collections;
using UnityEngine;

public class DataCollectScreen : MonoBehaviour
{
	public static DataCollectScreen Instance;

	public ToggleButtonBehaviour SendNameButton;

	public ToggleButtonBehaviour SendTelemButton;

	public AdDataCollectScreen AdPolicy;

	private bool QuitGameMode = false;

	private void Start()
	{
		Instance = this;
		UpdateButtons();
	}

	public IEnumerator Show()
	{
		yield return Show(false);
	}

	public IEnumerator Show(bool QuitDialogMode)
	{
		QuitGameMode = QuitDialogMode;
		if (QuitGameMode)
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
		base.gameObject.SetActive(value: false);
	}

	public void ToggleSendTelemetry()
	{
		Close();
	}

	public void ToggleSendName()
	{
		Application.Quit();
	}

	public void UpdateButtons()
	{
		SendNameButton.Text.Text = "Yes";
		SendNameButton.Text.Color = Color.red;
		SendTelemButton.Text.Text = "No";
		SendTelemButton.Text.Color = Color.green;

		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].name == "LinkButton")
			{
				array[i].gameObject.SetActive(false);
			}
			if (array[i].name == "CloseButton")
			{
				array[i].gameObject.SetActive(false);
			}
			if (array[i].name == "TitleText")
			{
				array[i].gameObject.SetActive(false);
			}
			if (array[i].name == "InfoText")
			{
				var textObject = array[i].GetComponent<TextRenderer>();
				textObject.Text = "Are you sure you want to quit?";
			}
		}
	}
}
