using UnityEngine;
using System.Collections.Generic;

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

	public static readonly Color32 VisorColor;

	public static readonly Color32 ImpostorOnlyRed;

	public static List<CE_PlayerColor> PLColors = new List<CE_PlayerColor>
	{
		new CE_PlayerColor(new Color32(198, 17, 17, byte.MaxValue),new Color32(122, 8, 56, byte.MaxValue),"Red"),
		new CE_PlayerColor(new Color32(19, 46, 210, byte.MaxValue),new Color32(9, 21, 142, byte.MaxValue),"Blue"),
		new CE_PlayerColor(new Color32(17, 128, 45, byte.MaxValue),new Color32(10, 77, 46, byte.MaxValue),"Green"),
		new CE_PlayerColor(new Color32(238, 84, 187, byte.MaxValue),new Color32(172, 43, 174, byte.MaxValue),"Pink"),
		new CE_PlayerColor(new Color32(240, 125, 13, byte.MaxValue),new Color32(180, 62, 21, byte.MaxValue),"Orange"),
		new CE_PlayerColor(new Color32(246, 246, 87, byte.MaxValue),new Color32(195, 136, 34, byte.MaxValue),"Yellow"),
		new CE_PlayerColor(new Color32(63, 71, 78, byte.MaxValue),new Color32(30, 31, 38, byte.MaxValue),"Black"),
		new CE_PlayerColor(new Color32(215, 225, 241, byte.MaxValue),new Color32(132, 149, 192, byte.MaxValue),"White"),
		new CE_PlayerColor(new Color32(107, 47, 188, byte.MaxValue),new Color32(59, 23, 124, byte.MaxValue),"Purple"),
		new CE_PlayerColor(new Color32(113, 73, 30, byte.MaxValue),new Color32(94, 38, 21, byte.MaxValue),"Brown"),
		new CE_PlayerColor(new Color32(56, byte.MaxValue, 221, byte.MaxValue),new Color32(36, 169, 191, byte.MaxValue),"Cyan"),
		new CE_PlayerColor(new Color32(80, 240, 57, byte.MaxValue),new Color32(21, 168, 66, byte.MaxValue),"Lime"),
		new CE_PlayerColor(new Color32(236, 117, 120, byte.MaxValue),new Color32(180, 67, 98, byte.MaxValue),"Coral"),
		new CE_PlayerColor(new Color32(236, 192, 211, byte.MaxValue),new Color32(211, 146, 179, byte.MaxValue),"Rose"),
		new CE_PlayerColor(new Color32(112, 132, 151, byte.MaxValue),new Color32(68, 83, 102, byte.MaxValue),"Gray"),
		new CE_PlayerColor(new Color32(146, 135, 118, byte.MaxValue),new Color32(82, 67, 63, byte.MaxValue),"Tan"),
		new CE_PlayerColor(new Color32(115, 27, 19, byte.MaxValue),new Color32(91, 19, 27, byte.MaxValue),"Maroon"),
		new CE_PlayerColor(new Color32(255, 253, 190, byte.MaxValue),new Color32(209, 188, 137, byte.MaxValue),"Banana")

	};

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
		ImpostorOnlyRed = new Color32(206, 121, 139, byte.MaxValue);
                VisorColor = new Color32(149, 202, 220, byte.MaxValue);
	}
}
