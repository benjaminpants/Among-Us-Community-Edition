using System;
using UnityEngine;

// Token: 0x02000097 RID: 151
public class GameSettingMenu : MonoBehaviour
{
	// Token: 0x0600032D RID: 813 RVA: 0x0001712C File Offset: 0x0001532C
	private void OnEnable()
	{
		int num = 0;
		for (int i = 0; i < this.AllItems.Length; i++)
		{
			Transform transform = this.AllItems[i];
			if (transform.gameObject.activeSelf)
			{
				if (AmongUsClient.Instance.GameMode == GameModes.OnlineGame && this.HideForOnline.IndexOf(transform) != -1)
				{
					transform.gameObject.SetActive(false);
				}
				else
				{
					Vector3 localPosition = transform.localPosition;
					localPosition.y = this.YStart - (float)num * this.YOffset;
					transform.localPosition = localPosition;
					num++;
				}
			}
		}
		base.GetComponent<Scroller>().YBounds.max = (float)num * this.YOffset / 2f + 0.1f;
	}

	// Token: 0x04000323 RID: 803
	public Transform[] AllItems;

	// Token: 0x04000324 RID: 804
	public float YStart;

	// Token: 0x04000325 RID: 805
	public float YOffset;

	// Token: 0x04000326 RID: 806
	public Transform[] HideForOnline;
}
