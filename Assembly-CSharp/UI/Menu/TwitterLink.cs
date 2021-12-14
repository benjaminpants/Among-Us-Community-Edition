using UnityEngine;

public class TwitterLink : MonoBehaviour
{
	public string LinkUrl = "https://among-us.fandom.com/wiki/Voting";

	public void Click()
	{
		Application.OpenURL(LinkUrl);
	}
}
