using UnityEngine;

public class WeatherMinigame : Minigame
{
	public float RefuelDuration = 5f;

	public VerticalGauge destGauge;

	private bool isDown;

	private float timer;

	public void FixedUpdate()
	{
		if (isDown && timer < 1f)
		{
			timer += Time.fixedDeltaTime / RefuelDuration;
			MyNormTask.Data[0] = (byte)Mathf.Min(255f, timer * 255f);
			if (timer >= 1f)
			{
				timer = 1f;
				MyNormTask.NextStep();
				StartCoroutine(CoStartClose());
			}
		}
		destGauge.value = timer;
	}

	public void StartStopFill()
	{
		isDown = !isDown;
	}
}
