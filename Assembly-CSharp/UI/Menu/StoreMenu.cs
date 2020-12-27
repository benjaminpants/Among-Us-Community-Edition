using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public class StoreMenu : MonoBehaviour, IStoreListener
{
	private static bool ConfirmedPurchases;

	public SpriteRenderer HatSlot;

	public SpriteRenderer SkinSlot;

	public TextRenderer ItemName;

	public SpriteRenderer PurchaseBackground;

	public TextRenderer PriceText;

	public PurchaseButton PurchasablePrefab;

	public TextRenderer LoadingText;

	public TextRenderer RestorePurchasesButton;

	public SpriteRenderer HorizontalLine;

	public SpriteRenderer BannerPrefab;

	public Sprite HatBanner;

	public Sprite SkinsBanner;

	public Sprite HolidayBanner;

	public SpriteRenderer TopArrow;

	public SpriteRenderer BottomArrow;

	public const string BoughtAdsProductId = "bought_ads";

	private IStoreController controller;

	private IExtensionProvider extensions;

	public Scroller Scroller;

	public Vector2 StartPositionVertical;

	public FloatRange XRange = new FloatRange(-1f, 1f);

	public int NumPerRow = 4;

	private PurchaseButton CurrentButton;

	private List<GameObject> AllObjects = new List<GameObject>();

	private const bool IgnoreMonth = false;

	private const float NormalHeight = -0.45f;

	private const float BoxHeight = -0.75f;

	public PurchaseStates PurchaseState
	{
		get;
		private set;
	}

	public void Start()
	{
		ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		configurationBuilder.AddProduct("bought_ads", ProductType.NonConsumable);
		foreach (HatBehaviour allHat in DestroyableSingleton<HatManager>.Instance.AllHats)
		{
			if (!allHat.Free)
			{
				configurationBuilder.AddProduct(allHat.ProductId, ProductType.NonConsumable);
			}
		}
		foreach (SkinData allSkin in DestroyableSingleton<HatManager>.Instance.AllSkins)
		{
			if (!allSkin.Free)
			{
				configurationBuilder.AddProduct(allSkin.ProdId, ProductType.NonConsumable);
			}
		}
		UnityPurchasing.Initialize(this, configurationBuilder);
		PurchaseBackground.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		PriceText.Color = new Color(0.8f, 0.8f, 0.8f, 1f);
		PriceText.Text = "";
	}

	public void Update()
	{
		TopArrow.enabled = !Scroller.AtTop;
		BottomArrow.enabled = !Scroller.AtBottom;
	}

	public void RestorePurchases()
	{
		if (ConfirmedPurchases)
		{
			return;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
		{
			extensions.GetExtension<IAppleExtensions>().RestoreTransactions(delegate (bool result)
			{
				if (!result)
				{
					LoadingText.gameObject.SetActive(value: true);
					LoadingText.Text = "Couldn't restore purchases";
					ConfirmedPurchases = false;
					DestroySliderObjects();
					RestorePurchasesButton.transform.parent.gameObject.SetActive(value: true);
				}
				else
				{
					FinishRestoring();
				}
			});
		}
		else
		{
			FinishRestoring();
		}
		ConfirmedPurchases = true;
		RestorePurchasesButton.transform.parent.gameObject.SetActive(value: false);
	}

	private void DestroySliderObjects()
	{
		for (int i = 0; i < AllObjects.Count; i++)
		{
			UnityEngine.Object.Destroy(AllObjects[i]);
		}
		AllObjects.Clear();
	}

	private void FinishRestoring()
	{
		DestroySliderObjects();
		ShowAllButtons();
		RestorePurchasesButton.Text = "Purchases Restored";
	}

	public void SetProduct(PurchaseButton button)
	{
		if (PurchaseState == PurchaseStates.Started)
		{
			return;
		}
		CurrentButton = button;
		if (CurrentButton.Product is HatBehaviour)
		{
			HatBehaviour hatBehaviour = (HatBehaviour)CurrentButton.Product;
			HatSlot.gameObject.SetActive(value: true);
			SkinSlot.gameObject.SetActive(value: false);
			PlayerControl.SetHatImage(hatBehaviour, HatSlot);
			ItemName.Text = (string.IsNullOrWhiteSpace(hatBehaviour.StoreName) ? hatBehaviour.name : hatBehaviour.StoreName);
			if ((bool)hatBehaviour.RelatedSkin)
			{
				ItemName.Text += " (Includes skin!)";
				SkinSlot.gameObject.SetActive(value: true);
				PlayerControl.SetSkinImage(hatBehaviour.RelatedSkin, SkinSlot);
			}
		}
		else if (CurrentButton.Product is SkinData)
		{
			SkinData skinData = (SkinData)CurrentButton.Product;
			SkinSlot.gameObject.SetActive(value: true);
			HatSlot.gameObject.SetActive(value: true);
			PlayerControl.SetHatImage(skinData.RelatedHat, HatSlot);
			PlayerControl.SetSkinImage(skinData, SkinSlot);
			ItemName.Text = (string.IsNullOrWhiteSpace(skinData.StoreName) ? skinData.name : skinData.StoreName);
		}
		else
		{
			HatSlot.gameObject.SetActive(value: false);
			SkinSlot.gameObject.SetActive(value: false);
			ItemName.Text = "Remove All Ads";
		}
		if (button.Purchased)
		{
			PurchaseBackground.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			PriceText.Color = new Color(0.8f, 0.8f, 0.8f, 1f);
			PriceText.Text = "Owned";
		}
		else
		{
			PurchaseBackground.color = Color.white;
			PriceText.Color = Color.white;
			PriceText.Text = button.Price;
		}
	}

	public void BuyProduct()
	{
		if ((bool)CurrentButton && !CurrentButton.Purchased && PurchaseState != PurchaseStates.Started)
		{
			StartCoroutine(WaitForPurchaseAds(CurrentButton));
		}
	}

	public IEnumerator WaitForPurchaseAds(PurchaseButton button)
	{
		PurchaseState = PurchaseStates.Started;
		controller.InitiatePurchase(button.ProductId);
		while (PurchaseState == PurchaseStates.Started)
		{
			yield return null;
		}
		if (PurchaseState == PurchaseStates.Success)
		{
			foreach (PurchaseButton item in from p in AllObjects
											select p.GetComponent<PurchaseButton>() into h
											where (bool)h && h.ProductId == button.ProductId
											select h)
			{
				item.SetPurchased();
			}
		}
		SetProduct(button);
	}

	public void Close()
	{
		HatsTab hatsTab = UnityEngine.Object.FindObjectOfType<HatsTab>();
		if ((bool)hatsTab)
		{
			hatsTab.OnDisable();
			hatsTab.OnEnable();
		}
		base.gameObject.SetActive(value: false);
	}

	private void ShowAllButtons()
	{
		LoadingText.gameObject.SetActive(value: false);
		Vector3 vector = StartPositionVertical;
		RestorePurchasesButton.transform.parent.gameObject.SetActive(value: false);
		HorizontalLine.gameObject.SetActive(value: false);
		Product[] array = controller.products.all.ToArray();
		ScriptableObject[] hats;
		vector.y += -0.375f;
		SkinData[] array3 = DestroyableSingleton<HatManager>.Instance.AllSkins.Where((SkinData h) => !h.Free).ToArray();
		vector = InsertBanner(vector, SkinsBanner);
		Vector3 position2 = vector;
		hats = array3;
		vector = InsertHatsFromList(position2, array, hats);
		HatBehaviour[] array4 = DestroyableSingleton<HatManager>.Instance.AllHats.ToArray();
		vector = InsertBanner(vector, HatBanner);
		vector.y += -0.375f;
		Vector3 position3 = vector;
		hats = array4;
		vector = InsertHatsFromList(position3, array, hats);
		Scroller.YBounds.max = Mathf.Max(0f, 0f - vector.y - 2.5f);
	}

	private Vector3 InsertHatsFromList(Vector3 position, Product[] allProducts, ScriptableObject[] hats)
	{
		int num = 0;
		for (int i = 0; i < hats.Length; i++)
		{
			IBuyable item = hats[i] as IBuyable;
			Product product = allProducts.FirstOrDefault((Product p) => item.ProdId == p.definition.id);
			if (product != null)
			{
				int num2 = num % NumPerRow;
				position.x = StartPositionVertical.x + XRange.Lerp((float)num2 / ((float)NumPerRow - 1f));
				if (num2 == 0 && num > 1)
				{
					position.y += -0.75f;
				}
				InsertProduct(position, product, item);
				num++;
			}
		}
		position.y += -0.75f;
		return position;
	}

	private void InsertProduct(Vector3 position, Product product, IBuyable item)
	{
		PurchaseButton purchaseButton = UnityEngine.Object.Instantiate(PurchasablePrefab, Scroller.Inner);
		AllObjects.Add(purchaseButton.gameObject);
		purchaseButton.transform.localPosition = position;
		purchaseButton.Parent = this;
		purchaseButton.SetItem(item, product.definition.id, product.metadata.localizedTitle.Replace("(Among Us)", ""), product.metadata.localizedPriceString, true);
		SaveManager.SetPurchased(product.definition.id);

	}

	private Vector3 InsertBanner(Vector3 position, Sprite s)
	{
		position.x = StartPositionVertical.x;
		SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate(BannerPrefab, Scroller.Inner);
		spriteRenderer.sprite = s;
		spriteRenderer.transform.localPosition = position;
		position.y += 0f - spriteRenderer.sprite.bounds.size.y;
		AllObjects.Add(spriteRenderer.gameObject);
		return position;
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		this.controller = controller;
		this.extensions = extensions;
		ShowAllButtons();
		if (ConfirmedPurchases)
		{
			RestorePurchasesButton.Text = "Purchases Restored";
		}
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		Debug.Log("Purchased product:" + e.purchasedProduct.metadata.localizedTitle);
		SaveManager.SetPurchased(e.purchasedProduct.definition.id);
		PurchaseState = PurchaseStates.Success;
		return PurchaseProcessingResult.Complete;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		if (error == InitializationFailureReason.NoProductsAvailable)
		{
			LoadingText.Text = "Coming Soon!";
		}
		else
		{
			LoadingText.Text = "Loading Failed";
		}
	}

	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		Debug.LogError("Failed: " + p);
		PurchaseState = PurchaseStates.Fail;
	}
}
