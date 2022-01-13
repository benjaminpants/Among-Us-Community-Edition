using UnityEngine;

public class PingTracker : MonoBehaviour
{
	public TextRenderer text;
//	public TextRenderer2 text2;

	private void Update()
	{
		if ((bool)AmongUsClient.Instance)
		{
			if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
			{
				base.gameObject.SetActive(value: false);
			}
	//		text.Text = $"Ping: {AmongUsClient.Instance.Ping} ms";
			text.Text = $"AUTOM ver 1.0";
		}
	}
}
