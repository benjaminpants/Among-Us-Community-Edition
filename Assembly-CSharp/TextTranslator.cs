using System;
using UnityEngine;

// Token: 0x02000226 RID: 550
[RequireComponent(typeof(TextRenderer))]
public class TextTranslator : MonoBehaviour
{
	// Token: 0x06000BCF RID: 3023 RVA: 0x0000917F File Offset: 0x0000737F
	public void Start()
	{
		base.GetComponent<TextRenderer>().Text = DestroyableSingleton<TranslationController>.Instance.GetString(this.TargetText);
	}

	// Token: 0x04000B69 RID: 2921
	public StringNames TargetText;
}
