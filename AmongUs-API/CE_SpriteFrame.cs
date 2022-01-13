using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class CE_SpriteFrame
{
	public CE_Point Position { get; set; }
	public CE_Point Size { get; set; }
	public CE_Point Offset { get; set; }
	public string Name { get; set; } = string.Empty;
	public string SpritePath { get; set; }
	public bool Hat_NoBobbing { get; set; }
	public bool InFront { get; set; }
	public bool UsePointFiltering { get; set; }
	public bool UseColorFiltering { get; set; }
// public bool PlayerOveride { get; set; }
	public CE_SpriteFrame()
	{
		Position = CE_Point.Zero;
		Size = CE_Point.Zero;
		Offset = CE_Point.Zero;
		UsePointFiltering = false;
	}
	public override string ToString()
	{
		return Name;
		// breh
	}
}


