using UnityEngine;

public class TabGroup : MonoBehaviour
{
	public SpriteRenderer Button;

	public ButtonRolloverHandler Rollover;

	public GameObject Content;

	internal void Close()
	{
		Button.color = Color.white;
		Rollover.OutColor = Color.white;
		Content.SetActive(value: false);
	}

	internal void Open()
	{
		Button.color = Color.green;
		Rollover.OutColor = Color.green;
		Content.SetActive(value: true);
	}
}
