using System.Collections.Generic;
using UnityEngine;

public class CrewVisualizer : MonoBehaviour
{
	public ObjectPoolBehavior CrewPool;

	public SpriteRenderer Background;

	public Sprite[] MapBackgrounds;

	public float yOffset = 0.4f;

	public FloatRange BgWidth;

	public void SetCrewSize(int numPlayers, int numImpostors)
	{
		CrewPool.ReclaimAll();
		int num = numPlayers / 2;
		int num2 = Mathf.CeilToInt((float)numPlayers / 2f);
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		Vector3 localPosition = new Vector3(0f, 0f, -1f);
		for (int i = 0; i < numPlayers; i++)
		{
			SpriteRenderer component = CrewPool.Get<PoolableBehavior>().GetComponent<SpriteRenderer>();
			component.color = Color.white;
			list.Add(component);
			if (i < num)
			{
				float num3 = Mathf.Clamp((float)num / 5f * 1.3f, 0f, 1f) * 0.85f;
				localPosition.z = -1.5f;
				localPosition.y = 0f - yOffset;
				localPosition.x = BgWidth.Lerp((float)i / ((float)num - 1f)) * num3;
			}
			else
			{
				float num4 = Mathf.Clamp((float)num2 / 5f * 1.3f, 0f, 1f);
				localPosition.z = -1f;
				localPosition.y = yOffset;
				localPosition.x = BgWidth.Lerp((float)(i - num) / ((float)num2 - 1f)) * num4;
			}
			component.transform.localPosition = localPosition;
		}
		int num5 = 0;
		int num6 = 0;
		while (num5 < numImpostors)
		{
			if (BoolRange.Next(1f / (float)list.Count))
			{
				num5++;
				list[num6].color = Color.red;
				list.RemoveAt(num6);
			}
			num6 = (num6 + 1) % list.Count;
		}
	}

	public void SetMap(int mapid)
	{
		Background.sprite = MapBackgrounds[mapid];
	}
}
