using UnityEngine;

public class PingTracker : MonoBehaviour
{
	public TextRenderer text;

	private void Update()
	{
		if ((bool)AmongUsClient.Instance)
		{
			if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
			{
				text.Text = "> Among Us: CE <\n> " + VersionShower.BuildID + " <\n[FFFF00FF]> Alpha Build <[]";
				//text.Text = ((int)(1.0f / Time.smoothDeltaTime)).ToString();
			}
			else
			{
				text.Text = $"Ping: {AmongUsClient.Instance.Ping} ms\n> " + VersionShower.BuildID + " <\n[FFFF00FF]> Alpha Build <[]";
			}
		}
	}
}
