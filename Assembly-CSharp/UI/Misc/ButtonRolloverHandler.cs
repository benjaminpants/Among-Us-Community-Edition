using UnityEngine;

public class ButtonRolloverHandler : MonoBehaviour
{
	public SpriteRenderer Target;

	public Color OverColor = Color.green;

	public Color OutColor = Color.white;

	public AudioClip HoverSound;

	public void Start()
	{
		PassiveButton component = GetComponent<PassiveButton>();
		component.OnMouseOver.AddListener(DoMouseOver);
		component.OnMouseOut.AddListener(DoMouseOut);
	}

	public void DoMouseOver()
	{
		Target.color = OverColor;
		if ((bool)HoverSound)
		{
			SoundManager.Instance.PlaySound(HoverSound, loop: false);
		}
	}

	public void DoMouseOut()
	{
		Target.color = OutColor;
	}
}
