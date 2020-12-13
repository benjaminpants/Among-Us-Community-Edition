using System.Text;
using UnityEngine;

public class StatsPopup : MonoBehaviour
{
	public TextRenderer StatsText;

	private void OnEnable()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"Bodies Reported:\t{StatsManager.Instance.BodiesReported}");
		stringBuilder.AppendLine($"Emergencies Called:\t{StatsManager.Instance.EmergenciesCalled}");
		stringBuilder.AppendLine($"Tasks Completed:\t{StatsManager.Instance.TasksCompleted}");
		stringBuilder.AppendLine($"All Tasks Completed:\t{StatsManager.Instance.CompletedAllTasks}");
		stringBuilder.AppendLine($"Sabotages Fixed:\t{StatsManager.Instance.SabsFixed}");
		stringBuilder.AppendLine($"Impostor Kills:    \t{StatsManager.Instance.ImpostorKills}");
		stringBuilder.AppendLine($"Times Murdered:\t{StatsManager.Instance.TimesMurdered}");
		stringBuilder.AppendLine($"Times Ejected:    \t{StatsManager.Instance.TimesEjected}");
		stringBuilder.AppendLine($"Crewmate Streak:\t{StatsManager.Instance.CrewmateStreak}");
		stringBuilder.AppendLine($"Games Impostor:\t{StatsManager.Instance.TimesImpostor}");
		stringBuilder.AppendLine($"Games Crewmate:\t{StatsManager.Instance.TimesCrewmate}");
		stringBuilder.AppendLine($"Games Started:  \t{StatsManager.Instance.GamesStarted}");
		stringBuilder.AppendLine($"Games Finished:\t{StatsManager.Instance.GamesFinished}");
		stringBuilder.AppendLine($"Impostor Vote Wins:\t{StatsManager.Instance.GetWinReason(GameOverReason.ImpostorByVote)}");
		stringBuilder.AppendLine($"Impostor Kill Wins:\t{StatsManager.Instance.GetWinReason(GameOverReason.ImpostorByKill)}");
		stringBuilder.AppendLine($"Impostor Sabotage Wins:\t{StatsManager.Instance.GetWinReason(GameOverReason.ImpostorBySabotage)}");
		stringBuilder.AppendLine($"Crewmate Vote Wins:\t{StatsManager.Instance.GetWinReason(GameOverReason.HumansByVote)}");
		stringBuilder.AppendLine($"Crewmate Task Wins:\t{StatsManager.Instance.GetWinReason(GameOverReason.HumansByTask)}");
		StatsText.Text = stringBuilder.ToString();
	}
}
