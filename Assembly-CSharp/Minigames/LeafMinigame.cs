using PowerTools;
using UnityEngine;

public class LeafMinigame : Minigame
{
	public LeafBehaviour LeafPrefab;

	public Vector2Range ValidArea;

	public SpriteAnim[] Arrows;

	public AnimationClip[] Inactive;

	public AnimationClip[] Active;

	public AnimationClip[] Complete;

	private Collider2D[] Leaves;

	public AudioClip[] LeaveSounds;

	public AudioClip[] SuckSounds;

	private Controller myController = new Controller();

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		Leaves = new Collider2D[MyNormTask.MaxStep - MyNormTask.taskStep];
		for (int i = 0; i < Leaves.Length; i++)
		{
			LeafBehaviour leafBehaviour = Object.Instantiate(LeafPrefab);
			leafBehaviour.transform.SetParent(base.transform);
			leafBehaviour.Parent = this;
			Vector3 localPosition = ValidArea.Next();
			localPosition.z = -1f;
			leafBehaviour.transform.localPosition = localPosition;
			Leaves[i] = leafBehaviour.GetComponent<Collider2D>();
		}
	}

	public void FixedUpdate()
	{
		myController.Update();
		for (int i = 0; i < Leaves.Length; i++)
		{
			Collider2D collider2D = Leaves[i];
			if (!collider2D)
			{
				continue;
			}
			LeafBehaviour component = collider2D.GetComponent<LeafBehaviour>();
			switch (myController.CheckDrag(collider2D))
			{
			case DragState.TouchStart:
			{
				if (Constants.ShouldPlaySfx())
				{
					SoundManager.Instance.PlaySound(LeaveSounds.Random(), loop: false);
				}
				for (int k = 0; k < Arrows.Length; k++)
				{
					Arrows[k].Play(Active[k]);
				}
				component.Held = true;
				break;
			}
			case DragState.Dragging:
			{
				Vector2 vector = myController.DragPosition - component.body.position;
				component.body.velocity = vector.normalized * Mathf.Min(3f, vector.magnitude * 3f);
				break;
			}
			case DragState.Released:
			{
				component.Held = false;
				for (int j = 0; j < Arrows.Length; j++)
				{
					Arrows[j].Play(Inactive[j]);
					Arrows[j].GetComponent<SpriteRenderer>().sprite = null;
				}
				break;
			}
			}
		}
	}

	public void LeafDone(LeafBehaviour leaf)
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(SuckSounds.Random(), loop: false);
		}
		Object.Destroy(leaf.gameObject);
		if (!MyNormTask)
		{
			return;
		}
		MyNormTask.NextStep();
		if (MyNormTask.IsComplete)
		{
			for (int i = 0; i < Arrows.Length; i++)
			{
				Arrows[i].Play(Complete[i]);
			}
			StartCoroutine(CoStartClose());
		}
	}
}
