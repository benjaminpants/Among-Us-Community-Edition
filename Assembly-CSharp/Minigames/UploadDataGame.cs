using System.Collections;
using System.Text;
using PowerTools;
using UnityEngine;

public class UploadDataGame : Minigame
{
	public SpriteAnim LeftFolder;

	public SpriteAnim RightFolder;

	public AnimationClip FolderOpen;

	public AnimationClip FolderClose;

	public SpriteRenderer Runner;

	public HorizontalGauge Gauge;

	public TextRenderer PercentText;

	public TextRenderer EstimatedText;

	public TextRenderer SourceText;

	public TextRenderer TargetText;

	public SpriteRenderer Button;

	public Sprite DownloadImage;

	public GameObject Status;

	public GameObject Tower;

	private int count;

	private float timer;

	public const float RandomChunks = 5f;

	public const float ConstantTime = 3f;

	private bool running = true;

	public override void Begin(PlayerTask task)
	{
		PlayerControl.LocalPlayer.SetPlayerMaterialColors(Runner);
		base.Begin(task);
		if (MyNormTask.taskStep == 0)
		{
			Button.sprite = DownloadImage;
			Tower.SetActive(value: false);
			SourceText.Text = MyTask.StartAt.ToString();
			TargetText.Text = "My Tablet";
		}
		else
		{
			SourceText.Text = "My Tablet";
			TargetText.Text = "Headquarters";
		}
	}

	public void Click()
	{
		StartCoroutine(Transition());
	}

	private IEnumerator Transition()
	{
		Button.gameObject.SetActive(value: false);
		Status.SetActive(value: true);
		float target = Gauge.transform.localScale.x;
		for (float t = 0f; t < 0.15f; t += Time.deltaTime)
		{
			Gauge.transform.localScale = new Vector3(t / 0.15f * target, 1f, 1f);
			yield return null;
		}
		StartCoroutine(PulseText());
		StartCoroutine(DoRun());
		StartCoroutine(DoText());
		StartCoroutine(DoPercent());
	}

	private IEnumerator PulseText()
	{
		MeshRenderer rend2 = PercentText.GetComponent<MeshRenderer>();
		MeshRenderer rend1 = EstimatedText.GetComponent<MeshRenderer>();
		Color gray = new Color(0.3f, 0.3f, 0.3f, 1f);
		while (running)
		{
			yield return new WaitForLerp(0.4f, delegate(float t)
			{
				Color value2 = Color.Lerp(Color.black, gray, t);
				rend2.material.SetColor("_OutlineColor", value2);
				rend1.material.SetColor("_OutlineColor", value2);
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
	}

	private IEnumerator DoPercent()
	{
		while (running)
		{
			float num = (float)count / 5f * 0.7f + timer / 3f * 0.3f;
			if (num >= 1f)
			{
				running = false;
			}
			num = Mathf.Clamp(num, 0f, 1f);
			Gauge.Value = num;
			PercentText.Text = Mathf.RoundToInt(num * 100f) + "%";
			yield return null;
		}
	}

	private IEnumerator DoText()
	{
		StringBuilder txt = new StringBuilder("Estimated Time: ");
		int baselen = txt.Length;
		int max = 604800;
		count = 0;
		while ((float)count < 5f)
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
			EstimatedText.Text = txt.ToString();
			max /= 4;
			yield return new WaitForSeconds(FloatRange.Next(0.6f, 1.2f));
			count++;
		}
		for (timer = 0f; timer < 3f; timer += Time.deltaTime)
		{
			txt.Length = baselen;
			int num6 = Mathf.RoundToInt(3f - timer);
			txt.Append(num6 + "s");
			EstimatedText.Text = txt.ToString();
			yield return null;
		}
	}

	private IEnumerator DoRun()
	{
		while (running)
		{
			LeftFolder.Play(FolderOpen);
			Vector3 pos = Runner.transform.localPosition;
			yield return new WaitForLerp(1.125f, delegate(float t)
			{
				pos.x = Mathf.Lerp(-1.25f, 0.5625f, t);
				Runner.transform.localPosition = pos;
			});
			LeftFolder.Play(FolderClose);
			RightFolder.Play(FolderOpen);
			yield return new WaitForLerp(1.375f, delegate(float t)
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
}
