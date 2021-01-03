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

	public bool NoBobbing;

	public Sprite PreviewImage;

	public bool IsCustom;

	public string RelatedSkinName;

    #region Extra Hat Layers

    public Sprite MainImageExt;

	public Sprite FloorImageExt;

	public Sprite PreviewImageExt;

	public bool InFrontExt;

	public bool NoBobbingExt;

	public Sprite MainImageExt2;

	public Sprite FloorImageExt2;

	public Sprite PreviewImageExt2;

	public bool InFrontExt2;

	public bool NoBobbingExt2;

	public Sprite MainImageExt3;

	public Sprite FloorImageExt3;

	public Sprite PreviewImageExt3;

	public bool InFrontExt3;

	public bool NoBobbingExt3;

	public Sprite MainImageExt4;

	public Sprite FloorImageExt4;

	public Sprite PreviewImageExt4;

	public bool InFrontExt4;

	public bool NoBobbingExt4;

    #endregion

    public string ProdId => ProductId;
}
