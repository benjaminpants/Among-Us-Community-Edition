using UnityEngine;

public class RefuelStage : MonoBehaviour
{
	public float RefuelDuration = 5f;

	private Color darkRed = new Color32(90, 0, 0, byte.MaxValue);

	private Color red = new Color32(byte.MaxValue, 58, 0, byte.MaxValue);

	private Color green = Color.green;

	public SpriteRenderer redLight;

	public SpriteRenderer greenLight;

	public VerticalGauge srcGauge;

	public VerticalGauge destGauge;

	public AudioClip RefuelSound;

	private float timer;

	private bool isDown;

	private bool complete;

	public NormalPlayerTask MyNormTask
	{
		get;
		set;
	}

	public void Begin()
	{
		timer = (float)(int)MyNormTask.Data[0] / 255f;
	}

	public void FixedUpdate()
	{
		if (complete)
		{
			return;
		}
		if (isDown && timer < 1f)
		{
			timer += Time.fixedDeltaTime / (RefuelDuration * GameOptionsData.TaskDifficultyMult[PlayerControl.GameOptions.TaskDifficulty]);
			MyNormTask.Data[0] = (byte)Mathf.Min(255f, timer * 255f);
			if (timer >= 1f)
			{
				complete = true;
				greenLight.color = green;
				redLight.color = darkRed;
				MyNormTask.Data[0] = 0;
				MyNormTask.Data[1]++;
				if ((int)MyNormTask.Data[1] % 2 == 0)
				{
					MyNormTask.NextStep();
				}
				MyNormTask.UpdateArrow();
			}
		}
		destGauge.value = timer;
		if ((bool)srcGauge)
		{
			srcGauge.value = 1f - timer;
		}
	}

	public void Refuel()
	{
		if (complete)
		{
			base.transform.parent.GetComponent<Minigame>().Close();
			return;
		}
		isDown = !isDown;
		redLight.color = (isDown ? red : darkRed);
		if (isDown)
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlayDynamicSound("Refuel", RefuelSound, loop: true, GetRefuelDynamics, playAsSfx: true);
			}
		}
		else
		{
			SoundManager.Instance.StopSound(RefuelSound);
		}
	}

	private void GetRefuelDynamics(AudioSource player, float dt)
	{
		player.volume = 1f;
		player.pitch = Mathf.Lerp(0.75f, 1.25f, timer);
	}
}
