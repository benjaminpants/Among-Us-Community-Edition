using System;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class VersionShower : MonoBehaviour
{
	// Token: 0x06000208 RID: 520 RVA: 0x0000348A File Offset: 0x0000168A
	public void Start()
	{
		this.text.Text = string.Concat(new object[]
		{
			"v",
			Application.version,
			".",
			0
		});
		Screen.sleepTimeout = -1;
	}

	// Token: 0x040001F8 RID: 504
	public TextRenderer text;
}
