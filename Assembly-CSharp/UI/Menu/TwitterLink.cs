using UnityEngine;

public class TwitterLink : MonoBehaviour
{
	public string LinkUrl = "https://github.com/";

	public void Click()
	{
		Application.OpenURL("https://github.com/");
	}
}
