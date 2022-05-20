using System.Collections;
using UnityEngine;

public class SampleMinigame : Minigame
{
	public enum States : byte
	{
		PrepareSample = 0,
		Complete = 0x10,
		AwaitingStart = 0x20,
		Selection = 0x40,
		Processing = 0x80
	}

	private static string[] ProcessingStrings = new string[10]
	{
		"Take a break",
		"Go grab a coffee",
		"You dont need to wait",
		"Go do something else",
		"Impostor was here",
		"Stoopidman was here",
		"Walk away please",
		"Go and play Pokemon Go",
		"Vibe somewhere else",
		"Losing format"
	};

	private const float PanelMoveDuration = 0.75f;

	private const byte TubeMask = 15;

	public TextRenderer UpperText;

	public TextRenderer LowerText;

	public float TimePerStep = 15f;

	public FloatRange platformY = new FloatRange(-3.5f, -0.75f);

	public FloatRange dropperX = new FloatRange(-1.25f, 1.25f);

	public SpriteRenderer CenterPanel;

	public SpriteRenderer Dropper;

	public SpriteRenderer[] Tubes;

	public SpriteRenderer[] Buttons;

	public SpriteRenderer LowerButton;

	public AudioClip ButtonSound;

	public AudioClip PanelMoveSound;

	public AudioClip FailSound;

	public AudioClip[] DropSounds;

	private RandomFill<AudioClip> dropSounds;

	private States State
	{
		get
		{
			return (States)MyNormTask.Data[0];
		}
		set
		{
			MyNormTask.Data[0] = (byte)value;
		}
	}

	private int AnomalyId
	{
		get
		{
			return MyNormTask.Data[1];
		}
		set
		{
			MyNormTask.Data[1] = (byte)value;
		}
	}

	public void Awake()
	{
		dropSounds = new RandomFill<AudioClip>();
		dropSounds.Set(DropSounds);
	}

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		switch (State)
		{
		case States.Processing:
		{
			for (int j = 0; j < Tubes.Length; j++)
			{
				Tubes[j].color = Color.blue;
			}
			LowerText.Text = ProcessingStrings.Random();
			SetPlatformBottom();
			break;
		}
		case States.Selection:
		{
			for (int i = 0; i < Tubes.Length; i++)
			{
				Tubes[i].color = Color.blue;
			}
			Tubes[AnomalyId].color = Color.red;
			LowerText.Text = "Select Anomaly";
			SetPlatformTop();
			break;
		}
		case States.AwaitingStart:
			LowerText.Text = "Press To Begin      -->";
			SetPlatformTop();
			break;
		case States.PrepareSample:
			StartCoroutine(BringPanelUp(isBeginning: true));
			break;
		}
	}

	private void SetPlatformBottom()
	{
		Vector3 localPosition = CenterPanel.transform.localPosition;
		localPosition.y = platformY.min;
		CenterPanel.transform.localPosition = localPosition;
	}

	private void SetPlatformTop()
	{
		Vector3 localPosition = CenterPanel.transform.localPosition;
		localPosition.y = platformY.max;
		CenterPanel.transform.localPosition = localPosition;
	}

	public void FixedUpdate()
	{
		if (State == States.Processing)
		{
			if (MyNormTask.TaskTimer <= 0f)
			{
				State = States.Selection;
				LowerText.Text = "Select Anomaly";
				UpperText.Text = "";
				AnomalyId = Tubes.RandomIdx();
				Tubes[AnomalyId].color = Color.red;
				LowerButton.color = Color.white;
				StartCoroutine(BringPanelUp(isBeginning: false));
			}
			else
			{
				UpperText.Text = "ETA: " + (int)MyNormTask.TaskTimer;
			}
		}
		else if (State == States.Selection)
		{
			float num = Mathf.Cos(Time.time * 1.5f) - 0.2f;
			Color color = new Color(num, 1f, num, 1f);
			for (int i = 0; i < Buttons.Length; i++)
			{
				Buttons[i].color = color;
			}
		}
		else if (State == States.AwaitingStart)
		{
			float num2 = Mathf.Cos(Time.time * 1.5f) - 0.2f;
			Color color2 = new Color(num2, 1f, num2, 1f);
			LowerButton.color = color2;
		}
	}

	public IEnumerator BringPanelUp(bool isBeginning)
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(PanelMoveSound, loop: false);
		}
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		Vector3 pos = CenterPanel.transform.localPosition;
		for (float i = 0f; i < 0.75f; i += Time.deltaTime)
		{
			pos.y = platformY.Lerp(i / 0.75f);
			CenterPanel.transform.localPosition = pos;
			yield return wait;
		}
		if (isBeginning)
		{
			State = States.AwaitingStart;
			LowerText.Text = "Press To Begin      -->";
		}
		pos.y = platformY.max;
		CenterPanel.transform.localPosition = pos;
	}

	public IEnumerator BringPanelDown()
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(PanelMoveSound, loop: false);
		}
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		Vector3 pos = CenterPanel.transform.localPosition;
		for (float i = 0f; i < 0.75f; i += Time.deltaTime)
		{
			pos.y = platformY.Lerp(1f - i / 0.75f);
			CenterPanel.transform.localPosition = pos;
			yield return wait;
		}
		pos.y = platformY.min;
		CenterPanel.transform.localPosition = pos;
	}

	private IEnumerator DropTube(int id)
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(dropSounds.Get(), loop: false);
		}
		Tubes[id].color = Color.blue;
		yield break;
	}

	public void SelectTube(int tubeId)
	{
		if (State == States.Selection)
		{
			State = States.PrepareSample;
			for (int i = 0; i < Buttons.Length; i++)
			{
				Buttons[i].color = Color.white;
			}
			StartCoroutine(CoSelectTube(AnomalyId, tubeId));
		}
	}

	private IEnumerator CoSelectTube(int correctTube, int selectedTube)
	{
		if (selectedTube != correctTube)
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(FailSound, loop: false);
			}
			UpperText.Text = "Bad Result";
			LowerText.Text = "Bad Result";
			for (int i = 0; i < Buttons.Length; i++)
			{
				Buttons[i].color = Color.red;
			}
			yield return new WaitForSeconds(0.25f);
			for (int j = 0; j < Buttons.Length; j++)
			{
				Buttons[j].color = Color.white;
			}
			yield return new WaitForSeconds(0.25f);
			for (int k = 0; k < Buttons.Length; k++)
			{
				Buttons[k].color = Color.red;
			}
			yield return new WaitForSeconds(0.25f);
			for (int l = 0; l < Buttons.Length; l++)
			{
				Buttons[l].color = Color.white;
			}
			UpperText.Text = "";
		}
		else
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(ButtonSound, loop: false, 0.6f);
			}
			UpperText.Text = "Thank you";
			MyNormTask.NextStep();
			if (MyNormTask.IsComplete)
			{
				State = States.Complete;
			}
		}
		int num = MyNormTask.MaxStep - MyNormTask.taskStep;
		if (num == 0)
		{
			LowerText.Text = "Tests Complete";
		}
		else
		{
			LowerText.Text = num + " more";
		}
		yield return BringPanelDown();
		for (int m = 0; m < Tubes.Length; m++)
		{
			Tubes[m].color = Color.white;
		}
		if (!MyNormTask.IsComplete)
		{
			yield return BringPanelUp(isBeginning: true);
		}
	}

	public void NextStep()
	{
		if (State == States.AwaitingStart)
		{
			State = States.Processing;
			LowerButton.color = Color.white;
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(ButtonSound, loop: false).volume = 0.6f;
			}
			StartCoroutine(CoStartProcessing());
		}
	}

	private IEnumerator CoStartProcessing()
	{
		MyNormTask.TaskTimer = TimePerStep * GameOptionsData.TaskDifficultyMult[PlayerControl.GameOptions.TaskDifficulty];
		MyNormTask.TimerStarted = NormalPlayerTask.TimerState.Started;
		yield return DropLiquid();
		LowerText.Text = ProcessingStrings.Random();
		yield return BringPanelDown();
	}

	private IEnumerator DropLiquid()
	{
		LowerText.Text = "Adding Reagent";
		WaitForSeconds dropWait = new WaitForSeconds(0.25f);
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		Vector3 pos = Dropper.transform.localPosition;
		yield return dropWait;
		yield return DropTube(0);
		int step = -2;
		while (step < 2)
		{
			float start = (float)step / 2f * 1.25f;
			float xTarg = (float)(step + 1) / 2f * 1.25f;
			for (float i = 0f; i < 0.15f; i += Time.deltaTime)
			{
				pos.x = Mathf.Lerp(start, xTarg, i / 0.15f);
				Dropper.transform.localPosition = pos;
				yield return wait;
			}
			pos.x = xTarg;
			Dropper.transform.localPosition = pos;
			yield return dropWait;
			int id = step + 3;
			yield return DropTube(id);
			int num = step + 1;
			step = num;
		}
		for (float xTarg = 0f; xTarg < 0.15f; xTarg += Time.deltaTime)
		{
			pos.x = dropperX.Lerp(1f - xTarg / 0.15f);
			Dropper.transform.localPosition = pos;
			yield return wait;
		}
		pos.x = dropperX.min;
		Dropper.transform.localPosition = pos;
	}
}
