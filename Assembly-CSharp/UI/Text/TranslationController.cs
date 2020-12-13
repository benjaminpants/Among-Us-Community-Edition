using System.Linq;
using UnityEngine;

public class TranslationController : DestroyableSingleton<TranslationController>
{
	public TextAsset[] Translations;

	public LanguageUnit CurrentLanguage;

	public override void Awake()
	{
		base.Awake();
		if (DestroyableSingleton<TranslationController>.Instance == this)
		{
			Translations = Resources.LoadAll<TextAsset>("Text");
			Debug.Log(string.Format("Loaded {0} languages: {1}", Translations.Length, string.Join(" ", Translations.Select((TextAsset t) => t.name))));
			CurrentLanguage = new LanguageUnit(Translations[0]);
		}
	}

	public string GetString(StringNames stringId)
	{
		return CurrentLanguage.GetString(stringId);
	}
}
