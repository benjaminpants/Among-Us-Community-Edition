using System.Collections.Generic;
using Newtonsoft.Json;

public class CE_FrameSet
{
	[JsonIgnore]
	public string FilePath;
	public string ID
	{
		get;
		set;
	} = "NullSkin";
	public string Name
	{
		get;
		set;
	} = "CustomSkin";
	public string StoreName { get; set; }
	public bool HatInFront { get; set; }
	public bool NoHatBobbing { get; set; }
	public bool UsePointFilteringGlobally { get; set; }
	public bool UseColorFilteringGlobally { get; set; }
	public string RelatedSkin { get; set; }
	public string RelatedHat { get; set; }
	public bool UsePercentageBasedPivot { get; set; }
	public List<CE_SpriteFrame> FrameList
	{
		get;
		set;
	} = new List<CE_SpriteFrame>();

}
