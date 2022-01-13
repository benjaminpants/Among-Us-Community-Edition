using UnityEngine;

public class VersionShower : MonoBehaviour
{
	public TextRenderer text;

	public void Start()
	{
		text.Text = "v0.1.0 - First Release DONT LEAK!";
		Screen.sleepTimeout = -1;
		CE_CustomMapManager.Initialize();
                CE_Extensions.OnStartup();
	}

	public void Update()
    {
		CE_Extensions.UpdateWindowTitle();
	}

}
