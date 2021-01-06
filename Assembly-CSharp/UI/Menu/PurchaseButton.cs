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

	public SpriteRenderer HatImageExt;

	public SpriteRenderer HatImageExt2;

	public SpriteRenderer HatImageExt3;

	public SpriteRenderer HatImageExt4;


	public StoreMenu Parent
	{
		get;
		set;
	}

	public void Update()
    {
		UpdateExtHats();
	}

	public void SetItem(IBuyable product, string productId, string name, string price, bool purchased)
	{
		Product = product;
		Purchased = purchased;
		Name = name;
		Price = price;
		ProductId = productId;
		PurchasedIcon.enabled = false;
		if (Product is HatBehaviour)
		{
			HatBehaviour hat = (HatBehaviour)Product;
			NameText.gameObject.SetActive(value: false);
			HatImage.transform.parent.gameObject.SetActive(value: true);
			SetHatImage(hat);
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
		PurchasedIcon.enabled = false;
	}

	public void DoPurchase()
	{
		Parent.SetProduct(this);
	}

	public void UpdateExtHats()
	{
		if (Product is HatBehaviour)
		{
			CE_WardrobeManager.MatchBaseHatRender(HatImageExt, HatImage);
			CE_WardrobeManager.MatchBaseHatRender(HatImageExt2, HatImage);
			CE_WardrobeManager.MatchBaseHatRender(HatImageExt3, HatImage);
			CE_WardrobeManager.MatchBaseHatRender(HatImageExt4, HatImage);
		}
	}

	public void Awake()
	{
		HatImageExt = CE_WardrobeManager.CreateExtSpriteRender(HatImage);
		HatImageExt2 = CE_WardrobeManager.CreateExtSpriteRender(HatImage);
		HatImageExt3 = CE_WardrobeManager.CreateExtSpriteRender(HatImage);
		HatImageExt4 = CE_WardrobeManager.CreateExtSpriteRender(HatImage);
	}

	public void SetHatImage(HatBehaviour hatId)
	{
		CE_WardrobeManager.SetExtHatImage(hatId, HatImage, 0, (int)SaveManager.BodyColor);
		CE_WardrobeManager.SetExtHatImage(hatId, HatImageExt, 1, (int)SaveManager.BodyColor);
		CE_WardrobeManager.SetExtHatImage(hatId, HatImageExt2, 2, (int)SaveManager.BodyColor);
		CE_WardrobeManager.SetExtHatImage(hatId, HatImageExt3, 3, (int)SaveManager.BodyColor);
		CE_WardrobeManager.SetExtHatImage(hatId, HatImageExt4, 4, (int)SaveManager.BodyColor);
	}
}
