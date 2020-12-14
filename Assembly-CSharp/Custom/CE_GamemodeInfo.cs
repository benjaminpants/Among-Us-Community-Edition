using MoonSharp.Interpreter;

public class CE_GamemodeInfo
{
	public string name;

	public Script script;

	public byte id;

	public CE_GamemodeInfo()
	{
		name = "null";
		script = new Script();
		id = 0;
	}

	public CE_GamemodeInfo(string nme, Script scr, byte id)
	{
		name = nme;
		script = scr;
		this.id = id;
	}
}
