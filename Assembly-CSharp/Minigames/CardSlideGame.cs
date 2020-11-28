using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000002 RID: 2
public class CardSlideGame : Minigame
{
	// Token: 0x06000001 RID: 1 RVA: 0x0000AB74 File Offset: 0x00008D74
	public void Update()
	{
		if (this.MyNormTask.IsComplete)
		{
			return;
		}
		this.myController.Update();
		Vector3 localPosition = this.col.transform.localPosition;
		switch (this.myController.CheckDrag(this.col, false))
		{
		case DragState.NoTouch:
			if (this.State == CardSlideGame.TaskStages.Inserted)
			{
				localPosition.x = Mathf.Lerp(localPosition.x, this.XRange.min, Time.deltaTime * 4f);
			}
			break;
		case DragState.TouchStart:
			this.dragTime = 0f;
			break;
		case DragState.Dragging:
			if (this.State == CardSlideGame.TaskStages.Inserted)
			{
				Vector2 vector = this.myController.DragPosition - (Vector2)base.transform.position;
				vector.x = this.XRange.Clamp(vector.x);
				if (vector.x - localPosition.x > 0.01f)
				{
					this.dragTime += Time.deltaTime;
					this.redLight.color = this.gray;
					this.greenLight.color = this.gray;
					if (!this.moving)
					{
						this.moving = true;
						if (Constants.ShouldPlaySfx())
						{
							SoundManager.Instance.PlaySound(this.CardMove.Random<AudioClip>(), false, 1f);
						}
					}
				}
				localPosition.x = vector.x;
			}
			break;
		case DragState.Released:
			this.moving = false;
			if (this.State == CardSlideGame.TaskStages.Before)
			{
				this.State = CardSlideGame.TaskStages.Animating;
				base.StartCoroutine(this.InsertCard());
			}
			else if (this.State == CardSlideGame.TaskStages.Inserted)
			{
				if (this.XRange.max - localPosition.x < 0.05f)
				{
					if (this.AcceptedTime.Contains(this.dragTime))
					{
						if (Constants.ShouldPlaySfx())
						{
							SoundManager.Instance.PlaySound(this.AcceptSound, false, 1f);
						}
						this.State = CardSlideGame.TaskStages.After;
						this.StatusText.Text = "Accepted. Thank you.";
						base.StartCoroutine(this.PutCardBack());
						if (this.MyNormTask)
						{
							this.MyNormTask.NextStep();
						}
						this.redLight.color = this.gray;
						this.greenLight.color = this.green;
					}
					else
					{
						if (this.AcceptedTime.max < this.dragTime)
						{
							this.StatusText.Text = "Too slow. Try again";
						}
						else
						{
							this.StatusText.Text = "Too fast. Try again.";
						}
						this.redLight.color = Color.red;
						this.greenLight.color = this.gray;
					}
				}
				else
				{
					this.StatusText.Text = "Bad read. Try again.";
					this.redLight.color = Color.red;
					this.greenLight.color = this.gray;
				}
			}
			break;
		}
		this.col.transform.localPosition = localPosition;
	}

	// Token: 0x06000002 RID: 2 RVA: 0x00002238 File Offset: 0x00000438
	private IEnumerator PutCardBack()
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.WalletOut, false, 1f);
		}
		Vector3 pos = this.col.transform.localPosition;
		Vector3 targ = new Vector3(-1.11f, -1.9f, pos.z);
		float time = 0f;
		for (;;)
		{
			float t = Mathf.Min(1f, time / 0.6f);
			this.col.transform.localPosition = Vector3.Lerp(pos, targ, t);
			this.col.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.75f, t);
			if (time > 0.6f)
			{
				break;
			}
			yield return null;
			time += Time.deltaTime;
		}
		base.StartCoroutine(base.CoStartClose(0.75f));
		yield break;
	}

	// Token: 0x06000003 RID: 3 RVA: 0x00002247 File Offset: 0x00000447
	private IEnumerator InsertCard()
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.WalletOut, false, 1f);
		}
		Vector3 pos = this.col.transform.localPosition;
		Vector3 targ = new Vector3(this.XRange.min, 0.75f, pos.z);
		float time = 0f;
		for (;;)
		{
			float t = Mathf.Min(1f, time / 0.6f);
			this.col.transform.localPosition = Vector3.Lerp(pos, targ, t);
			this.col.transform.localScale = Vector3.Lerp(Vector3.one * 0.75f, Vector3.one, t);
			if (time > 0.6f)
			{
				break;
			}
			yield return null;
			time += Time.deltaTime;
		}
		this.StatusText.Text = "Please swipe card";
		this.greenLight.color = this.green;
		this.State = CardSlideGame.TaskStages.Inserted;
		yield break;
	}

	// Token: 0x04000001 RID: 1
	private Color gray = new Color(0.45f, 0.45f, 0.45f);

	// Token: 0x04000002 RID: 2
	private Color green = new Color(0f, 0.8f, 0f);

	// Token: 0x04000003 RID: 3
	private CardSlideGame.TaskStages State;

	// Token: 0x04000004 RID: 4
	private Controller myController = new Controller();

	// Token: 0x04000005 RID: 5
	private FloatRange XRange = new FloatRange(-2.38f, 2.38f);

	// Token: 0x04000006 RID: 6
	public FloatRange AcceptedTime = new FloatRange(0.4f, 0.6f);

	// Token: 0x04000007 RID: 7
	public Collider2D col;

	// Token: 0x04000008 RID: 8
	public SpriteRenderer redLight;

	// Token: 0x04000009 RID: 9
	public SpriteRenderer greenLight;

	// Token: 0x0400000A RID: 10
	public TextRenderer StatusText;

	// Token: 0x0400000B RID: 11
	public AudioClip AcceptSound;

	// Token: 0x0400000C RID: 12
	public AudioClip[] CardMove;

	// Token: 0x0400000D RID: 13
	public AudioClip WalletOut;

	// Token: 0x0400000E RID: 14
	public float dragTime;

	// Token: 0x0400000F RID: 15
	private bool moving;

	// Token: 0x02000003 RID: 3
	private enum TaskStages
	{
		// Token: 0x04000011 RID: 17
		Before,
		// Token: 0x04000012 RID: 18
		Animating,
		// Token: 0x04000013 RID: 19
		Inserted,
		// Token: 0x04000014 RID: 20
		After
	}
}
