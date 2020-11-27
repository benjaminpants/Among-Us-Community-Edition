using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000066 RID: 102
public class CrewVisualizer : MonoBehaviour
{
	// Token: 0x0600022B RID: 555 RVA: 0x000122D4 File Offset: 0x000104D4
	public void SetCrewSize(int numPlayers, int numImpostors)
	{
		this.CrewPool.ReclaimAll();
		int num = numPlayers / 2;
		int num2 = Mathf.CeilToInt((float)numPlayers / 2f);
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		Vector3 localPosition = new Vector3(0f, 0f, -1f);
		for (int i = 0; i < numPlayers; i++)
		{
			SpriteRenderer component = this.CrewPool.Get<PoolableBehavior>().GetComponent<SpriteRenderer>();
			component.color = Color.white;
			list.Add(component);
			if (i < num)
			{
				float num3 = Mathf.Clamp((float)num / 5f * 1.3f, 0f, 1f) * 0.85f;
				localPosition.z = -1.5f;
				localPosition.y = -this.yOffset;
				localPosition.x = this.BgWidth.Lerp((float)i / ((float)num - 1f)) * num3;
			}
			else
			{
				float num4 = Mathf.Clamp((float)num2 / 5f * 1.3f, 0f, 1f);
				localPosition.z = -1f;
				localPosition.y = this.yOffset;
				localPosition.x = this.BgWidth.Lerp((float)(i - num) / ((float)num2 - 1f)) * num4;
			}
			component.transform.localPosition = localPosition;
		}
		int j = 0;
		int num5 = 0;
		while (j < numImpostors)
		{
			if (BoolRange.Next(1f / (float)list.Count))
			{
				j++;
				list[num5].color = Color.red;
				list.RemoveAt(num5);
			}
			num5 = (num5 + 1) % list.Count;
		}
	}

	// Token: 0x0600022C RID: 556 RVA: 0x000035DF File Offset: 0x000017DF
	public void SetMap(int mapid)
	{
		this.Background.sprite = this.MapBackgrounds[mapid];
	}

	// Token: 0x0400020C RID: 524
	public ObjectPoolBehavior CrewPool;

	// Token: 0x0400020D RID: 525
	public SpriteRenderer Background;

	// Token: 0x0400020E RID: 526
	public Sprite[] MapBackgrounds;

	// Token: 0x0400020F RID: 527
	public float yOffset = 0.4f;

	// Token: 0x04000210 RID: 528
	public FloatRange BgWidth;
}
