using UnityEngine;

public class BanButton : MonoBehaviour
{
	public TextRenderer NameText;

	public SpriteRenderer Background;

	public int TargetClientId;

	public BanMenu Parent
	{
		get;
		set;
	}

	public void Select()
	{
		Background.color = new Color(1f, 1f, 1f, 1f);
		Parent.Select(TargetClientId);
	}

	public void Unselect()
	{
		Background.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
	}
}
