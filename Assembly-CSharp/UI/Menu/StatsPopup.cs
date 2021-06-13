using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class StatsPopup : MonoBehaviour
{
	public TextRenderer StatsText;

    public int Page;

    private const int LinesPerPage = 17;

    private void OnEnable()
    {
        Page = 0;
    }

	private void Update()
	{
        StringBuilder stringBuilder = new StringBuilder();
        List<string> Strings = new List<string>();
        Strings.Add($"Tasks Completed:\t{StatsManager.Instance.TasksCompleted}");
        Strings.Add($"All Tasks Completed:\t{StatsManager.Instance.CompletedAllTasks}");
        Strings.Add($"Sabotages Fixed:\t{StatsManager.Instance.SabsFixed}");
        Strings.Add($"Games Finished:\t{StatsManager.Instance.GamesFinished}");
        Strings.Add($"Stalemates: \t\t{StatsManager.Instance.Stalemates}");
        Strings.Add($"Deaths: \t\t{StatsManager.Instance.TimesMurdered}");
        foreach (KeyValuePair<string, uint> kvp in StatsManager.Instance.GameStarts)
        {
            Strings.Add(kvp.Key + " Games: \t\t" + kvp.Value);
        }
        foreach (KeyValuePair<string, uint> kvp in StatsManager.Instance.RoleWins)
        {
            if (kvp.Key == "Crewmate" || kvp.Key == "Impostor") //not proper UUIDs
            {
                Strings.Add(kvp.Key + " W/L Ratio: \t[00FF00FF]" + kvp.Value + "[]/[FF0000FF]" + StatsManager.Instance.RoleLoses[kvp.Key] + "[]" + " [FFFF00FF](" + (((float)kvp.Value / ((float)StatsManager.Instance.RoleLoses[kvp.Key] + (float)kvp.Value)) * 100f) + "%)[]");
                continue;
            }
            CE_Role role = CE_RoleManager.GetActualRoleFromUUID(kvp.Key);
            if (role == null)
            {
                continue;
            }
            Strings.Add(role.RoleName + " W/L Ratio: \t[00FF00FF]" + kvp.Value + "[]/[FF0000FF]" + StatsManager.Instance.RoleLoses[kvp.Key] + "[]" + " [FFFF00FF](" + (((float)kvp.Value / ((float)StatsManager.Instance.RoleLoses[kvp.Key] + (float)kvp.Value)) * 100f) + "%)[]");
        }

        foreach (KeyValuePair<string, uint> kvp in StatsManager.Instance.RoleEjects)
        {
            if (kvp.Key == "Crewmate" || kvp.Key == "Impostor") //not proper UUIDs
            {
                Strings.Add("Times ejected as " + kvp.Key.AOrAn(false) + " " + kvp.Key + ": \t" + kvp.Value);
                continue;
            }
            CE_Role role = CE_RoleManager.GetActualRoleFromUUID(kvp.Key);
            if (role == null)
            {
                continue;
            }
            Strings.Add("Times ejected as " + role.RoleName.AOrAn(false) + " " + role.RoleName + ": \t" + kvp.Value);
        }

        foreach (KeyValuePair<string, uint> kvp in StatsManager.Instance.RoleKills)
        {
            if (kvp.Key == "Crewmate" || kvp.Key == "Impostor") //not proper UUIDs
            {
                Strings.Add(kvp.Key + " Kills: \t" + kvp.Value);
                continue;
            }
            CE_Role role = CE_RoleManager.GetActualRoleFromUUID(kvp.Key);
            if (role == null)
            {
                continue;
            }
            Strings.Add(role.RoleName + " Kills: \t" + kvp.Value);
        }

        foreach (KeyValuePair<string, uint> kvp in StatsManager.Instance.AbilityUses)
        {
            if (kvp.Key == "Crewmate" || kvp.Key == "Impostor") //not proper UUIDs
            {
                Strings.Add(kvp.Key + " Ability Uses: \t" + kvp.Value);
                continue;
            }
            CE_Role role = CE_RoleManager.GetActualRoleFromUUID(kvp.Key);
            if (role == null)
            {
                continue;
            }
            Strings.Add(role.RoleName + " Ability Uses: \t" + kvp.Value);
        }




        //actually create the string builder
        stringBuilder.AppendLine("[0000FFFF]Use the arrow keys to scroll.[] (" + (Page + 1) + "/" + (Mathf.CeilToInt(Strings.Count / LinesPerPage) + 1) + ")");
        for (int i=(Page * LinesPerPage); (i - (Page * LinesPerPage)) < Mathf.Clamp(Strings.Count,0, LinesPerPage); i++)
        {
            if (!(i > (Strings.Count - 1)))
            {
                stringBuilder.AppendLine(Strings[i]);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Page--;
            Page = Mathf.Clamp(Page, 0, Mathf.CeilToInt(Strings.Count / LinesPerPage));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Page++;
            Page = Mathf.Clamp(Page, 0, Mathf.CeilToInt(Strings.Count / LinesPerPage));
        }
        StatsText.Text = stringBuilder.ToString();
	}
}
