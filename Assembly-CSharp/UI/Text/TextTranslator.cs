using UnityEngine;

[RequireComponent(typeof(TextRenderer))]
public class TextTranslator : MonoBehaviour
{
	public StringNames TargetText;

	public void Start()
	{
		GetComponent<TextRenderer>().Text = DestroyableSingleton<TranslationController>.Instance.GetString(TargetText);
	}
}
