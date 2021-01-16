using UnityEngine;

public class SweepMinigame : Minigame
{
	public SpriteRenderer[] Spinners;

	public SpriteRenderer[] Shadows;

	public SpriteRenderer[] Lights;

	public HorizontalGauge[] Gauges;

	private int spinnerIdx;

	private float timer;

	public float SpinRate = 45f;

	private float initialTimer;

	public AudioClip SpinningSound;

	public AudioClip AcceptSound;

	public AudioClip RejectSound;

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		SpinRate *= GameOptionsData.TaskDifficultyMult[PlayerControl.GameOptions.TaskDifficulty];
		if (PlayerControl.GameOptions.TaskDifficulty == 3)
        {
			SpinRate *= 0.5f;
        }
		ResetGauges();
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(SpinningSound, loop: true);
		}
	}

	public void FixedUpdate()
	{
		float num = Mathf.Clamp(2f - timer / 30f, 1f, 2f);
		timer += Time.fixedDeltaTime * num;
		if (spinnerIdx < Spinners.Length)
		{
			float num2 = CalcXPerc();
			Gauges[spinnerIdx].Value = ((num2 < 13f) ? 0.9f : 0.1f);
			Quaternion localRotation = Quaternion.Euler(0f, 0f, timer * SpinRate);
			Spinners[spinnerIdx].transform.localRotation = localRotation;
			Shadows[spinnerIdx].transform.localRotation = localRotation;
			Lights[spinnerIdx].enabled = num2 < 13f;
		}
		for (int i = 0; i < Gauges.Length; i++)
		{
			HorizontalGauge horizontalGauge = Gauges[i];
			if (i < spinnerIdx)
			{
				horizontalGauge.Value = 0.95f;
			}
			if (i > spinnerIdx)
			{
				horizontalGauge.Value = 0.05f;
			}
			horizontalGauge.Value += (Mathf.PerlinNoise(i, Time.time * 51f) - 0.5f) * 0.025f;
		}
	}

	private float CalcXPerc()
	{
		int num = (int)(timer * SpinRate) % 360;
		return Mathf.Min(360 - num, num);
	}

	public void HitButton(int i)
	{
		if (i != spinnerIdx)
		{
			return;
		}
		if (CalcXPerc() < 13f)
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(AcceptSound, loop: false);
			}
			Spinners[spinnerIdx].transform.localRotation = Quaternion.identity;
			Shadows[spinnerIdx].transform.localRotation = Quaternion.identity;
			spinnerIdx++;
			timer = initialTimer;
			if (spinnerIdx >= Gauges.Length)
			{
				SoundManager.Instance.StopSound(SpinningSound);
				MyNormTask.NextStep();
				StartCoroutine(CoStartClose());
			}
		}
		else
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(RejectSound, loop: false);
			}
			ResetGauges();
		}
	}

	private void ResetGauges()
	{
		spinnerIdx = 0;
		timer = FloatRange.Next(1f, 3f);
		initialTimer = timer;
		for (int i = 0; i < Gauges.Length; i++)
		{
			Lights[i].enabled = false;
			Spinners[i].transform.localRotation = Quaternion.Euler(0f, 0f, timer * SpinRate);
			Shadows[i].transform.localRotation = Quaternion.Euler(0f, 0f, timer * SpinRate);
		}
	}
}
