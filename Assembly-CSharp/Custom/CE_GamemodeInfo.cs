using MoonSharp.Interpreter;

public class CE_GamemodeInfo
{
    public string name;

    public string internalname;

    public Script script;

    public byte id;

    public CE_GamemodeInfo()
    {
        name = "null";
        script = new Script();
        id = 0;
    }

    public CE_GamemodeInfo(string nme, Script scr, byte id, string intr)
    {
        name = nme;
        script = scr;
        this.id = id;
        internalname = intr;
    }
}

public class CE_PluginInfo
{
    public string name;

    public Script script;

    public byte id;

    public bool highprior;

    public CE_PluginInfo()
    {
        name = "null";
        script = new Script();
        highprior = false;
        id = 0;
    }

    public CE_PluginInfo(string nme, Script scr, byte id)
    {
        name = nme;
        script = scr;
        this.id = id;
        highprior = false;
    }

    public CE_PluginInfo(string nme, Script scr, byte id, bool high)
    {
        name = nme;
        script = scr;
        this.id = id;
        highprior = high;
    }
}
