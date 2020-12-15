using System.Collections.Generic;
using UnityEngine;

public class CE_CustomSkinDefinition
{
	public string ID
	{
		get;
		set;
	} = "NullSkin";
	public string SkinSheet
	{
		get;
		set;
	} = string.Empty;
	public List<CE_SpriteFrame> FrameList
	{
		get;
		set;
	} = new List<CE_SpriteFrame>();

}
