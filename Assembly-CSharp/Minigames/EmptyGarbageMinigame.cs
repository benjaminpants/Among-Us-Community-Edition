using System;
using System.Collections;
using System.Linq;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class EmptyGarbageMinigame : Minigame
{
	// Token: 0x06000B43 RID: 2883 RVA: 0x000384CC File Offset: 0x000366CC
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		int i = 0;
		this.Objects = new SpriteRenderer[this.NumObjects];
		RandomFill<SpriteRenderer> randomFill = new RandomFill<SpriteRenderer>();
		NormalPlayerTask myNormTask = this.MyNormTask;
		if (myNormTask != null && myNormTask.taskStep == 0)
		{
			if (this.MyNormTask.StartAt == SystemTypes.Cafeteria)
			{
				randomFill.Set(this.GarbagePrefabs);
			}
			else
			{
				randomFill.Set(this.LeafPrefabs);
			}
		}
		else
		{
			randomFill.Set(this.GarbagePrefabs.Union(this.LeafPrefabs));
			while (i < this.SpecialObjectPrefabs.Length)
			{
				SpriteRenderer spriteRenderer = this.Objects[i] = UnityEngine.Object.Instantiate<SpriteRenderer>(this.SpecialObjectPrefabs[i]);
				spriteRenderer.transform.SetParent(base.transform);
				spriteRenderer.transform.localPosition = this.SpawnRange.Next();
				i++;
			}
		}
		while (i < this.Objects.Length)
		{
			SpriteRenderer spriteRenderer2 = this.Objects[i] = UnityEngine.Object.Instantiate<SpriteRenderer>(randomFill.Get());
			spriteRenderer2.transform.SetParent(base.transform);
			Vector3 vector = this.SpawnRange.Next();
			vector.z = FloatRange.Next(-0.5f, 0.5f);
			spriteRenderer2.transform.localPosition = vector;
			spriteRenderer2.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.black, (vector.z + 0.5f) * 0.7f);
			i++;
		}
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x00038644 File Offset: 0x00036844
	public void Update()
	{
		if (this.amClosing != Minigame.CloseState.None)
		{
			return;
		}
		this.controller.Update();
		Vector3 localPosition = this.Handle.transform.localPosition;
		float num = this.HandleRange.ReverseLerp(localPosition.y);
		switch (this.controller.CheckDrag(this.Handle, false))
		{
		case DragState.NoTouch:
			localPosition.y = Mathf.Lerp(localPosition.y, this.HandleRange.max, num + Time.deltaTime * 15f);
			break;
		case DragState.Dragging:
			if (!this.finished)
			{
				if (num > 0.5f)
				{
					Vector2 vector = this.controller.DragPosition - (Vector2)base.transform.position;
					float num2 = this.HandleRange.ReverseLerp(this.HandleRange.Clamp(vector.y));
					localPosition.y = this.HandleRange.Lerp(num2 / 2f + 0.5f);
				}
				else
				{
					localPosition.y = Mathf.Lerp(localPosition.y, this.HandleRange.min, num + Time.deltaTime * 15f);
					if (this.Blocker.enabled)
					{
						if (Constants.ShouldPlaySfx())
						{
							SoundManager.Instance.PlaySound(this.LeverDown, false, 1f);
							SoundManager.Instance.PlaySound(this.GrinderStart, false, 0.8f);
							SoundManager.Instance.StopSound(this.GrinderEnd);
							SoundManager.Instance.StopSound(this.GrinderLoop);
						}
						this.Blocker.enabled = false;
						base.StopAllCoroutines();
						base.StartCoroutine(this.PopObjects());
						base.StartCoroutine(this.AnimateObjects());
					}
				}
			}
			break;
		case DragState.Released:
			if (!this.Blocker.enabled)
			{
				this.Blocker.enabled = true;
				if (Constants.ShouldPlaySfx())
				{
					SoundManager.Instance.PlaySound(this.LeverUp, false, 1f);
					SoundManager.Instance.StopSound(this.GrinderStart);
					SoundManager.Instance.StopSound(this.GrinderLoop);
					SoundManager.Instance.PlaySound(this.GrinderEnd, false, 0.8f);
				}
			}
			if (!this.finished)
			{
				if (this.Objects.All((SpriteRenderer o) => !o))
				{
					this.finished = true;
					this.MyNormTask.NextStep();
					base.StartCoroutine(base.CoStartClose(0.75f));
				}
			}
			break;
		}
		if (Constants.ShouldPlaySfx() && !this.Blocker.enabled && !SoundManager.Instance.SoundIsPlaying(this.GrinderStart))
		{
			SoundManager.Instance.PlaySound(this.GrinderLoop, true, 0.8f);
		}
		this.Handle.transform.localPosition = localPosition;
		Vector3 localScale = this.Bars.transform.localScale;
		localScale.y = this.HandleRange.ChangeRange(localPosition.y, -1f, 1f);
		this.Bars.transform.localScale = localScale;
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x00008CDB File Offset: 0x00006EDB
	private IEnumerator PopObjects()
	{
		this.Popper.enabled = true;
		yield return new WaitForSeconds(0.05f);
		this.Popper.enabled = false;
		yield break;
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x00008CEA File Offset: 0x00006EEA
	private IEnumerator AnimateObjects()
	{
		Vector3 pos = base.transform.localPosition;
		for (float t = 3f; t > 0f; t -= Time.deltaTime)
		{
			float d = t / 3f;
			base.transform.localPosition = pos + (Vector3)(Vector2Range.NextEdge() * d * 0.1f);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x00038978 File Offset: 0x00036B78
	public override void Close()
	{
		SoundManager.Instance.StopSound(this.GrinderStart);
		SoundManager.Instance.StopSound(this.GrinderLoop);
		SoundManager.Instance.StopSound(this.GrinderEnd);
		if (this.MyNormTask && this.MyNormTask.IsComplete)
		{
			ShipStatus.Instance.OpenHatch();
			PlayerControl.LocalPlayer.RpcPlayAnimation((byte)this.MyTask.TaskType);
		}
		base.Close();
	}

	// Token: 0x04000AD5 RID: 2773
	private const float GrinderVolume = 0.8f;

	// Token: 0x04000AD6 RID: 2774
	public FloatRange HandleRange = new FloatRange(-0.65f, 0.65f);

	// Token: 0x04000AD7 RID: 2775
	public Vector2Range SpawnRange;

	// Token: 0x04000AD8 RID: 2776
	public Collider2D Blocker;

	// Token: 0x04000AD9 RID: 2777
	public AreaEffector2D Popper;

	// Token: 0x04000ADA RID: 2778
	public Collider2D Handle;

	// Token: 0x04000ADB RID: 2779
	public SpriteRenderer Bars;

	// Token: 0x04000ADC RID: 2780
	private Controller controller = new Controller();

	// Token: 0x04000ADD RID: 2781
	private bool finished;

	// Token: 0x04000ADE RID: 2782
	public int NumObjects = 15;

	// Token: 0x04000ADF RID: 2783
	private SpriteRenderer[] Objects;

	// Token: 0x04000AE0 RID: 2784
	public SpriteRenderer[] GarbagePrefabs;

	// Token: 0x04000AE1 RID: 2785
	public SpriteRenderer[] LeafPrefabs;

	// Token: 0x04000AE2 RID: 2786
	public SpriteRenderer[] SpecialObjectPrefabs;

	// Token: 0x04000AE3 RID: 2787
	public AudioClip LeverDown;

	// Token: 0x04000AE4 RID: 2788
	public AudioClip LeverUp;

	// Token: 0x04000AE5 RID: 2789
	public AudioClip GrinderStart;

	// Token: 0x04000AE6 RID: 2790
	public AudioClip GrinderLoop;

	// Token: 0x04000AE7 RID: 2791
	public AudioClip GrinderEnd;
}
