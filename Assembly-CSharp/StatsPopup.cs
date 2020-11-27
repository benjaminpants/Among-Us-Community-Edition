using System;
using System.Text;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class StatsPopup : MonoBehaviour
{
	// Token: 0x06000B41 RID: 2881 RVA: 0x00038260 File Offset: 0x00036460
	private void OnEnable()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("Bodies Reported:\t{0}", StatsManager.Instance.BodiesReported));
		stringBuilder.AppendLine(string.Format("Emergencies Called:\t{0}", StatsManager.Instance.EmergenciesCalled));
		stringBuilder.AppendLine(string.Format("Tasks Completed:\t{0}", StatsManager.Instance.TasksCompleted));
		stringBuilder.AppendLine(string.Format("All Tasks Completed:\t{0}", StatsManager.Instance.CompletedAllTasks));
		stringBuilder.AppendLine(string.Format("Sabotages Fixed:\t{0}", StatsManager.Instance.SabsFixed));
		stringBuilder.AppendLine(string.Format("Impostor Kills:    \t{0}", StatsManager.Instance.ImpostorKills));
		stringBuilder.AppendLine(string.Format("Times Murdered:\t{0}", StatsManager.Instance.TimesMurdered));
		stringBuilder.AppendLine(string.Format("Times Ejected:    \t{0}", StatsManager.Instance.TimesEjected));
		stringBuilder.AppendLine(string.Format("Crewmate Streak:\t{0}", StatsManager.Instance.CrewmateStreak));
		stringBuilder.AppendLine(string.Format("Games Impostor:\t{0}", StatsManager.Instance.TimesImpostor));
		stringBuilder.AppendLine(string.Format("Games Crewmate:\t{0}", StatsManager.Instance.TimesCrewmate));
		stringBuilder.AppendLine(string.Format("Games Started:  \t{0}", StatsManager.Instance.GamesStarted));
		stringBuilder.AppendLine(string.Format("Games Finished:\t{0}", StatsManager.Instance.GamesFinished));
		stringBuilder.AppendLine(string.Format("Impostor Vote Wins:\t{0}", StatsManager.Instance.GetWinReason(GameOverReason.ImpostorByVote)));
		stringBuilder.AppendLine(string.Format("Impostor Kill Wins:\t{0}", StatsManager.Instance.GetWinReason(GameOverReason.ImpostorByKill)));
		stringBuilder.AppendLine(string.Format("Impostor Sabotage Wins:\t{0}", StatsManager.Instance.GetWinReason(GameOverReason.ImpostorBySabotage)));
		stringBuilder.AppendLine(string.Format("Crewmate Vote Wins:\t{0}", StatsManager.Instance.GetWinReason(GameOverReason.HumansByVote)));
		stringBuilder.AppendLine(string.Format("Crewmate Task Wins:\t{0}", StatsManager.Instance.GetWinReason(GameOverReason.HumansByTask)));
		this.StatsText.Text = stringBuilder.ToString();
	}

	// Token: 0x04000AD4 RID: 2772
	public TextRenderer StatsText;
}
