public class CE_OldCustomHatDefinition
{
	public string ID { get; set; } = "NullHat";
	public string NormalImg { get; set; } = string.Empty;
	public string FloorImg { get; set; } = string.Empty;
	public bool inFront { get; set; }
	public bool NoBobbing { get; set; }
	public float NormalPosX { get; set; }
	public float NormalPosY { get; set; }
	public float NormalWidth { get; set; }
	public float NormalHeight { get; set; }
	public float NormalPivotX { get; set; }
	public float NormalPivotY { get; set; }
	public float FloorPosX { get; set; }
	public float FloorPosY { get; set; }
	public float FloorWidth { get; set; }
	public float FloorHeight { get; set; }
	public float FloorPivotX { get; set; }
	public float FloorPivotY { get; set; }
	public bool UsePointFiltering { get; set; }
	public string PreviewImg { get; set; }
	public float PreviewPosX { get; set; }
	public float PreviewPosY { get; set; }
	public float PreviewWidth { get; set; }
	public float PreviewHeight { get; set; }
	public float PreviewPivotX { get; set; }
	public float PreviewPivotY { get; set; }
}
