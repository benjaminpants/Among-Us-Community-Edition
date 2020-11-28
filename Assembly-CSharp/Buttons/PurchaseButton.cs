using System;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class PurchaseButton : MonoBehaviour
{
	// Token: 0x1700010F RID: 271
	// (get) Token: 0x0600071A RID: 1818 RVA: 0x00006657 File Offset: 0x00004857
	// (set) Token: 0x0600071B RID: 1819 RVA: 0x0000665F File Offset: 0x0000485F
	public StoreMenu Parent { get; set; }

	// Token: 0x0600071C RID: 1820 RVA: 0x00029114 File Offset: 0x00027314
	public void SetItem(IBuyable product, string productId, string name, string price, bool purchased)
	{
		this.Product = product;
		this.Purchased = purchased;
		this.Name = name;
		this.Price = price;
		this.ProductId = productId;
		this.PurchasedIcon.enabled = this.Purchased;
		if (this.Product is HatBehaviour)
		{
			HatBehaviour hat = (HatBehaviour)this.Product;
			this.NameText.gameObject.SetActive(false);
			this.HatImage.transform.parent.gameObject.SetActive(true);
			PlayerControl.SetHatImage(hat, this.HatImage);
			this.Background.size = new Vector2(0.7f, 0.7f);
			this.Background.GetComponent<BoxCollider2D>().size = new Vector2(0.7f, 0.7f);
			this.PurchasedIcon.transform.localPosition = new Vector3(0f, 0f, -1f);
			return;
		}
		if (this.Product is SkinData)
		{
			SkinData skin = (SkinData)this.Product;
			this.NameText.gameObject.SetActive(false);
			this.HatImage.transform.parent.gameObject.SetActive(true);
			this.HatImage.transform.parent.GetComponent<SpriteRenderer>().sprite = this.MannequinFrame;
			this.HatImage.transform.parent.localPosition = new Vector3(0f, 0f, -0.01f);
			this.HatImage.transform.parent.localScale = Vector3.one * 0.3f;
			this.HatImage.transform.localPosition = new Vector3(0f, 0f, -0.01f);
			this.HatImage.transform.localScale = Vector3.one * 2f;
			PlayerControl.SetSkinImage(skin, this.HatImage);
			this.Background.size = new Vector2(0.7f, 0.7f);
			this.Background.GetComponent<BoxCollider2D>().size = new Vector2(0.7f, 0.7f);
			this.PurchasedIcon.transform.localPosition = new Vector3(0f, 0f, -1f);
			return;
		}
		this.NameText.Text = this.Name;
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x00006668 File Offset: 0x00004868
	internal void SetPurchased()
	{
		this.Purchased = true;
		this.PurchasedIcon.enabled = true;
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x0000667D File Offset: 0x0000487D
	public void DoPurchase()
	{
		this.Parent.SetProduct(this);
	}

	// Token: 0x040006E1 RID: 1761
	public SpriteRenderer PurchasedIcon;

	// Token: 0x040006E2 RID: 1762
	public TextRenderer NameText;

	// Token: 0x040006E3 RID: 1763
	public SpriteRenderer HatImage;

	// Token: 0x040006E4 RID: 1764
	public Sprite MannequinFrame;

	// Token: 0x040006E5 RID: 1765
	public SpriteRenderer Background;

	// Token: 0x040006E6 RID: 1766
	public IBuyable Product;

	// Token: 0x040006E7 RID: 1767
	public bool Purchased;

	// Token: 0x040006E8 RID: 1768
	public string Name;

	// Token: 0x040006E9 RID: 1769
	public string Price;

	// Token: 0x040006EA RID: 1770
	public string ProductId;
}
