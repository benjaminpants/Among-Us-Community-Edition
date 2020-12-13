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
				base.gameObject.SetActive(value: false);
			}
			text.Text = $"Ping: {AmongUsClient.Instance.Ping} ms";
		}
	}
}
