using UnityEngine;

public static class Palette
{
	public static readonly Color DisabledGrey;

	public static readonly Color DisabledColor;

	public static readonly Color EnabledColor;

	public static readonly Color Black;

	public static readonly Color ClearWhite;

	public static readonly Color HalfWhite;

	public static readonly Color White;

	public static readonly Color LightBlue;

	public static readonly Color Blue;

	public static readonly Color Orange;

	public static readonly Color Purple;

	public static readonly Color Brown;

	public static readonly Color CrewmateBlue;

	public static readonly Color ImpostorRed;

	public static readonly Color32[] PlayerColors;

	public static readonly Color32[] ShadowColors;

	public static readonly Color32 VisorColor;

	public static readonly Color32 VisorColorRed;

	public static readonly Color32 VisorColorGreen;

	public static readonly Color InfectedGreen;

	public static readonly Color SheriffYellow;

	public static readonly Color32 VisorColorCarJemGenerations;

	static Palette()
	{
		DisabledGrey = new Color(0.3f, 0.3f, 0.3f, 1f);
		DisabledColor = new Color(1f, 1f, 1f, 0.3f);
		EnabledColor = new Color(1f, 1f, 1f, 1f);
		Black = new Color(0f, 0f, 0f, 1f);
		ClearWhite = new Color(1f, 1f, 1f, 0f);
		HalfWhite = new Color(1f, 1f, 1f, 0.5f);
		White = new Color(1f, 1f, 1f, 1f);
		LightBlue = new Color(0.5f, 0.5f, 1f);
		Blue = new Color(0.2f, 0.2f, 1f);
		Orange = new Color(1f, 0.6f, 0.005f);
		Purple = new Color(0.6f, 0.1f, 0.6f);
		Brown = new Color(0.72f, 0.43f, 0.11f);
		CrewmateBlue = new Color32(140, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		ImpostorRed = new Color32(byte.MaxValue, 25, 25, byte.MaxValue);
		SheriffYellow = new Color32(byte.MaxValue, 216, 0, byte.MaxValue);
		InfectedGreen = new Color32(98, 167, 74, byte.MaxValue);
		PlayerColors = new Color32[24]
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
			new Color32(byte.MaxValue, 0, 0, byte.MaxValue),
			new Color32(122, 7, 7, byte.MaxValue)
		};
		ShadowColors = new Color32[24]
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
			new Color32(0, 0, byte.MaxValue, byte.MaxValue),
			new Color32(byte.MaxValue, 0, 42, byte.MaxValue)
		};
		VisorColor = new Color32(149, 202, 220, byte.MaxValue);
		VisorColorRed = new Color32(192, 15, 11, byte.MaxValue);
		VisorColorGreen = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
		VisorColorCarJemGenerations = new Color32(220, 220, 220, byte.MaxValue);
	}
}