using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class LanguageUnit
{
	// Token: 0x06000BCD RID: 3021 RVA: 0x0003A1AC File Offset: 0x000383AC
	public LanguageUnit(TextAsset data)
	{
		using (StringReader stringReader = new StringReader(data.text))
		{
			for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
			{
				int num = text.IndexOf(',');
				StringNames key;
				if (num >= 0 && Enum.TryParse<StringNames>(text.Substring(0, num), out key))
				{
					string value = text.Substring(num + 1);
					this.AllStrings.Add(key, value);
				}
			}
		}
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x0003A23C File Offset: 0x0003843C
	public string GetString(StringNames stringId, params string[] parts)
	{
		string text;
		if (!this.AllStrings.TryGetValue(stringId, out text))
		{
			return "Missing string";
		}
		if (parts.Length != 0)
		{
			return string.Format(text, parts);
		}
		return text;
	}

	// Token: 0x04000B61 RID: 2913
	public bool IsEnglish;

	// Token: 0x04000B62 RID: 2914
	private Dictionary<StringNames, string> AllStrings = new Dictionary<StringNames, string>();
}
