using System;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class ConditionalHide : MonoBehaviour
{
	// Token: 0x06000073 RID: 115 RVA: 0x0000CCDC File Offset: 0x0000AEDC
	private void Awake()
	{
		for (int i = 0; i < this.HideForPlatforms.Length; i++)
		{
			if (this.HideForPlatforms[i] == RuntimePlatform.WindowsPlayer)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0400009E RID: 158
	public RuntimePlatform[] HideForPlatforms = new RuntimePlatform[]
	{
		RuntimePlatform.WindowsPlayer
	};
}
