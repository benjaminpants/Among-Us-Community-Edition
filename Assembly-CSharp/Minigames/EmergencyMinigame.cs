using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class EmergencyMinigame : Minigame
{
	public SpriteRenderer ClosedLid;

	public SpriteRenderer OpenLid;

	public Transform meetingButton;

	public TextRenderer StatusText;

	public TextRenderer NumberText;

	public bool ButtonActive = true;

	public AudioClip ButtonSound;

	private int state;

	public const int MinEmergencyTime = 15;

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		Update();
	}

	public void Update()
	{
		if (ShipStatus.Instance.Timer < 15f)
		{
			int num = Mathf.CeilToInt(15f - ShipStatus.Instance.Timer);
			ButtonActive = false;
			StatusText.Text = "CREWMATES MUST WAIT\r\n\r\nBEFORE FIRST EMERGENCY";
			NumberText.Text = num + "s";
			ClosedLid.gameObject.SetActive(value: true);
			OpenLid.gameObject.SetActive(value: false);
		}
		else if (!PlayerControl.LocalPlayer.myTasks.Any(PlayerTask.TaskIsEmergency))
		{
			if (state != 1)
			{
				state = 1;
				int remainingEmergencies = PlayerControl.LocalPlayer.RemainingEmergencies;
				StatusText.Text = $"CREWMEMBER {PlayerControl.LocalPlayer.Data.PlayerName} HAS\r\n\r\nEMERGENCY MEETINGS LEFT";
				NumberText.Text = remainingEmergencies.ToString();
				ButtonActive = remainingEmergencies > 0;
				ClosedLid.gameObject.SetActive(!ButtonActive);
				OpenLid.gameObject.SetActive(ButtonActive);
			}
		}
		else if (state != 2)
		{
			state = 2;
			ButtonActive = false;
			StatusText.Text = "EMERGENCY MEETINGS CANNOT\r\nBE CALLED DURING CRISES";
			NumberText.Text = string.Empty;
			ClosedLid.gameObject.SetActive(value: true);
			OpenLid.gameObject.SetActive(value: false);
		}
	}

	public void CallMeeting()
	{
		if (!PlayerControl.LocalPlayer.myTasks.Any(PlayerTask.TaskIsEmergency) && PlayerControl.LocalPlayer.RemainingEmergencies > 0 && ButtonActive)
		{
			StatusText.Text = "EMERGENCY MEETING REQUESTED\r\nWAITING FOR HOST";
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(ButtonSound, loop: false);
			}
			PlayerControl.LocalPlayer.RemainingEmergencies--;
			PlayerControl.LocalPlayer.CmdReportDeadBody(null);
			ButtonActive = false;
		}
	}

	private float easeOutElastic(float t)
	{
		float num = 0.3f;
		return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - num / 4f) * ((float)Math.PI * 2f) / num) + 1f;
	}

	protected override IEnumerator CoAnimateOpen()
	{
		for (float timer2 = 0f; timer2 < 0.2f; timer2 += Time.deltaTime)
		{
			float t = timer2 / 0.2f;
			base.transform.localPosition = new Vector3(0f, Mathf.SmoothStep(-8f, 0f, t), -50f);
			yield return null;
		}
		base.transform.localPosition = new Vector3(0f, 0f, -50f);
		Vector3 meetingPos = meetingButton.localPosition;
		for (float timer2 = 0f; timer2 < 0.1f; timer2 += Time.deltaTime)
		{
			float num = timer2 / 0.1f;
			meetingPos.y = Mathf.Sin((float)Math.PI * num) * 1f / (num * 5f + 4f) - 0.882f;
			meetingButton.localPosition = meetingPos;
			yield return null;
		}
		meetingPos.y = -0.882f;
		meetingButton.localPosition = meetingPos;
	}
}
