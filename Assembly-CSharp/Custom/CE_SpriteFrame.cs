using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CE_SpriteFrame
{
	public Point Position
	{
		get;
		set;
	}
	public Point Size
	{
		get;
		set;
	}
	public Point Offset
	{
		get;
		set;
	}
	public string Name { get; set; } = string.Empty;
	public string SpritePath { get; set; }
	[Newtonsoft.Json.JsonIgnore]
	public Texture2D Texture { get; set; }
	[Newtonsoft.Json.JsonIgnore]
	public Sprite Sprite { get; set; }
	public bool UsePointFiltering { get; set; }
	public CE_SpriteFrame()
	{
		Position = Point.Zero;
		Size = Point.Zero;
		Offset = Point.Zero;
		UsePointFiltering = false;
	}
	public override string ToString()
	{
		return Name;
	}
}


