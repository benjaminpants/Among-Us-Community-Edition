using UnityEngine;

public class VersionShower : MonoBehaviour
{
	public TextRenderer text;

	public void Start()
	{
		text.Text = "v0.5.2 - The Shields and Bug-squashing Update";
		Screen.sleepTimeout = -1;
        CE_Extensions.OnStartup();
	}

	public void Update()
    {
		CE_Extensions.UpdateWindowTitle();
	}

}
