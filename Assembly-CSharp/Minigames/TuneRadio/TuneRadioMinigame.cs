using UnityEngine;

public class TuneRadioMinigame : Minigame
{
	public RadioWaveBehaviour actualSignal;

	public DialBehaviour dial;

	public SpriteRenderer redLight;

	public SpriteRenderer greenLight;

	public float Tolerance = 0.1f;

	public float targetAngle;

	public bool finished;

	private float steadyTimer;

	public AudioClip StaticSound;

	public AudioClip RadioSound;

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		targetAngle = dial.DialRange.Next();
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlayDynamicSound("CommsRadio", RadioSound, loop: true, GetRadioVolume, playAsSfx: true);
			SoundManager.Instance.PlayDynamicSound("RadioStatic", StaticSound, loop: true, GetStaticVolume, playAsSfx: true);
		}
	}

	private void GetRadioVolume(AudioSource player, float dt)
	{
		player.volume = 1f - actualSignal.NoiseLevel;
	}

	private void GetStaticVolume(AudioSource player, float dt)
	{
		player.volume = actualSignal.NoiseLevel;
	}

	public void Update()
	{
		if (finished)
		{
			return;
		}
		float f = Mathf.Abs((targetAngle - dial.Value) / dial.DialRange.Width) * 2f;
		actualSignal.NoiseLevel = Mathf.Clamp(Mathf.Sqrt(f), 0f, 1f);
		if (actualSignal.NoiseLevel <= Tolerance)
		{
			redLight.color = new Color(0.35f, 0f, 0f);
			if (!dial.Engaged)
			{
				FinishGame();
				return;
			}
			steadyTimer += Time.deltaTime;
			if (steadyTimer > 1.5f)
			{
				FinishGame();
			}
		}
		else
		{
			redLight.color = new Color(1f, 0f, 0f);
			steadyTimer = 0f;
		}
	}

	private void FinishGame()
	{
		greenLight.color = Color.green;
		finished = true;
		dial.enabled = false;
		dial.SetValue(targetAngle);
		actualSignal.NoiseLevel = 0f;
		if ((bool)PlayerControl.LocalPlayer)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 0);
		}
		StartCoroutine(CoStartClose());
		try
		{
			((SabotageTask)MyTask).MarkContributed();
		}
		catch
		{
		}
	}

	public override void Close()
	{
		SoundManager.Instance.StopSound(StaticSound);
		SoundManager.Instance.StopSound(RadioSound);
		base.Close();
	}
}
