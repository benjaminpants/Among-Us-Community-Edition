using System.Collections;
using UnityEngine;

public class WeaponsMinigame : Minigame
{
	public FloatRange XSpan = new FloatRange(-1.15f, 1.15f);

	public FloatRange YSpan = new FloatRange(-1.15f, 1.15f);

	public FloatRange TimeToSpawn;

	public ObjectPoolBehavior asteroidPool;

	public TextController ScoreText;

	public SpriteRenderer TargetReticle;

	public LineRenderer TargetLines;

	private Vector3 TargetCenter;

	public Collider2D BackgroundCol;

	public SpriteRenderer Background;

	public Controller myController = new Controller();

	private float Timer;

	public AudioClip ShootSound;

	public AudioClip[] ExplodeSounds;

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		ScoreText.Text = "Destroyed: " + MyNormTask.taskStep;
		TimeToSpawn.Next();
	}

	protected override IEnumerator CoAnimateOpen()
	{
		for (float timer2 = 0f; timer2 < 0.1f; timer2 += Time.deltaTime)
		{
			float num = timer2 / 0.1f;
			base.transform.localScale = new Vector3(num, 0.1f, num);
			yield return null;
		}
		for (float timer2 = 0.0100000007f; timer2 < 0.1f; timer2 += Time.deltaTime)
		{
			float y = timer2 / 0.1f;
			base.transform.localScale = new Vector3(1f, y, 1f);
			yield return null;
		}
		base.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	protected override IEnumerator CoDestroySelf()
	{
		for (float timer2 = 0.0100000007f; timer2 < 0.1f; timer2 += Time.deltaTime)
		{
			float y = 1f - timer2 / 0.1f;
			base.transform.localScale = new Vector3(1f, y, 1f);
			yield return null;
		}
		for (float timer2 = 0f; timer2 < 0.1f; timer2 += Time.deltaTime)
		{
			float num = 1f - timer2 / 0.1f;
			base.transform.localScale = new Vector3(num, 0.1f, num);
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	public void FixedUpdate()
	{
		Background.color = Color.Lerp(Palette.ClearWhite, Color.white, Mathf.Sin(Time.time * 3f) * 0.1f + 0.799999952f);
		if ((bool)MyNormTask && MyNormTask.IsComplete)
		{
			return;
		}
		Timer += Time.fixedDeltaTime;
		if (Timer >= TimeToSpawn.Last)
		{
			Timer = 0f;
			TimeToSpawn.Next();
			if (asteroidPool.InUse < MyNormTask.MaxStep - MyNormTask.TaskStep)
			{
				Asteroid ast = asteroidPool.Get<Asteroid>();
				ast.transform.localPosition = new Vector3(XSpan.max, YSpan.Next(), -1f);
				ast.TargetPosition = new Vector3(XSpan.min, YSpan.Next(), -1f);
				ast.GetComponent<ButtonBehavior>().OnClick.AddListener(delegate
				{
					BreakApart(ast);
				});
			}
		}
		myController.Update();
		if (myController.CheckDrag(BackgroundCol) == DragState.TouchStart)
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(ShootSound, loop: false);
			}
			Vector3 vector = (Vector3)myController.DragPosition - base.transform.position;
			vector.z = -2f;
			TargetReticle.transform.localPosition = vector;
			vector.z = 0f;
			TargetLines.SetPosition(1, vector);
			if (!ShipStatus.Instance.WeaponsImage.IsPlaying())
			{
				ShipStatus.Instance.FireWeapon();
				PlayerControl.LocalPlayer.RpcPlayAnimation(6);
			}
		}
	}

	public void BreakApart(Asteroid ast)
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(ExplodeSounds.Random(), loop: false).pitch = FloatRange.Next(0.8f, 1.2f);
		}
		if (MyNormTask.IsComplete)
		{
			return;
		}
		StartCoroutine(ast.CoBreakApart());
		if ((bool)MyNormTask)
		{
			MyNormTask.NextStep();
			ScoreText.Text = "Destroyed: " + MyNormTask.taskStep;
		}
		if (!MyNormTask || !MyNormTask.IsComplete)
		{
			return;
		}
		StartCoroutine(CoStartClose());
		foreach (Asteroid activeChild in asteroidPool.activeChildren)
		{
			if (!(activeChild == ast))
			{
				StartCoroutine(activeChild.CoBreakApart());
			}
		}
	}
}
