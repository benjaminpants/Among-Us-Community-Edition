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

	public bool NoBobbing;

	public bool IsColorFiltered;

	public bool IsCustom;

	public string RelatedSkinName;

    #region Extra Hat Layer 1

    public Sprite MainImageExt;

	public Sprite FloorImageExt;

	public Sprite PreviewImageExt;

	public bool InFrontExt;

	public bool NoBobbingExt;

	public bool IsColorFilteredExt;

	#endregion

	#region Extra Hat Layer 2

	public Sprite MainImageExt2;

	public Sprite FloorImageExt2;

	public Sprite PreviewImageExt2;

	public bool InFrontExt2;

	public bool NoBobbingExt2;

	public bool IsColorFilteredExt2;

	#endregion

	#region Extra Hat Layer 3

	public Sprite MainImageExt3;

	public Sprite FloorImageExt3;

	public Sprite PreviewImageExt3;

	public bool InFrontExt3;

	public bool NoBobbingExt3;

	public bool IsColorFilteredExt3;

	#endregion

	#region Extra Hat Layer 4

	public Sprite MainImageExt4;

	public Sprite FloorImageExt4;

	public Sprite PreviewImageExt4;

	public bool InFrontExt4;

	public bool NoBobbingExt4;

	public bool IsColorFilteredExt4;

	#endregion

	public string ProdId => ProductId;
}
