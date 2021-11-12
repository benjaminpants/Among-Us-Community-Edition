using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Analytics;

public class MainMenuManager : MonoBehaviour
{
	public DataCollectScreen DataPolicy;

	public AdDataCollectScreen AdsPolicy;

	public AnnouncementPopUp Announcement;

	public UnlockPopUp UnlockPop;

	private static bool SentTelemetry;

	public Sprite CustomLogo;

	public Sprite PlayerIcon;

	public Sprite FoundersIcon;

	public void Start()
	{
		SaveManager.SendTelemetry = false;
		SaveManager.SendName = false;
		if (!SentTelemetry && SaveManager.SendTelemetry)
		{
			SentTelemetry = true;
			DateTime utcNow = DateTime.UtcNow;
			if (SaveManager.LastStartDate != DateTime.MinValue && SaveManager.LastStartDate < utcNow)
			{
				TimeSpan timeSpan = utcNow - SaveManager.LastStartDate;
				Analytics.CustomEvent("GameOpened", new Dictionary<string, object>
				{
					{
						"TotalMinutes",
						timeSpan.TotalMinutes
					}
				});
			}
			SaveManager.LastStartDate = utcNow;
		}
		StartCoroutine(Announcement.Init());
		StartCoroutine(RunStartUp());
	}

	public void Quit()
    {
		StartCoroutine(CloseApp());
		IEnumerator CloseApp()
        {
			yield return this.DataPolicy.Show(true);
		}
    }

	private IEnumerator RunStartUp()
	{
		yield return Announcement.Show();
		yield return UnlockPop.Show();
		DateTime utcNow = DateTime.UtcNow;
		for (int i = 0; i < DestroyableSingleton<HatManager>.Instance.AllHats.Count; i++)
		{
			HatBehaviour hatBehaviour = DestroyableSingleton<HatManager>.Instance.AllHats[i];
			if (hatBehaviour.LimitedMonth == utcNow.Month && !SaveManager.GetPurchase(hatBehaviour.ProductId))
			{
				SaveManager.SetPurchased(hatBehaviour.ProductId);
			}
		}
	}

	private void Update()
	{
		if (CE_Input.CE_GetKeyUp(KeyCode.Escape))
		{
			CE_Input.EscapeFunctionality_MainMenu(this);
		}
	}

	static MainMenuManager()
	{
	}

	private void LoadLogo()
	{
		if (!CustomLogo)
        {
			Texture2D texture = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(Application.dataPath, "CE_Assets", "Textures", "logo.png"));
			CustomLogo = CE_TextureNSpriteExtensions.ConvertToSprite(texture, new Vector2(0.5f, 0.75f));
		}

		if (!FoundersIcon)
        {
			Texture2D texture2 = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(Application.dataPath, "CE_Assets", "Textures", "UI", "FoundersIcon.png"));
			FoundersIcon = CE_TextureNSpriteExtensions.ConvertToSprite(texture2, new Vector2(0.5f, 0.5f));
		}

		if (!PlayerIcon)
        {
			Texture2D texture3 = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(Application.dataPath, "CE_Assets", "Textures", "UI", "PlayerButton.png"));
			PlayerIcon = CE_TextureNSpriteExtensions.ConvertToSprite(texture3, new Vector2(0.5f, 0.5f));
		}
	}

	private void ModifyImages()
	{
		LoadLogo();
		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
		for (int i = 0; i < array.Length; i++)
		{
			SpriteRenderer component = array[i].GetComponent<SpriteRenderer>();
			if ((bool)component && component.name == "bannerLogo_AmongUs")
			{
				component.sprite = CustomLogo;
			}
			if ((bool)component && component.name == "StoreButton")
			{
				component.sprite = PlayerIcon;
			}
			/*
			if ((bool)component && component.name == "CreditsButton")
			{
				component.sprite = FoundersIcon;
			}
			*/
		}
	}

	private void LateUpdate()
	{
		ModifyImages();
	}
}
