using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class CE_ConversionHelpers
{

    public static bool FloatToBool(float f)
    {
        return f == 1f;
    }

    public static float BoolToFloat(bool b)
    {
        return (b ? 1f : 0f);
    }
}

public static class CE_PrefabHelpers
{
	public static UnityEngine.Object FindPrefab(string name,Type type)
	{
		var resources = UnityEngine.Resources.FindObjectsOfTypeAll(type);
		if (resources != null)
		{
			foreach (var item in resources)
			{
				if (item.name == name)
				{
					return item; 
				}
			}
		}
		return null;
	}
}
