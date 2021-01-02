using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct CE_Point
{
	public static CE_Point Zero;

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

	public CE_Point(int _x, int _y)
	{
		x = _x;
		y = _y;
	}

	public CE_Point(float _x, float _y)
	{
		x = _x;
		y = _y;
	}

	static CE_Point()
	{
		Zero = new CE_Point(0, 0);
	}
}
