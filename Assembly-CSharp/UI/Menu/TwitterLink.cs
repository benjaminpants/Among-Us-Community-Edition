using UnityEngine;

public class TwitterLink : MonoBehaviour
{
	public string LinkUrl = "https://twitter.com/OfficialMTM101";

	public void Click()
	{
		Application.OpenURL("https://twitter.com/OfficialMTM101");
	}
}
