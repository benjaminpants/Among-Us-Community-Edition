using System;
using System.Collections;
using System.Linq;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public class EmergencyMinigame : Minigame
{
	// Token: 0x06000A17 RID: 2583 RVA: 0x00008283 File Offset: 0x00006483
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.Update();
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x00034A7C File Offset: 0x00032C7C
	public void Update()
	{
		if (ShipStatus.Instance.Timer < 15f)
		{
			int num = Mathf.CeilToInt(15f - ShipStatus.Instance.Timer);
			this.ButtonActive = false;
			this.StatusText.Text = "CREWMATES MUST WAIT\r\n\r\nBEFORE FIRST EMERGENCY";
			this.NumberText.Text = num + "s";
			this.ClosedLid.gameObject.SetActive(true);
			this.OpenLid.gameObject.SetActive(false);
			return;
		}
		if (!PlayerControl.LocalPlayer.myTasks.Any(new Func<PlayerTask, bool>(PlayerTask.TaskIsEmergency)))
		{
			if (this.state == 1)
			{
				return;
			}
			this.state = 1;
			int remainingEmergencies = PlayerControl.LocalPlayer.RemainingEmergencies;
			this.StatusText.Text = string.Format("CREWMEMBER {0} HAS\r\n\r\nEMERGENCY MEETINGS LEFT", PlayerControl.LocalPlayer.Data.PlayerName);
			this.NumberText.Text = remainingEmergencies.ToString();
			this.ButtonActive = (remainingEmergencies > 0);
			this.ClosedLid.gameObject.SetActive(!this.ButtonActive);
			this.OpenLid.gameObject.SetActive(this.ButtonActive);
			return;
		}
		else
		{
			if (this.state == 2)
			{
				return;
			}
			this.state = 2;
			this.ButtonActive = false;
			this.StatusText.Text = "EMERGENCY MEETINGS CANNOT\r\nBE CALLED DURING CRISES";
			this.NumberText.Text = string.Empty;
			this.ClosedLid.gameObject.SetActive(true);
			this.OpenLid.gameObject.SetActive(false);
			return;
		}
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00034C0C File Offset: 0x00032E0C
	public void CallMeeting()
	{
		if (!PlayerControl.LocalPlayer.myTasks.Any(new Func<PlayerTask, bool>(PlayerTask.TaskIsEmergency)) && PlayerControl.LocalPlayer.RemainingEmergencies > 0 && this.ButtonActive)
		{
			this.StatusText.Text = "EMERGENCY MEETING REQUESTED\r\nWAITING FOR HOST";
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(this.ButtonSound, false, 1f);
			}
			PlayerControl.LocalPlayer.RemainingEmergencies--;
			PlayerControl.LocalPlayer.CmdReportDeadBody(null);
			this.ButtonActive = false;
		}
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x00034CA0 File Offset: 0x00032EA0
	private float easeOutElastic(float t)
	{
		float num = 0.3f;
		return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - num / 4f) * 6.28318548f / num) + 1f;
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x00008292 File Offset: 0x00006492
	protected override IEnumerator CoAnimateOpen()
	{
		for (float timer = 0f; timer < 0.2f; timer += Time.deltaTime)
		{
			float t = timer / 0.2f;
			base.transform.localPosition = new Vector3(0f, Mathf.SmoothStep(-8f, 0f, t), -50f);
			yield return null;
		}
		base.transform.localPosition = new Vector3(0f, 0f, -50f);
		Vector3 meetingPos = this.meetingButton.localPosition;
		for (float timer = 0f; timer < 0.1f; timer += Time.deltaTime)
		{
			float num = timer / 0.1f;
			meetingPos.y = Mathf.Sin(3.14159274f * num) * 1f / (num * 5f + 4f) - 0.882f;
			this.meetingButton.localPosition = meetingPos;
			yield return null;
		}
		meetingPos.y = -0.882f;
		this.meetingButton.localPosition = meetingPos;
		yield break;
	}

	// Token: 0x040009B9 RID: 2489
	public SpriteRenderer ClosedLid;

	// Token: 0x040009BA RID: 2490
	public SpriteRenderer OpenLid;

	// Token: 0x040009BB RID: 2491
	public Transform meetingButton;

	// Token: 0x040009BC RID: 2492
	public TextRenderer StatusText;

	// Token: 0x040009BD RID: 2493
	public TextRenderer NumberText;

	// Token: 0x040009BE RID: 2494
	public bool ButtonActive = true;

	// Token: 0x040009BF RID: 2495
	public AudioClip ButtonSound;

	// Token: 0x040009C0 RID: 2496
	private int state;

	// Token: 0x040009C1 RID: 2497
	public const int MinEmergencyTime = 15;
}
