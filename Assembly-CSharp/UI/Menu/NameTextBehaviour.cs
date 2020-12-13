using System;
using UnityEngine;

public class NameTextBehaviour : MonoBehaviour
{
	public static NameTextBehaviour Instance;

	public TextBox nameSource;

	public void Start()
	{
		Instance = this;
		nameSource.SetText(SaveManager.PlayerName);
		nameSource.OnFocusLost.AddListener(UpdateName);
	}

	public void UpdateName()
	{
		if (!ShakeIfInvalid())
		{
			SaveManager.PlayerName = nameSource.text;
		}
	}

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

	public bool ShakeIfInvalid()
	{
		if (!IsValidName(nameSource.text))
		{
			StartCoroutine(Effects.Shake(nameSource.transform));
			return true;
		}
		return false;
	}
}
