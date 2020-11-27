using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

// Token: 0x0200015B RID: 347
public class StoreMenu : MonoBehaviour, IStoreListener
{
	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000720 RID: 1824 RVA: 0x0000668B File Offset: 0x0000488B
	// (set) Token: 0x06000721 RID: 1825 RVA: 0x00006693 File Offset: 0x00004893
	public PurchaseStates PurchaseState { get; private set; }

	// Token: 0x06000722 RID: 1826 RVA: 0x00029378 File Offset: 0x00027578
	public void Start()
	{
		ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(), new IPurchasingModule[] { });
		configurationBuilder.AddProduct("bought_ads", ProductType.NonConsumable);
		foreach (HatBehaviour hatBehaviour in DestroyableSingleton<HatManager>.Instance.AllHats)
		{
			if (!hatBehaviour.Free)
			{
				configurationBuilder.AddProduct(hatBehaviour.ProductId, ProductType.NonConsumable);
			}
		}
		foreach (SkinData skinData in DestroyableSingleton<HatManager>.Instance.AllSkins)
		{
			if (!skinData.Free)
			{
				configurationBuilder.AddProduct(skinData.ProdId, ProductType.NonConsumable);
			}
		}
		UnityPurchasing.Initialize(this, configurationBuilder);
		this.PurchaseBackground.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		this.PriceText.Color = new Color(0.8f, 0.8f, 0.8f, 1f);
		this.PriceText.Text = "";
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x0000669C File Offset: 0x0000489C
	public void Update()
	{
		this.TopArrow.enabled = !this.Scroller.AtTop;
		this.BottomArrow.enabled = !this.Scroller.AtBottom;
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x000294B4 File Offset: 0x000276B4
	public void RestorePurchases()
	{
		if (!StoreMenu.ConfirmedPurchases)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
			{
				this.extensions.GetExtension<IAppleExtensions>().RestoreTransactions(delegate(bool result)
				{
					if (!result)
					{
						this.LoadingText.gameObject.SetActive(true);
						this.LoadingText.Text = "Couldn't restore purchases";
						StoreMenu.ConfirmedPurchases = false;
						this.DestroySliderObjects();
						this.RestorePurchasesButton.transform.parent.gameObject.SetActive(true);
						return;
					}
					this.FinishRestoring();
				});
			}
			else
			{
				this.FinishRestoring();
			}
			StoreMenu.ConfirmedPurchases = true;
			this.RestorePurchasesButton.transform.parent.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00029520 File Offset: 0x00027720
	private void DestroySliderObjects()
	{
		for (int i = 0; i < this.AllObjects.Count; i++)
		{
			UnityEngine.Object.Destroy(this.AllObjects[i]);
		}
		this.AllObjects.Clear();
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x000066D0 File Offset: 0x000048D0
	private void FinishRestoring()
	{
		this.DestroySliderObjects();
		this.ShowAllButtons();
		this.RestorePurchasesButton.Text = "Purchases Restored";
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00029560 File Offset: 0x00027760
	public void SetProduct(PurchaseButton button)
	{
		if (this.PurchaseState == PurchaseStates.Started)
		{
			return;
		}
		this.CurrentButton = button;
		if (this.CurrentButton.Product is HatBehaviour)
		{
			HatBehaviour hatBehaviour = (HatBehaviour)this.CurrentButton.Product;
			this.HatSlot.gameObject.SetActive(true);
			this.SkinSlot.gameObject.SetActive(false);
			PlayerControl.SetHatImage(hatBehaviour, this.HatSlot);
			this.ItemName.Text = (string.IsNullOrWhiteSpace(hatBehaviour.StoreName) ? hatBehaviour.name : hatBehaviour.StoreName);
			if (hatBehaviour.RelatedSkin)
			{
				TextRenderer itemName = this.ItemName;
				itemName.Text += " (Includes skin!)";
				this.SkinSlot.gameObject.SetActive(true);
				PlayerControl.SetSkinImage(hatBehaviour.RelatedSkin, this.SkinSlot);
			}
		}
		else if (this.CurrentButton.Product is SkinData)
		{
			SkinData skinData = (SkinData)this.CurrentButton.Product;
			this.SkinSlot.gameObject.SetActive(true);
			this.HatSlot.gameObject.SetActive(true);
			PlayerControl.SetHatImage(skinData.RelatedHat, this.HatSlot);
			PlayerControl.SetSkinImage(skinData, this.SkinSlot);
			this.ItemName.Text = (string.IsNullOrWhiteSpace(skinData.StoreName) ? skinData.name : skinData.StoreName);
		}
		else
		{
			this.HatSlot.gameObject.SetActive(false);
			this.SkinSlot.gameObject.SetActive(false);
			this.ItemName.Text = "Remove All Ads";
		}
		if (button.Purchased)
		{
			this.PurchaseBackground.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			this.PriceText.Color = new Color(0.8f, 0.8f, 0.8f, 1f);
			this.PriceText.Text = "Owned";
			return;
		}
		this.PurchaseBackground.color = Color.white;
		this.PriceText.Color = Color.white;
		this.PriceText.Text = button.Price;
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x000066EE File Offset: 0x000048EE
	public void BuyProduct()
	{
		if (!this.CurrentButton || this.CurrentButton.Purchased || this.PurchaseState == PurchaseStates.Started)
		{
			return;
		}
		base.StartCoroutine(this.WaitForPurchaseAds(this.CurrentButton));
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00006727 File Offset: 0x00004927
	public IEnumerator WaitForPurchaseAds(PurchaseButton button)
	{
		this.PurchaseState = PurchaseStates.Started;
		this.controller.InitiatePurchase(button.ProductId);
		while (this.PurchaseState == PurchaseStates.Started)
		{
			yield return null;
		}
		if (this.PurchaseState == PurchaseStates.Success)
		{
			foreach (PurchaseButton purchaseButton in from p in this.AllObjects
			select p.GetComponent<PurchaseButton>() into h
			where h && h.ProductId == button.ProductId
			select h)
			{
				purchaseButton.SetPurchased();
			}
		}
		this.SetProduct(button);
		yield break;
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00029798 File Offset: 0x00027998
	public void Close()
	{
		HatsTab hatsTab = UnityEngine.Object.FindObjectOfType<HatsTab>();
		if (hatsTab)
		{
			hatsTab.OnDisable();
			hatsTab.OnEnable();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x000297CC File Offset: 0x000279CC
	private void ShowAllButtons()
	{
		this.LoadingText.gameObject.SetActive(false);
		Vector3 vector = this.StartPositionVertical;
		this.RestorePurchasesButton.transform.parent.gameObject.SetActive(false);
		this.HorizontalLine.gameObject.SetActive(false);
		Product[] array = this.controller.products.all.ToArray<Product>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].hasReceipt)
			{
				SaveManager.SetPurchased(array[i].definition.id);
			}
		}
		ScriptableObject[] hats;
		if (DateTime.UtcNow.Month == 12)
		{
			HatBehaviour[] array2 = (from h in DestroyableSingleton<HatManager>.Instance.AllHats
			where h.LimitedMonth == 12
			select h).ToArray<HatBehaviour>();
			vector = this.InsertBanner(vector, this.HolidayBanner);
			vector.y += -0.375f;
			Vector3 position = vector;
			Product[] allProducts = array;
			hats = array2;
			vector = this.InsertHatsFromList(position, allProducts, hats);
		}
		vector.y += -0.375f;
		SkinData[] array3 = (from h in DestroyableSingleton<HatManager>.Instance.AllSkins
		where !h.Free
		select h).ToArray<SkinData>();
		vector = this.InsertBanner(vector, this.SkinsBanner);
		Vector3 position2 = vector;
		Product[] allProducts2 = array;
		hats = array3;
		vector = this.InsertHatsFromList(position2, allProducts2, hats);
		HatBehaviour[] array4 = (from h in DestroyableSingleton<HatManager>.Instance.AllHats
		where h.LimitedMonth == 0
		select h).ToArray<HatBehaviour>();
		vector = this.InsertBanner(vector, this.HatBanner);
		vector.y += -0.375f;
		Vector3 position3 = vector;
		Product[] allProducts3 = array;
		hats = array4;
		vector = this.InsertHatsFromList(position3, allProducts3, hats);
		this.Scroller.YBounds.max = Mathf.Max(0f, -vector.y - 2.5f);
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x000299C4 File Offset: 0x00027BC4
	private Vector3 InsertHatsFromList(Vector3 position, Product[] allProducts, ScriptableObject[] hats)
	{
		int num = 0;
		for (int i = 0; i < hats.Length; i++)
		{
			IBuyable item = hats[i] as IBuyable;
			Product product = allProducts.FirstOrDefault((Product p) => item.ProdId == p.definition.id);
			if (product != null && product.availableToPurchase)
			{
				int num2 = num % this.NumPerRow;
				position.x = this.StartPositionVertical.x + this.XRange.Lerp((float)num2 / ((float)this.NumPerRow - 1f));
				if (num2 == 0 && num > 1)
				{
					position.y += -0.75f;
				}
				this.InsertProduct(position, product, item);
				num++;
			}
		}
		position.y += -0.75f;
		return position;
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00029A90 File Offset: 0x00027C90
	private void InsertProduct(Vector3 position, Product product, IBuyable item)
	{
		PurchaseButton purchaseButton = UnityEngine.Object.Instantiate<PurchaseButton>(this.PurchasablePrefab, this.Scroller.Inner);
		this.AllObjects.Add(purchaseButton.gameObject);
		purchaseButton.transform.localPosition = position;
		purchaseButton.Parent = this;
		purchaseButton.SetItem(item, product.definition.id, product.metadata.localizedTitle.Replace("(Among Us)", ""), product.metadata.localizedPriceString, product.hasReceipt || SaveManager.GetPurchase(product.definition.id));
		if (product.hasReceipt)
		{
			SaveManager.SetPurchased(product.definition.id);
		}
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00029B44 File Offset: 0x00027D44
	private Vector3 InsertBanner(Vector3 position, Sprite s)
	{
		position.x = this.StartPositionVertical.x;
		SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate<SpriteRenderer>(this.BannerPrefab, this.Scroller.Inner);
		spriteRenderer.sprite = s;
		spriteRenderer.transform.localPosition = position;
		position.y += -spriteRenderer.sprite.bounds.size.y;
		this.AllObjects.Add(spriteRenderer.gameObject);
		return position;
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x0000673D File Offset: 0x0000493D
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		this.controller = controller;
		this.extensions = extensions;
		this.ShowAllButtons();
		if (StoreMenu.ConfirmedPurchases)
		{
			this.RestorePurchasesButton.Text = "Purchases Restored";
		}
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x0000676A File Offset: 0x0000496A
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		Debug.Log("Purchased product:" + e.purchasedProduct.metadata.localizedTitle);
		SaveManager.SetPurchased(e.purchasedProduct.definition.id);
		this.PurchaseState = PurchaseStates.Success;
		return PurchaseProcessingResult.Complete;
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x000067A8 File Offset: 0x000049A8
	public void OnInitializeFailed(InitializationFailureReason error)
	{
		if (error == InitializationFailureReason.NoProductsAvailable)
		{
			this.LoadingText.Text = "Coming Soon!";
			return;
		}
		this.LoadingText.Text = "Loading Failed";
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x000067CF File Offset: 0x000049CF
	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		Debug.LogError("Failed: " + p);
		this.PurchaseState = PurchaseStates.Fail;
	}

	// Token: 0x040006F2 RID: 1778
	private static bool ConfirmedPurchases;

	// Token: 0x040006F3 RID: 1779
	public SpriteRenderer HatSlot;

	// Token: 0x040006F4 RID: 1780
	public SpriteRenderer SkinSlot;

	// Token: 0x040006F5 RID: 1781
	public TextRenderer ItemName;

	// Token: 0x040006F6 RID: 1782
	public SpriteRenderer PurchaseBackground;

	// Token: 0x040006F7 RID: 1783
	public TextRenderer PriceText;

	// Token: 0x040006F8 RID: 1784
	public PurchaseButton PurchasablePrefab;

	// Token: 0x040006F9 RID: 1785
	public TextRenderer LoadingText;

	// Token: 0x040006FA RID: 1786
	public TextRenderer RestorePurchasesButton;

	// Token: 0x040006FB RID: 1787
	public SpriteRenderer HorizontalLine;

	// Token: 0x040006FC RID: 1788
	public SpriteRenderer BannerPrefab;

	// Token: 0x040006FD RID: 1789
	public Sprite HatBanner;

	// Token: 0x040006FE RID: 1790
	public Sprite SkinsBanner;

	// Token: 0x040006FF RID: 1791
	public Sprite HolidayBanner;

	// Token: 0x04000700 RID: 1792
	public SpriteRenderer TopArrow;

	// Token: 0x04000701 RID: 1793
	public SpriteRenderer BottomArrow;

	// Token: 0x04000702 RID: 1794
	public const string BoughtAdsProductId = "bought_ads";

	// Token: 0x04000703 RID: 1795
	private IStoreController controller;

	// Token: 0x04000704 RID: 1796
	private IExtensionProvider extensions;

	// Token: 0x04000706 RID: 1798
	public Scroller Scroller;

	// Token: 0x04000707 RID: 1799
	public Vector2 StartPositionVertical;

	// Token: 0x04000708 RID: 1800
	public FloatRange XRange = new FloatRange(-1f, 1f);

	// Token: 0x04000709 RID: 1801
	public int NumPerRow = 4;

	// Token: 0x0400070A RID: 1802
	private PurchaseButton CurrentButton;

	// Token: 0x0400070B RID: 1803
	private List<GameObject> AllObjects = new List<GameObject>();

	// Token: 0x0400070C RID: 1804
	private const bool IgnoreMonth = false;

	// Token: 0x0400070D RID: 1805
	private const float NormalHeight = -0.45f;

	// Token: 0x0400070E RID: 1806
	private const float BoxHeight = -0.75f;
}
