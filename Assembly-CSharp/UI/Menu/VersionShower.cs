using UnityEngine;

public class VersionShower : MonoBehaviour
{
	public TextRenderer text;

	public void Start()
	{
		text.Text = "v0.5.1 - New Settings + Fix Attempt 1";
		Screen.sleepTimeout = -1;
        CE_Extensions.OnStartup();
	}

	public void Update()
    {
		CE_Extensions.UpdateWindowTitle();
	}

}
