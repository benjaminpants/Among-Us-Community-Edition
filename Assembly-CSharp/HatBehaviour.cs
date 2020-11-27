using System;
using UnityEngine;

// Token: 0x0200009A RID: 154
[CreateAssetMenu]
public class HatBehaviour : ScriptableObject, IBuyable
{
	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000338 RID: 824 RVA: 0x000041DC File Offset: 0x000023DC
	public string ProdId
	{
		get
		{
			return this.ProductId;
		}
	}

	// Token: 0x04000333 RID: 819
	public Sprite MainImage;

	// Token: 0x04000334 RID: 820
	public Sprite FloorImage;

	// Token: 0x04000335 RID: 821
	public bool InFront;

	// Token: 0x04000336 RID: 822
	public bool Free;

	// Token: 0x04000337 RID: 823
	public int LimitedMonth;

	// Token: 0x04000338 RID: 824
	public SkinData RelatedSkin;

	// Token: 0x04000339 RID: 825
	public string StoreName;

	// Token: 0x0400033A RID: 826
	public string ProductId;

	// Token: 0x0400033B RID: 827
	public int Order;
}
