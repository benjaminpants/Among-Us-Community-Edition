using System.Collections.Generic;
using UnityEngine;

public class CE_CustomSkinDefinition
{
	public string FilePath;
	public string ID
	{
		get;
		set;
	} = "NullSkin";
	public List<CE_SpriteFrame> FrameList
	{
		get;
		set;
	} = new List<CE_SpriteFrame>();

}
