using UnityEngine;

public class VersionShower : MonoBehaviour
{
	public TextRenderer text;

	public void Start()
	{
		text.Text = "v" + Application.version + "." + 0;
		Screen.sleepTimeout = -1;
	}
}
