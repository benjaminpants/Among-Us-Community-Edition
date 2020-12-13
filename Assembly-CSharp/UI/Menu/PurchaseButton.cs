using UnityEngine;

public class PurchaseButton : MonoBehaviour
{
	public SpriteRenderer PurchasedIcon;

	public TextRenderer NameText;

	public SpriteRenderer HatImage;

	public Sprite MannequinFrame;

	public SpriteRenderer Background;

	public IBuyable Product;

	public bool Purchased;

	public string Name;

	public string Price;

	public string ProductId;

	public StoreMenu Parent
	{
		get;
		set;
	}

	public void SetItem(IBuyable product, string productId, string name, string price, bool purchased)
	{
		Product = product;
		Purchased = purchased;
		Name = name;
		Price = price;
		ProductId = productId;
		PurchasedIcon.enabled = Purchased;
		if (Product is HatBehaviour)
		{
			HatBehaviour hat = (HatBehaviour)Product;
			NameText.gameObject.SetActive(value: false);
			HatImage.transform.parent.gameObject.SetActive(value: true);
			PlayerControl.SetHatImage(hat, HatImage);
			Background.size = new Vector2(0.7f, 0.7f);
			Background.GetComponent<BoxCollider2D>().size = new Vector2(0.7f, 0.7f);
			PurchasedIcon.transform.localPosition = new Vector3(0f, 0f, -1f);
		}
		else if (Product is SkinData)
		{
			SkinData skin = (SkinData)Product;
			NameText.gameObject.SetActive(value: false);
			HatImage.transform.parent.gameObject.SetActive(value: true);
			HatImage.transform.parent.GetComponent<SpriteRenderer>().sprite = MannequinFrame;
			HatImage.transform.parent.localPosition = new Vector3(0f, 0f, -0.01f);
			HatImage.transform.parent.localScale = Vector3.one * 0.3f;
			HatImage.transform.localPosition = new Vector3(0f, 0f, -0.01f);
			HatImage.transform.localScale = Vector3.one * 2f;
			PlayerControl.SetSkinImage(skin, HatImage);
			Background.size = new Vector2(0.7f, 0.7f);
			Background.GetComponent<BoxCollider2D>().size = new Vector2(0.7f, 0.7f);
			PurchasedIcon.transform.localPosition = new Vector3(0f, 0f, -1f);
		}
		else
		{
			NameText.Text = Name;
		}
	}

	internal void SetPurchased()
	{
		Purchased = true;
		PurchasedIcon.enabled = true;
	}

	public void DoPurchase()
	{
		Parent.SetProduct(this);
	}
}
