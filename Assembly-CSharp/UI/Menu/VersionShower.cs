using UnityEngine;

public class VersionShower : MonoBehaviour
{
	public TextRenderer text;

	public void Start()
	{
		text.Text = "v0.4.3 - Lua API Beta";
		Screen.sleepTimeout = -1;
		CE_Extensions.OnStartup();
	}
}
