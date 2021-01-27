using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CE_CustomLuaSetting
{
    public CE_OptDataTypes DataType { get; private set; }
    public float Min { get; private set; }
    public float Max { get; private set; }
    public float Increment { get; private set; }
    public float NumDefault { get; private set; }
    public string StringDefault { get; private set; }
    public string Name { get; private set; }
    public bool IsNumber { get; private set; }
    public float NumValue;
    public string StringValue;
    public CE_CustomLuaSetting()
    {
        StringDefault = "Undefined";
        Min = 0;
        Max = 100;
        NumDefault = 50;
        DataType = CE_OptDataTypes.None;
        IsNumber = true;
        Name = "Undefined";
        StringValue = StringDefault;
        NumValue = NumDefault;
    }

    public CE_CustomLuaSetting(CE_OptDataTypes type, string name)
    {
        StringDefault = "Default";
        Name = name;
        DataType = type;
        IsNumber = true;
        StringValue = StringDefault;
        NumValue = NumDefault;
    }

    public CE_CustomLuaSetting(byte min, byte max, byte def, string name, byte increment = 1)
    {
        Min = min;
        Max = max;
        NumDefault = def;
        Increment = increment;
        Name = name;
        DataType = CE_OptDataTypes.ByteRange;
        IsNumber = true;
        StringValue = StringDefault;
        NumValue = NumDefault;
    }

    public CE_CustomLuaSetting(float min, float max, float def, string name, float increment = 1)
    {
        Min = min;
        Max = max;
        NumDefault = def;
        Increment = increment;
        Name = name;
        DataType = CE_OptDataTypes.FloatRange;
        IsNumber = true;
        StringValue = StringDefault;
        NumValue = NumDefault;
    }

    public CE_CustomLuaSetting(string def, string name)
    {
        StringDefault = def;
        Name = name;
        DataType = CE_OptDataTypes.String;
        IsNumber = false;
        StringValue = StringDefault;
        NumValue = NumDefault;
    }

    public CE_CustomLuaSetting(int min, int max, int def, string name, int increment = 1)
    {
        Min = min;
        Max = max;
        NumDefault = def;
        Name = name;
        Increment = increment;
        DataType = CE_OptDataTypes.IntRange;
        IsNumber = true;
        StringValue = StringDefault;
        NumValue = NumDefault;
    }

    public CE_CustomLuaSetting(string name, bool def)
    {
        Min = 0;
        Max = 1;
        NumDefault = CE_ConversionHelpers.BoolToFloat(def);
        Name = name;
        Increment = 1;
        DataType = CE_OptDataTypes.Toggle;
        IsNumber = true;
        StringValue = StringDefault;
        NumValue = NumDefault;
    }


}


public class CE_InterpretedSetting
{
    public CE_OptDataTypes DataType;
    public float NumValue;
    public string StringValue;
}

public enum CE_OptDataTypes : byte
{
    None,
    Toggle,
    String,
    FloatRange,
    IntRange,
    BoolArray,
    FloatArray,
    IntArray,
    StringArray,
    ByteRange,
    ByteArray
}

