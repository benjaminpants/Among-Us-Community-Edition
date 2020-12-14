using System.Collections.Generic;
using UnityEngine;

public class CE_CustomSkinDefinition
{
	public class CustomSkinFrame
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

		public string Name
		{
			get;
			set;
		} = string.Empty;


		public string SpritePath
		{
			get;
			set;
		}

		public Texture2D Texture
		{
			get;
			set;
		}

		public bool UsePointFiltering
		{
			get;
			set;
		}

		public CustomSkinFrame()
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

	public struct Point
	{
		public static Point Zero;

		public float x
		{
			get;
			set;
		}

		public float y
		{
			get;
			set;
		}

		public Point(int _x, int _y)
		{
			x = _x;
			y = _y;
		}

		public Point(float _x, float _y)
		{
			x = _x;
			y = _y;
		}

		static Point()
		{
			Zero = new Point(0, 0);
		}
	}

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


	public List<CustomSkinFrame> FrameList
	{
		get;
		set;
	} = new List<CustomSkinFrame>();

}
