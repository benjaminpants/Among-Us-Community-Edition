using System;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class PingTracker : MonoBehaviour
{
	// Token: 0x060007C2 RID: 1986 RVA: 0x0002C168 File Offset: 0x0002A368
	private void Update()
	{
		if (AmongUsClient.Instance)
		{
			if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
			{
				base.gameObject.SetActive(false);
			}
			this.text.Text = string.Format("Ping: {0} ms", AmongUsClient.Instance.Ping);
		}
	}

	// Token: 0x040007A2 RID: 1954
	public TextRenderer text;
}
