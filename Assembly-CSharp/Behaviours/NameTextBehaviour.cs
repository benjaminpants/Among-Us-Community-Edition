using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000F3 RID: 243
public class NameTextBehaviour : MonoBehaviour
{
	// Token: 0x06000533 RID: 1331 RVA: 0x00005335 File Offset: 0x00003535
	public void Start()
	{
		NameTextBehaviour.Instance = this;
		this.nameSource.SetText(SaveManager.PlayerName, "");
		this.nameSource.OnFocusLost.AddListener(new UnityAction(this.UpdateName));
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0000536E File Offset: 0x0000356E
	public void UpdateName()
	{
		if (this.ShakeIfInvalid())
		{
			return;
		}
		SaveManager.PlayerName = this.nameSource.text;
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x000223E4 File Offset: 0x000205E4
	public static bool IsValidName(string text)
	{
		if (text == null || text.Length == 0)
		{
			return false;
		}
		if (text.Equals("Enter Name", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		if (BlockedWords.ContainsWord(text))
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] != ' ')
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x00005389 File Offset: 0x00003589
	public bool ShakeIfInvalid()
	{
		if (!NameTextBehaviour.IsValidName(this.nameSource.text))
		{
			base.StartCoroutine(Effects.Shake(this.nameSource.transform, 0.75f, 0.25f));
			return true;
		}
		return false;
	}

	// Token: 0x040004FF RID: 1279
	public static NameTextBehaviour Instance;

	// Token: 0x04000500 RID: 1280
	public TextBox nameSource;
}
