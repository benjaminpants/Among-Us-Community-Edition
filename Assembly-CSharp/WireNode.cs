using System;
using UnityEngine;

// Token: 0x02000085 RID: 133
public class WireNode : MonoBehaviour
{
	// Token: 0x060002D3 RID: 723 RVA: 0x00015BCC File Offset: 0x00013DCC
	internal void SetColor(Color color)
	{
		for (int i = 0; i < this.WireColors.Length; i++)
		{
			this.WireColors[i].color = color;
		}
	}

	// Token: 0x040002CF RID: 719
	public Collider2D hitbox;

	// Token: 0x040002D0 RID: 720
	public SpriteRenderer[] WireColors;

	// Token: 0x040002D1 RID: 721
	public sbyte WireId;
}
