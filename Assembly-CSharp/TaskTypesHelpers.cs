using System;
using System.Linq;
using System.Text;

// Token: 0x0200021D RID: 541
public static class TaskTypesHelpers
{
	// Token: 0x06000BBE RID: 3006 RVA: 0x00039C28 File Offset: 0x00037E28
	static TaskTypesHelpers()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < TaskTypesHelpers.AllTypes.Length; i++)
		{
			stringBuilder.Length = 0;
			stringBuilder.Append(TaskTypesHelpers.AllTypes[i]);
			for (int j = 1; j < stringBuilder.Length; j++)
			{
				if (stringBuilder[j] >= 'A' && stringBuilder[j] <= 'Z')
				{
					stringBuilder.Insert(j, " ");
					j++;
				}
			}
			TaskTypesHelpers.StringNames[i] = stringBuilder.ToString();
		}
	}

	// Token: 0x04000B47 RID: 2887
	public static readonly TaskTypes[] AllTypes = Enum.GetValues(typeof(TaskTypes)).Cast<TaskTypes>().ToArray<TaskTypes>();

	// Token: 0x04000B48 RID: 2888
	public static string[] StringNames = new string[TaskTypesHelpers.AllTypes.Length];
}
