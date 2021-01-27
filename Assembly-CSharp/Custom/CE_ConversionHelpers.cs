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

