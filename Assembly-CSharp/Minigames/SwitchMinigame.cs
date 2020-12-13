using UnityEngine;

public class SwitchMinigame : Minigame
{
	public Color OnColor = Color.green;

	public Color OffColor = new Color(0.1f, 0.3f, 0.1f);

	private ShipStatus ship;

	public SpriteRenderer[] switches;

	public SpriteRenderer[] lights;

	public RadioWaveBehaviour top;

	public HorizontalGauge middle;

	public FlatWaveBehaviour bottom;

	public override void Begin(PlayerTask task)
	{
		ship = Object.FindObjectOfType<ShipStatus>();
		SwitchSystem switchSystem = ship.Systems[SystemTypes.Electrical] as SwitchSystem;
		for (int i = 0; i < switches.Length; i++)
		{
			byte b = (byte)(1 << i);
			int num = switchSystem.ActualSwitches & b;
			lights[i].color = ((num == (switchSystem.ExpectedSwitches & b)) ? OnColor : OffColor);
			switches[i].flipY = num >> i == 0;
		}
	}

	public void FixedUpdate()
	{
		if (amClosing != 0)
		{
			return;
		}
		int num = 0;
		SwitchSystem switchSystem = ship.Systems[SystemTypes.Electrical] as SwitchSystem;
		for (int i = 0; i < switches.Length; i++)
		{
			byte b = (byte)(1 << i);
			int num2 = switchSystem.ActualSwitches & b;
			if (num2 == (switchSystem.ExpectedSwitches & b))
			{
				num++;
				lights[i].color = OnColor;
			}
			else
			{
				lights[i].color = OffColor;
			}
			switches[i].flipY = num2 >> i == 0;
		}
		float num3 = (float)num / (float)switches.Length;
		bottom.Center = 0.47f * num3;
		top.NoiseLevel = 1f - num3;
		middle.Value = switchSystem.Level + (Mathf.PerlinNoise(0f, Time.time * 51f) - 0.5f) * 0.04f;
		if (num == switches.Length)
		{
			StartCoroutine(CoStartClose(0.5f));
		}
	}

	public void FlipSwitch(int switchIdx)
	{
		if (amClosing != 0)
		{
			return;
		}
		int num = 0;
		SwitchSystem switchSystem = ship.Systems[SystemTypes.Electrical] as SwitchSystem;
		for (int i = 0; i < switches.Length; i++)
		{
			byte b = (byte)(1 << i);
			if ((switchSystem.ActualSwitches & b) == (switchSystem.ExpectedSwitches & b))
			{
				num++;
			}
		}
		if (num != switches.Length)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Electrical, (byte)switchIdx);
			try
			{
				((SabotageTask)MyTask).MarkContributed();
			}
			catch
			{
			}
		}
	}
}
