using System;
using System.Linq;

public static class SystemTypeHelpers
{
	public static readonly SystemTypes[] AllTypes;

	public const int SecondsLeftField = 0;

	public static string[] StringNames;


	public static string GetName(SystemTypes syst)
    {
		CE_MapInfo mfo = GetCurrentMap();
		if ((int)syst > 24)
        {
			return mfo.CustomLocationNames[(int)syst - 25];
        }
		return StringNames[(int)syst];
    }

    private static CE_MapInfo GetCurrentMap()
    {
        throw new NotImplementedException();
    }

    static SystemTypeHelpers()
	{
		AllTypes = Enum.GetValues(typeof(SystemTypes)).Cast<SystemTypes>().ToArray();
		StringNames = new string[AllTypes.Length];
		for (int i = 0; i < AllTypes.Length; i++)
		{
			switch (AllTypes[i])
			{
			case SystemTypes.Nav:
				StringNames[i] = "Navigations";
				break;
			case SystemTypes.LifeSupp:
				StringNames[i] = "O2";
				break;
			case SystemTypes.UpperEngine:
				StringNames[i] = "Upper Engine";
				break;
			case SystemTypes.LowerEngine:
				StringNames[i] = "Lower Engine";
				break;
			case SystemTypes.LockerRoom:
				StringNames[i] = "Locker Room";
				break;
			default:
				StringNames[i] = AllTypes[i].ToString();
				break;
			}
		}
	}
}
