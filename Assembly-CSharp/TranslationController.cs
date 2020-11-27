using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class TranslationController : DestroyableSingleton<TranslationController>
{
	// Token: 0x06000BD1 RID: 3025 RVA: 0x0003A270 File Offset: 0x00038470
	public override void Awake()
	{
		base.Awake();
		if (DestroyableSingleton<TranslationController>.Instance == this)
		{
			this.Translations = Resources.LoadAll<TextAsset>("Text");
			Debug.Log(string.Format("Loaded {0} languages: {1}", this.Translations.Length, string.Join(" ", from t in this.Translations
			select t.name)));
			this.CurrentLanguage = new LanguageUnit(this.Translations[0]);
		}
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x0000919C File Offset: 0x0000739C
	public string GetString(StringNames stringId)
	{
		return this.CurrentLanguage.GetString(stringId, new string[] { });
	}

	// Token: 0x04000B6A RID: 2922
	public TextAsset[] Translations;

	// Token: 0x04000B6B RID: 2923
	public LanguageUnit CurrentLanguage;
}
