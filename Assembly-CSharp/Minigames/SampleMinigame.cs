using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class SampleMinigame : Minigame
{
	// Token: 0x1700011C RID: 284
	// (get) Token: 0x0600076D RID: 1901 RVA: 0x0000696F File Offset: 0x00004B6F
	// (set) Token: 0x0600076E RID: 1902 RVA: 0x0000697E File Offset: 0x00004B7E
	private SampleMinigame.States State
	{
		get
		{
			return (SampleMinigame.States)this.MyNormTask.Data[0];
		}
		set
		{
			this.MyNormTask.Data[0] = (byte)value;
		}
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x0600076F RID: 1903 RVA: 0x0000698E File Offset: 0x00004B8E
	// (set) Token: 0x06000770 RID: 1904 RVA: 0x0000699D File Offset: 0x00004B9D
	private int AnomalyId
	{
		get
		{
			return (int)this.MyNormTask.Data[1];
		}
		set
		{
			this.MyNormTask.Data[1] = (byte)value;
		}
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x000069AE File Offset: 0x00004BAE
	public void Awake()
	{
		this.dropSounds = new RandomFill<AudioClip>();
		this.dropSounds.Set(this.DropSounds);
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x0002A82C File Offset: 0x00028A2C
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		SampleMinigame.States state = this.State;
		if (state <= SampleMinigame.States.AwaitingStart)
		{
			if (state != SampleMinigame.States.PrepareSample)
			{
				if (state != SampleMinigame.States.AwaitingStart)
				{
					return;
				}
				this.LowerText.Text = "Press To Begin      -->";
				this.SetPlatformTop();
				return;
			}
			else
			{
				base.StartCoroutine(this.BringPanelUp(true));
			}
		}
		else
		{
			if (state == SampleMinigame.States.Selection)
			{
				for (int i = 0; i < this.Tubes.Length; i++)
				{
					this.Tubes[i].color = Color.blue;
				}
				this.Tubes[this.AnomalyId].color = Color.red;
				this.LowerText.Text = "Select Anomaly";
				this.SetPlatformTop();
				return;
			}
			if (state == SampleMinigame.States.Processing)
			{
				for (int j = 0; j < this.Tubes.Length; j++)
				{
					this.Tubes[j].color = Color.blue;
				}
				this.LowerText.Text = SampleMinigame.ProcessingStrings.Random<string>();
				this.SetPlatformBottom();
				return;
			}
		}
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x0002A924 File Offset: 0x00028B24
	private void SetPlatformBottom()
	{
		Vector3 localPosition = this.CenterPanel.transform.localPosition;
		localPosition.y = this.platformY.min;
		this.CenterPanel.transform.localPosition = localPosition;
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x0002A968 File Offset: 0x00028B68
	private void SetPlatformTop()
	{
		Vector3 localPosition = this.CenterPanel.transform.localPosition;
		localPosition.y = this.platformY.max;
		this.CenterPanel.transform.localPosition = localPosition;
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x0002A9AC File Offset: 0x00028BAC
	public void FixedUpdate()
	{
		if (this.State == SampleMinigame.States.Processing)
		{
			if (this.MyNormTask.TaskTimer <= 0f)
			{
				this.State = SampleMinigame.States.Selection;
				this.LowerText.Text = "Select Anomaly";
				this.UpperText.Text = "";
				this.AnomalyId = this.Tubes.RandomIdx<SpriteRenderer>();
				this.Tubes[this.AnomalyId].color = Color.red;
				this.LowerButton.color = Color.white;
				base.StartCoroutine(this.BringPanelUp(false));
				return;
			}
			this.UpperText.Text = "ETA: " + (int)this.MyNormTask.TaskTimer;
			return;
		}
		else
		{
			if (this.State == SampleMinigame.States.Selection)
			{
				float num = Mathf.Cos(Time.time * 1.5f) - 0.2f;
				Color color = new Color(num, 1f, num, 1f);
				for (int i = 0; i < this.Buttons.Length; i++)
				{
					this.Buttons[i].color = color;
				}
				return;
			}
			if (this.State == SampleMinigame.States.AwaitingStart)
			{
				float num2 = Mathf.Cos(Time.time * 1.5f) - 0.2f;
				Color color2 = new Color(num2, 1f, num2, 1f);
				this.LowerButton.color = color2;
			}
			return;
		}
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x000069CC File Offset: 0x00004BCC
	public IEnumerator BringPanelUp(bool isBeginning)
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.PanelMoveSound, false, 1f);
		}
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		Vector3 pos = this.CenterPanel.transform.localPosition;
		for (float i = 0f; i < 0.75f; i += Time.deltaTime)
		{
			pos.y = this.platformY.Lerp(i / 0.75f);
			this.CenterPanel.transform.localPosition = pos;
			yield return wait;
		}
		if (isBeginning)
		{
			this.State = SampleMinigame.States.AwaitingStart;
			this.LowerText.Text = "Press To Begin      -->";
		}
		pos.y = this.platformY.max;
		this.CenterPanel.transform.localPosition = pos;
		yield break;
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x000069E2 File Offset: 0x00004BE2
	public IEnumerator BringPanelDown()
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.PanelMoveSound, false, 1f);
		}
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		Vector3 pos = this.CenterPanel.transform.localPosition;
		for (float i = 0f; i < 0.75f; i += Time.deltaTime)
		{
			pos.y = this.platformY.Lerp(1f - i / 0.75f);
			this.CenterPanel.transform.localPosition = pos;
			yield return wait;
		}
		pos.y = this.platformY.min;
		this.CenterPanel.transform.localPosition = pos;
		yield break;
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x000069F1 File Offset: 0x00004BF1
	private IEnumerator DropTube(int id)
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.dropSounds.Get(), false, 1f);
		}
		this.Tubes[id].color = Color.blue;
		yield break;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x0002AB08 File Offset: 0x00028D08
	public void SelectTube(int tubeId)
	{
		if (this.State != SampleMinigame.States.Selection)
		{
			return;
		}
		this.State = SampleMinigame.States.PrepareSample;
		for (int i = 0; i < this.Buttons.Length; i++)
		{
			this.Buttons[i].color = Color.white;
		}
		base.StartCoroutine(this.CoSelectTube(this.AnomalyId, tubeId));
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00006A07 File Offset: 0x00004C07
	private IEnumerator CoSelectTube(int correctTube, int selectedTube)
	{
		if (selectedTube != correctTube)
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(this.FailSound, false, 1f);
			}
			this.UpperText.Text = "Bad Result";
			this.LowerText.Text = "Bad Result";
			for (int i = 0; i < this.Buttons.Length; i++)
			{
				this.Buttons[i].color = Color.red;
			}
			yield return new WaitForSeconds(0.25f);
			for (int j = 0; j < this.Buttons.Length; j++)
			{
				this.Buttons[j].color = Color.white;
			}
			yield return new WaitForSeconds(0.25f);
			for (int k = 0; k < this.Buttons.Length; k++)
			{
				this.Buttons[k].color = Color.red;
			}
			yield return new WaitForSeconds(0.25f);
			for (int l = 0; l < this.Buttons.Length; l++)
			{
				this.Buttons[l].color = Color.white;
			}
			this.UpperText.Text = "";
		}
		else
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(this.ButtonSound, false, 0.6f);
			}
			this.UpperText.Text = "Thank you";
			this.MyNormTask.NextStep();
			if (this.MyNormTask.IsComplete)
			{
				this.State = SampleMinigame.States.Complete;
			}
		}
		int num = this.MyNormTask.MaxStep - this.MyNormTask.taskStep;
		if (num == 0)
		{
			this.LowerText.Text = "Tests Complete";
		}
		else
		{
			this.LowerText.Text = num + " more";
		}
		yield return this.BringPanelDown();
		for (int m = 0; m < this.Tubes.Length; m++)
		{
			this.Tubes[m].color = Color.white;
		}
		if (!this.MyNormTask.IsComplete)
		{
			yield return this.BringPanelUp(true);
		}
		yield break;
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x0002AB60 File Offset: 0x00028D60
	public void NextStep()
	{
		if (this.State != SampleMinigame.States.AwaitingStart)
		{
			return;
		}
		this.State = SampleMinigame.States.Processing;
		this.LowerButton.color = Color.white;
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.ButtonSound, false, 1f).volume = 0.6f;
		}
		base.StartCoroutine(this.CoStartProcessing());
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00006A24 File Offset: 0x00004C24
	private IEnumerator CoStartProcessing()
	{
		this.MyNormTask.TaskTimer = this.TimePerStep;
		this.MyNormTask.TimerStarted = NormalPlayerTask.TimerState.Started;
		yield return this.DropLiquid();
		this.LowerText.Text = SampleMinigame.ProcessingStrings.Random<string>();
		yield return this.BringPanelDown();
		yield break;
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00006A33 File Offset: 0x00004C33
	private IEnumerator DropLiquid()
	{
		this.LowerText.Text = "Adding Reagent";
		WaitForSeconds dropWait = new WaitForSeconds(0.25f);
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		Vector3 pos = this.Dropper.transform.localPosition;
		yield return dropWait;
		yield return this.DropTube(0);
		int num;
		for (int step = -2; step < 2; step = num)
		{
			float start = (float)step / 2f * 1.25f;
			float xTarg = (float)(step + 1) / 2f * 1.25f;
			for (float i = 0f; i < 0.15f; i += Time.deltaTime)
			{
				pos.x = Mathf.Lerp(start, xTarg, i / 0.15f);
				this.Dropper.transform.localPosition = pos;
				yield return wait;
			}
			pos.x = xTarg;
			this.Dropper.transform.localPosition = pos;
			yield return dropWait;
			int id = step + 3;
			yield return this.DropTube(id);
			num = step + 1;
		}
		for (float xTarg = 0f; xTarg < 0.15f; xTarg += Time.deltaTime)
		{
			pos.x = this.dropperX.Lerp(1f - xTarg / 0.15f);
			this.Dropper.transform.localPosition = pos;
			yield return wait;
		}
		pos.x = this.dropperX.min;
		this.Dropper.transform.localPosition = pos;
		yield break;
	}

	// Token: 0x0400073F RID: 1855
	private static string[] ProcessingStrings = new string[]
	{
		"Take a break",
		"Go grab a coffee",
		"You dont need to wait",
		"go do something else"
	};

	// Token: 0x04000740 RID: 1856
	private const float PanelMoveDuration = 0.75f;

	// Token: 0x04000741 RID: 1857
	private const byte TubeMask = 15;

	// Token: 0x04000742 RID: 1858
	public TextRenderer UpperText;

	// Token: 0x04000743 RID: 1859
	public TextRenderer LowerText;

	// Token: 0x04000744 RID: 1860
	public float TimePerStep = 15f;

	// Token: 0x04000745 RID: 1861
	public FloatRange platformY = new FloatRange(-3.5f, -0.75f);

	// Token: 0x04000746 RID: 1862
	public FloatRange dropperX = new FloatRange(-1.25f, 1.25f);

	// Token: 0x04000747 RID: 1863
	public SpriteRenderer CenterPanel;

	// Token: 0x04000748 RID: 1864
	public SpriteRenderer Dropper;

	// Token: 0x04000749 RID: 1865
	public SpriteRenderer[] Tubes;

	// Token: 0x0400074A RID: 1866
	public SpriteRenderer[] Buttons;

	// Token: 0x0400074B RID: 1867
	public SpriteRenderer LowerButton;

	// Token: 0x0400074C RID: 1868
	public AudioClip ButtonSound;

	// Token: 0x0400074D RID: 1869
	public AudioClip PanelMoveSound;

	// Token: 0x0400074E RID: 1870
	public AudioClip FailSound;

	// Token: 0x0400074F RID: 1871
	public AudioClip[] DropSounds;

	// Token: 0x04000750 RID: 1872
	private RandomFill<AudioClip> dropSounds;

	// Token: 0x02000169 RID: 361
	public enum States : byte
	{
		// Token: 0x04000752 RID: 1874
		PrepareSample,
		// Token: 0x04000753 RID: 1875
		Complete = 16,
		// Token: 0x04000754 RID: 1876
		AwaitingStart = 32,
		// Token: 0x04000755 RID: 1877
		Selection = 64,
		// Token: 0x04000756 RID: 1878
		Processing = 128
	}
}
