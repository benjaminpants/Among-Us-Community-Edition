using System;
using UnityEngine;

// Token: 0x02000079 RID: 121
public class DotAligner : MonoBehaviour
{
	// Token: 0x0600029A RID: 666 RVA: 0x00014788 File Offset: 0x00012988
	public void Start()
	{
		int num = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i).gameObject.activeSelf)
			{
				num++;
			}
		}
		float num2;
		float num3;
		if (this.Even)
		{
			num2 = -this.Width * (float)(num - 1) / 2f;
			num3 = this.Width;
		}
		else
		{
			num2 = -this.Width / 2f;
			num3 = this.Width / (float)(num - 1);
		}
		int num4 = 0;
		for (int j = 0; j < base.transform.childCount; j++)
		{
			Transform child = base.transform.GetChild(j);
			if (child.gameObject.activeSelf)
			{
				child.transform.localPosition = new Vector3(num2 + (float)num4 * num3, 0f, 0f);
				num4++;
			}
		}
	}

	// Token: 0x04000282 RID: 642
	public float Width = 2f;

	// Token: 0x04000283 RID: 643
	public bool Even;
}
