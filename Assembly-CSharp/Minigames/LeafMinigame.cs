using System;
using PowerTools;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class LeafMinigame : Minigame
{
	// Token: 0x06000623 RID: 1571 RVA: 0x00025BE4 File Offset: 0x00023DE4
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.Leaves = new Collider2D[this.MyNormTask.MaxStep - this.MyNormTask.taskStep];
		for (int i = 0; i < this.Leaves.Length; i++)
		{
			LeafBehaviour leafBehaviour = UnityEngine.Object.Instantiate<LeafBehaviour>(this.LeafPrefab);
			leafBehaviour.transform.SetParent(base.transform);
			leafBehaviour.Parent = this;
			Vector3 localPosition = this.ValidArea.Next();
			localPosition.z = -1f;
			leafBehaviour.transform.localPosition = localPosition;
			this.Leaves[i] = leafBehaviour.GetComponent<Collider2D>();
		}
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00025C88 File Offset: 0x00023E88
	public void FixedUpdate()
	{
		this.myController.Update();
		for (int i = 0; i < this.Leaves.Length; i++)
		{
			Collider2D collider2D = this.Leaves[i];
			if (collider2D)
			{
				LeafBehaviour component = collider2D.GetComponent<LeafBehaviour>();
				switch (this.myController.CheckDrag(collider2D, false))
				{
				case DragState.TouchStart:
					if (Constants.ShouldPlaySfx())
					{
						SoundManager.Instance.PlaySound(this.LeaveSounds.Random<AudioClip>(), false, 1f);
					}
					for (int j = 0; j < this.Arrows.Length; j++)
					{
						this.Arrows[j].Play(this.Active[j], 1f);
					}
					component.Held = true;
					break;
				case DragState.Dragging:
				{
					Vector2 vector = this.myController.DragPosition - component.body.position;
					component.body.velocity = vector.normalized * Mathf.Min(3f, vector.magnitude * 3f);
					break;
				}
				case DragState.Released:
					component.Held = false;
					for (int k = 0; k < this.Arrows.Length; k++)
					{
						this.Arrows[k].Play(this.Inactive[k], 1f);
						this.Arrows[k].GetComponent<SpriteRenderer>().sprite = null;
					}
					break;
				}
			}
		}
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x00025DF8 File Offset: 0x00023FF8
	public void LeafDone(LeafBehaviour leaf)
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.SuckSounds.Random<AudioClip>(), false, 1f);
		}
		UnityEngine.Object.Destroy(leaf.gameObject);
		if (this.MyNormTask)
		{
			this.MyNormTask.NextStep();
			if (this.MyNormTask.IsComplete)
			{
				for (int i = 0; i < this.Arrows.Length; i++)
				{
					this.Arrows[i].Play(this.Complete[i], 1f);
				}
				base.StartCoroutine(base.CoStartClose(0.75f));
			}
		}
	}

	// Token: 0x04000610 RID: 1552
	public LeafBehaviour LeafPrefab;

	// Token: 0x04000611 RID: 1553
	public Vector2Range ValidArea;

	// Token: 0x04000612 RID: 1554
	public SpriteAnim[] Arrows;

	// Token: 0x04000613 RID: 1555
	public AnimationClip[] Inactive;

	// Token: 0x04000614 RID: 1556
	public AnimationClip[] Active;

	// Token: 0x04000615 RID: 1557
	public AnimationClip[] Complete;

	// Token: 0x04000616 RID: 1558
	private Collider2D[] Leaves;

	// Token: 0x04000617 RID: 1559
	public AudioClip[] LeaveSounds;

	// Token: 0x04000618 RID: 1560
	public AudioClip[] SuckSounds;

	// Token: 0x04000619 RID: 1561
	private Controller myController = new Controller();
}
