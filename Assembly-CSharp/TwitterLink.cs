using System;
using UnityEngine;

// Token: 0x0200005E RID: 94
public class TwitterLink : MonoBehaviour
{
	// Token: 0x06000206 RID: 518 RVA: 0x0000346A File Offset: 0x0000166A
	public void Click()
	{
		Application.OpenURL(this.LinkUrl);
	}

	// Token: 0x040001F7 RID: 503
	public string LinkUrl = "https://www.twitter.com/InnerslothDevs";
}
