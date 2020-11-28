using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000235 RID: 565
public class WeaponsMinigame : Minigame
{
	// Token: 0x06000C12 RID: 3090 RVA: 0x00009417 File Offset: 0x00007617
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.ScoreText.Text = "Destroyed: " + this.MyNormTask.taskStep;
		this.TimeToSpawn.Next();
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x00009451 File Offset: 0x00007651
	protected override IEnumerator CoAnimateOpen()
	{
		for (float timer = 0f; timer < 0.1f; timer += Time.deltaTime)
		{
			float num = timer / 0.1f;
			base.transform.localScale = new Vector3(num, 0.1f, num);
			yield return null;
		}
		for (float timer = 0.0100000007f; timer < 0.1f; timer += Time.deltaTime)
		{
			float y = timer / 0.1f;
			base.transform.localScale = new Vector3(1f, y, 1f);
			yield return null;
		}
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		yield break;
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x00009460 File Offset: 0x00007660
	protected override IEnumerator CoDestroySelf()
	{
		for (float timer = 0.0100000007f; timer < 0.1f; timer += Time.deltaTime)
		{
			float y = 1f - timer / 0.1f;
			base.transform.localScale = new Vector3(1f, y, 1f);
			yield return null;
		}
		for (float timer = 0f; timer < 0.1f; timer += Time.deltaTime)
		{
			float num = 1f - timer / 0.1f;
			base.transform.localScale = new Vector3(num, 0.1f, num);
			yield return null;
		}
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x0003B108 File Offset: 0x00039308
	public void FixedUpdate()
	{
		this.Background.color = Color.Lerp(Palette.ClearWhite, Color.white, Mathf.Sin(Time.time * 3f) * 0.1f + 0.799999952f);
		if (this.MyNormTask && this.MyNormTask.IsComplete)
		{
			return;
		}
		this.Timer += Time.fixedDeltaTime;
		if (this.Timer >= this.TimeToSpawn.Last)
		{
			this.Timer = 0f;
			this.TimeToSpawn.Next();
			if (this.asteroidPool.InUse < this.MyNormTask.MaxStep - this.MyNormTask.TaskStep)
			{
				Asteroid ast = this.asteroidPool.Get<Asteroid>();
				ast.transform.localPosition = new Vector3(this.XSpan.max, this.YSpan.Next(), -1f);
				ast.TargetPosition = new Vector3(this.XSpan.min, this.YSpan.Next(), -1f);
				ast.GetComponent<ButtonBehavior>().OnClick.AddListener(delegate()
				{
					this.BreakApart(ast);
				});
			}
		}
		this.myController.Update();
		if (this.myController.CheckDrag(this.BackgroundCol, false) == DragState.TouchStart)
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(this.ShootSound, false, 1f);
			}
			Vector3 vector = this.myController.DragPosition - (Vector2)base.transform.position;
			vector.z = -2f;
			this.TargetReticle.transform.localPosition = vector;
			vector.z = 0f;
			this.TargetLines.SetPosition(1, vector);
			if (!ShipStatus.Instance.WeaponsImage.IsPlaying((AnimationClip)null))
			{
				ShipStatus.Instance.FireWeapon();
				PlayerControl.LocalPlayer.RpcPlayAnimation(6);
			}
		}
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x0003B328 File Offset: 0x00039528
	public void BreakApart(Asteroid ast)
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.ExplodeSounds.Random<AudioClip>(), false, 1f).pitch = FloatRange.Next(0.8f, 1.2f);
		}
		if (!this.MyNormTask.IsComplete)
		{
			base.StartCoroutine(ast.CoBreakApart());
			if (this.MyNormTask)
			{
				this.MyNormTask.NextStep();
				this.ScoreText.Text = "Destroyed: " + this.MyNormTask.taskStep;
			}
			if (this.MyNormTask && this.MyNormTask.IsComplete)
			{
				base.StartCoroutine(base.CoStartClose(0.75f));
				foreach (PoolableBehavior poolableBehavior in this.asteroidPool.activeChildren)
				{
					Asteroid asteroid = (Asteroid)poolableBehavior;
					if (!(asteroid == ast))
					{
						base.StartCoroutine(asteroid.CoBreakApart());
					}
				}
			}
		}
	}

	// Token: 0x04000B9F RID: 2975
	public FloatRange XSpan = new FloatRange(-1.15f, 1.15f);

	// Token: 0x04000BA0 RID: 2976
	public FloatRange YSpan = new FloatRange(-1.15f, 1.15f);

	// Token: 0x04000BA1 RID: 2977
	public FloatRange TimeToSpawn;

	// Token: 0x04000BA2 RID: 2978
	public ObjectPoolBehavior asteroidPool;

	// Token: 0x04000BA3 RID: 2979
	public TextController ScoreText;

	// Token: 0x04000BA4 RID: 2980
	public SpriteRenderer TargetReticle;

	// Token: 0x04000BA5 RID: 2981
	public LineRenderer TargetLines;

	// Token: 0x04000BA6 RID: 2982
	private Vector3 TargetCenter;

	// Token: 0x04000BA7 RID: 2983
	public Collider2D BackgroundCol;

	// Token: 0x04000BA8 RID: 2984
	public SpriteRenderer Background;

	// Token: 0x04000BA9 RID: 2985
	public Controller myController = new Controller();

	// Token: 0x04000BAA RID: 2986
	private float Timer;

	// Token: 0x04000BAB RID: 2987
	public AudioClip ShootSound;

	// Token: 0x04000BAC RID: 2988
	public AudioClip[] ExplodeSounds;
}
