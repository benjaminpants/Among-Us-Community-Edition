using UnityEngine;

[CreateAssetMenu]
public class HatBehaviour : ScriptableObject, IBuyable
{
	public Sprite MainImage;

	public Sprite FloorImage;

	public bool InFront;

	public bool Free;

	public int LimitedMonth;

	public SkinData RelatedSkin;

	public string StoreName;

	public string ProductId;

	public int Order;

	public Sprite PreviewImage;

	public bool IsCustom;

	public string ProdId => ProductId;
}
