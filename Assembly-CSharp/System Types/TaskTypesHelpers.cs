using System;
using System.Linq;
using System.Text;

public static class TaskTypesHelpers
{
	public static readonly TaskTypes[] AllTypes;

	public static string[] StringNames;

	static TaskTypesHelpers()
	{
		AllTypes = Enum.GetValues(typeof(TaskTypes)).Cast<TaskTypes>().ToArray();
		StringNames = new string[AllTypes.Length];
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < AllTypes.Length; i++)
		{
			stringBuilder.Length = 0;
			stringBuilder.Append(AllTypes[i]);
			for (int j = 1; j < stringBuilder.Length; j++)
			{
				if (stringBuilder[j] >= 'A' && stringBuilder[j] <= 'Z')
				{
					stringBuilder.Insert(j, " ");
					j++;
				}
			}
			StringNames[i] = stringBuilder.ToString();
		}
	}
}
