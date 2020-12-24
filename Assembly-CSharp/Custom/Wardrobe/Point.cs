using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
