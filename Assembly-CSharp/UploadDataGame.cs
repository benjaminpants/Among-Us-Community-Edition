using System;
using System.Collections;
using System.Text;
using PowerTools;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class UploadDataGame : Minigame
{
	// Token: 0x06000026 RID: 38 RVA: 0x0000B6BC File Offset: 0x000098BC
	public override void Begin(PlayerTask task)
	{
		PlayerControl.LocalPlayer.SetPlayerMaterialColors(this.Runner);
		base.Begin(task);
		if (this.MyNormTask.taskStep == 0)
		{
			this.Button.sprite = this.DownloadImage;
			this.Tower.SetActive(false);
			this.SourceText.Text = this.MyTask.StartAt.ToString();
			this.TargetText.Text = "My Tablet";
			return;
		}
		this.SourceText.Text = "My Tablet";
		this.TargetText.Text = "Headquarters";
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002344 File Offset: 0x00000544
	public void Click()
	{
		base.StartCoroutine(this.Transition());
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00002353 File Offset: 0x00000553
	private IEnumerator Transition()
	{
		this.Button.gameObject.SetActive(false);
		this.Status.SetActive(true);
		float target = this.Gauge.transform.localScale.x;
		for (float t = 0f; t < 0.15f; t += Time.deltaTime)
		{
			this.Gauge.transform.localScale = new Vector3(t / 0.15f * target, 1f, 1f);
			yield return null;
		}
		base.StartCoroutine(this.PulseText());
		base.StartCoroutine(this.DoRun());
		base.StartCoroutine(this.DoText());
		base.StartCoroutine(this.DoPercent());
		yield break;
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00002362 File Offset: 0x00000562
	private IEnumerator PulseText()
	{
		MeshRenderer rend2 = this.PercentText.GetComponent<MeshRenderer>();
		MeshRenderer rend1 = this.EstimatedText.GetComponent<MeshRenderer>();
		Color gray = new Color(0.3f, 0.3f, 0.3f, 1f);
		while (this.running)
		{
			yield return new WaitForLerp(0.4f, delegate(float t)
			{
				Color value = Color.Lerp(Color.black, gray, t);
				rend2.material.SetColor("_OutlineColor", value);
				rend1.material.SetColor("_OutlineColor", value);
			});
			yield return new WaitForLerp(0.4f, delegate(float t)
			{
				Color value = Color.Lerp(gray, Color.black, t);
				rend2.material.SetColor("_OutlineColor", value);
				rend1.material.SetColor("_OutlineColor", value);
			});
		}
		rend2.material.SetColor("_OutlineColor", Color.black);
		rend1.material.SetColor("_OutlineColor", Color.black);
		yield break;
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00002371 File Offset: 0x00000571
	private IEnumerator DoPercent()
	{
		while (this.running)
		{
			float num = (float)this.count / 5f * 0.7f + this.timer / 3f * 0.3f;
			if (num >= 1f)
			{
				this.running = false;
			}
			num = Mathf.Clamp(num, 0f, 1f);
			this.Gauge.Value = num;
			this.PercentText.Text = Mathf.RoundToInt(num * 100f) + "%";
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002380 File Offset: 0x00000580
	private IEnumerator DoText()
	{
		StringBuilder txt = new StringBuilder("Estimated Time: ");
		int baselen = txt.Length;
		int max = 604800;
		this.count = 0;
		while ((float)this.count < 5f)
		{
			txt.Length = baselen;
			int num = IntRange.Next(max / 6, max);
			int num2 = num / 86400;
			if (num2 > 0)
			{
				txt.Append(num2 + "d ");
			}
			int num3 = num / 3600 % 24;
			if (num3 > 0)
			{
				txt.Append(num3 + "hr ");
			}
			int num4 = num / 60 % 60;
			if (num4 > 0)
			{
				txt.Append(num4 + "m ");
			}
			int num5 = num % 60;
			if (num5 > 0)
			{
				txt.Append(num5 + "s");
			}
			this.EstimatedText.Text = txt.ToString();
			max /= 4;
			yield return new WaitForSeconds(FloatRange.Next(0.6f, 1.2f));
			this.count++;
		}
		this.timer = 0f;
		while (this.timer < 3f)
		{
			txt.Length = baselen;
			int num6 = Mathf.RoundToInt(3f - this.timer);
			txt.Append(num6 + "s");
			this.EstimatedText.Text = txt.ToString();
			yield return null;
			this.timer += Time.deltaTime;
		}
		yield break;
	}

	// Token: 0x0600002C RID: 44 RVA: 0x0000238F File Offset: 0x0000058F
	private IEnumerator DoRun()
	{
		while (running)
		{
			LeftFolder.Play(FolderOpen);
			Vector3 pos = Runner.transform.localPosition;
			yield return new WaitForLerp(1.125f, delegate (float t)
			{
				pos.x = Mathf.Lerp(-1.25f, 0.5625f, t);
				Runner.transform.localPosition = pos;
			});
			LeftFolder.Play(FolderClose);
			RightFolder.Play(FolderOpen);
			yield return new WaitForLerp(1.375f, delegate (float t)
			{
				pos.x = Mathf.Lerp(0.5625f, 1.25f, t);
				Runner.transform.localPosition = pos;
			});
			yield return new WaitForAnimationFinish(RightFolder, FolderClose);
		}
		EstimatedText.Text = "Complete";
		MyNormTask.NextStep();
		StartCoroutine(CoStartClose());
	}

	// Token: 0x04000038 RID: 56
	public SpriteAnim LeftFolder;

	// Token: 0x04000039 RID: 57
	public SpriteAnim RightFolder;

	// Token: 0x0400003A RID: 58
	public AnimationClip FolderOpen;

	// Token: 0x0400003B RID: 59
	public AnimationClip FolderClose;

	// Token: 0x0400003C RID: 60
	public SpriteRenderer Runner;

	// Token: 0x0400003D RID: 61
	public HorizontalGauge Gauge;

	// Token: 0x0400003E RID: 62
	public TextRenderer PercentText;

	// Token: 0x0400003F RID: 63
	public TextRenderer EstimatedText;

	// Token: 0x04000040 RID: 64
	public TextRenderer SourceText;

	// Token: 0x04000041 RID: 65
	public TextRenderer TargetText;

	// Token: 0x04000042 RID: 66
	public SpriteRenderer Button;

	// Token: 0x04000043 RID: 67
	public Sprite DownloadImage;

	// Token: 0x04000044 RID: 68
	public GameObject Status;

	// Token: 0x04000045 RID: 69
	public GameObject Tower;

	// Token: 0x04000046 RID: 70
	private int count;

	// Token: 0x04000047 RID: 71
	private float timer;

	// Token: 0x04000048 RID: 72
	public const float RandomChunks = 5f;

	// Token: 0x04000049 RID: 73
	public const float ConstantTime = 3f;

	// Token: 0x0400004A RID: 74
	private bool running = true;
}
