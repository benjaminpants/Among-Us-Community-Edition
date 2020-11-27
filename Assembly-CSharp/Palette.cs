using System;
using UnityEngine;

// Token: 0x02000043 RID: 67
public static class Palette
{
	// Token: 0x0600017F RID: 383 RVA: 0x0000F598 File Offset: 0x0000D798
	static Palette()
	{
		Palette.InfectedGreen = new Color32(98, 167, 74, byte.MaxValue);
		Palette.PlayerColors = new Color32[]
		{
			new Color32(198, 17, 17, byte.MaxValue),
			new Color32(19, 46, 210, byte.MaxValue),
			new Color32(17, 128, 45, byte.MaxValue),
			new Color32(238, 84, 187, byte.MaxValue),
			new Color32(240, 125, 13, byte.MaxValue),
			new Color32(246, 246, 87, byte.MaxValue),
			new Color32(63, 71, 78, byte.MaxValue),
			new Color32(215, 225, 241, byte.MaxValue),
			new Color32(107, 47, 188, byte.MaxValue),
			new Color32(113, 73, 30, byte.MaxValue),
			new Color32(56, byte.MaxValue, 221, byte.MaxValue),
			new Color32(80, 240, 57, byte.MaxValue),
			new Color32(byte.MaxValue, 187, 142, byte.MaxValue),
			new Color32(byte.MaxValue, 127, 127, byte.MaxValue),
			new Color32(114, 137, 218, byte.MaxValue),
			new Color32(165, byte.MaxValue, 127, byte.MaxValue),
			new Color32(127, 106, 0, byte.MaxValue),
			new Color32(104, 0, 0, byte.MaxValue),
			new Color32(0, 15, 102, byte.MaxValue),
			new Color32(161, 127, byte.MaxValue, byte.MaxValue),
			new Color32(byte.MaxValue, 233, 127, byte.MaxValue),
			new Color32(85, 102, 101, byte.MaxValue),
			new Color32(byte.MaxValue, 0, 0, byte.MaxValue)
		};
		Palette.ShadowColors = new Color32[]
		{
			new Color32(122, 8, 56, byte.MaxValue),
			new Color32(9, 21, 142, byte.MaxValue),
			new Color32(10, 77, 46, byte.MaxValue),
			new Color32(172, 43, 174, byte.MaxValue),
			new Color32(180, 62, 21, byte.MaxValue),
			new Color32(195, 136, 34, byte.MaxValue),
			new Color32(30, 31, 38, byte.MaxValue),
			new Color32(132, 149, 192, byte.MaxValue),
			new Color32(59, 23, 124, byte.MaxValue),
			new Color32(94, 38, 21, byte.MaxValue),
			new Color32(36, 169, 191, byte.MaxValue),
			new Color32(21, 168, 66, byte.MaxValue),
			new Color32(224, 152, 65, byte.MaxValue),
			new Color32(124, 39, 39, byte.MaxValue),
			new Color32(78, 89, 148, byte.MaxValue),
			new Color32(42, 124, 52, byte.MaxValue),
			new Color32(99, 72, 0, byte.MaxValue),
			new Color32(73, 7, 0, byte.MaxValue),
			new Color32(0, 9, 63, byte.MaxValue),
			new Color32(72, 54, 124, byte.MaxValue),
			new Color32(168, 149, 67, byte.MaxValue),
			new Color32(51, 50, 69, byte.MaxValue),
			new Color32(0, 0, byte.MaxValue, byte.MaxValue)
		};
		Palette.VisorColor = new Color32(149, 202, 220, byte.MaxValue);
		Palette.VisorColorRed = new Color32(192, 15, 11, byte.MaxValue);
		Palette.VisorColorGreen = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
	}

	// Token: 0x04000174 RID: 372
	public static readonly Color DisabledGrey = new Color(0.3f, 0.3f, 0.3f, 1f);

	// Token: 0x04000175 RID: 373
	public static readonly Color DisabledColor = new Color(1f, 1f, 1f, 0.3f);

	// Token: 0x04000176 RID: 374
	public static readonly Color EnabledColor = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04000177 RID: 375
	public static readonly Color Black = new Color(0f, 0f, 0f, 1f);

	// Token: 0x04000178 RID: 376
	public static readonly Color ClearWhite = new Color(1f, 1f, 1f, 0f);

	// Token: 0x04000179 RID: 377
	public static readonly Color HalfWhite = new Color(1f, 1f, 1f, 0.5f);

	// Token: 0x0400017A RID: 378
	public static readonly Color White = new Color(1f, 1f, 1f, 1f);

	// Token: 0x0400017B RID: 379
	public static readonly Color LightBlue = new Color(0.5f, 0.5f, 1f);

	// Token: 0x0400017C RID: 380
	public static readonly Color Blue = new Color(0.2f, 0.2f, 1f);

	// Token: 0x0400017D RID: 381
	public static readonly Color Orange = new Color(1f, 0.6f, 0.005f);

	// Token: 0x0400017E RID: 382
	public static readonly Color Purple = new Color(0.6f, 0.1f, 0.6f);

	// Token: 0x0400017F RID: 383
	public static readonly Color Brown = new Color(0.72f, 0.43f, 0.11f);

	// Token: 0x04000180 RID: 384
	public static readonly Color CrewmateBlue = new Color32(140, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04000181 RID: 385
	public static readonly Color ImpostorRed = new Color32(byte.MaxValue, 25, 25, byte.MaxValue);

	// Token: 0x04000182 RID: 386
	public static readonly Color32[] PlayerColors;

	// Token: 0x04000183 RID: 387
	public static readonly Color32[] ShadowColors;

	// Token: 0x04000184 RID: 388
	public static readonly Color32 VisorColor;

	// Token: 0x04000185 RID: 389
	public static readonly Color32 VisorColorRed;

	// Token: 0x04000186 RID: 390
	public static readonly Color32 VisorColorGreen;

	// Token: 0x04000187 RID: 391
	public static readonly Color InfectedGreen;

	// Token: 0x04000188 RID: 392
	public static readonly Color SheriffYellow = new Color32(byte.MaxValue, 216, 0, byte.MaxValue);
}
