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

	private IEnumerator RunStartUp()
	{
		yield return DataPolicy.Show();
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
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	static MainMenuManager()
	{
	}

	private void LoadLogo()
	{
		Texture2D texture = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(Application.dataPath, "CE_Assets", "Textures", "logo.png"));
		CustomLogo = CE_TextureNSpriteExtensions.ConvertToSprite(texture, new Vector2(0.5f, 0.65f));
	}

	private void ModifyLogo()
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
		for (int i = 0; i < array.Length; i++)
		{
			SpriteRenderer component = array[i].GetComponent<SpriteRenderer>();
			if ((bool)component && component.name == "bannerLogo_AmongUs")
			{
				if (!CustomLogo)
				{
					LoadLogo();
					break;
				}
				component.sprite = CustomLogo;
			}
		}
	}

	private void LateUpdate()
	{
		ModifyLogo();
	}
}
