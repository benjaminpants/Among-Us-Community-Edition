using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LanguageUnit
{
	public bool IsEnglish;

	private Dictionary<StringNames, string> AllStrings = new Dictionary<StringNames, string>();

	public LanguageUnit(TextAsset data)
	{
		using StringReader stringReader = new StringReader(data.text);
		for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
		{
			int num = text.IndexOf(',');
			if (num >= 0 && Enum.TryParse<StringNames>(text.Substring(0, num), out var result))
			{
				string value = text.Substring(num + 1);
				AllStrings.Add(result, value);
			}
		}
	}

	public string GetString(StringNames stringId, params string[] parts)
	{
		if (AllStrings.TryGetValue(stringId, out var value))
		{
			if (parts.Length != 0)
			{
				return string.Format(value, parts);
			}
			return value;
		}
		return "Missing string";
	}
}
