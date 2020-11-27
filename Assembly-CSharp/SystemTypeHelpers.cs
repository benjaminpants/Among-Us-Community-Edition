using System;
using System.Linq;

// Token: 0x020001F5 RID: 501
public static class SystemTypeHelpers
{
	// Token: 0x06000ACB RID: 2763 RVA: 0x00036A68 File Offset: 0x00034C68
	static SystemTypeHelpers()
	{
		int i = 0;
		while (i < SystemTypeHelpers.AllTypes.Length)
		{
			SystemTypes systemTypes = SystemTypeHelpers.AllTypes[i];
			switch (systemTypes)
			{
			case SystemTypes.UpperEngine:
				SystemTypeHelpers.StringNames[i] = "Upper Engine";
				break;
			case SystemTypes.Nav:
				SystemTypeHelpers.StringNames[i] = "Navigations";
				break;
			case SystemTypes.Admin:
			case SystemTypes.Electrical:
				goto IL_AC;
			case SystemTypes.LifeSupp:
				SystemTypeHelpers.StringNames[i] = "O2";
				break;
			default:
				if (systemTypes != SystemTypes.LowerEngine)
				{
					if (systemTypes != SystemTypes.LockerRoom)
					{
						goto IL_AC;
					}
					SystemTypeHelpers.StringNames[i] = "Locker Room";
				}
				else
				{
					SystemTypeHelpers.StringNames[i] = "Lower Engine";
				}
				break;
			}
			IL_C9:
			i++;
			continue;
			IL_AC:
			SystemTypeHelpers.StringNames[i] = SystemTypeHelpers.AllTypes[i].ToString();
			goto IL_C9;
		}
	}

	// Token: 0x04000A7C RID: 2684
	public static readonly SystemTypes[] AllTypes = Enum.GetValues(typeof(SystemTypes)).Cast<SystemTypes>().ToArray<SystemTypes>();

	// Token: 0x04000A7D RID: 2685
	public const int SecondsLeftField = 0;

	// Token: 0x04000A7E RID: 2686
	public static string[] StringNames = new string[SystemTypeHelpers.AllTypes.Length];
}
