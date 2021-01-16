using System.Collections;
using UnityEngine;

public class CardSlideGame : Minigame
{
	private enum TaskStages
	{
		Before,
		Animating,
		Inserted,
		After
	}

	private Color gray = new Color(0.45f, 0.45f, 0.45f);

	private Color green = new Color(0f, 0.8f, 0f);

	private TaskStages State;

	private Controller myController = new Controller();

	private FloatRange XRange = new FloatRange(-2.38f, 2.38f);

	public FloatRange AcceptedTime = new FloatRange(0.4f, 0.6f);

	public Collider2D col;

	public SpriteRenderer redLight;

	public SpriteRenderer greenLight;

	public TextRenderer StatusText;

	public AudioClip AcceptSound;

	public AudioClip[] CardMove;

	public AudioClip WalletOut;

	public float dragTime;

	private bool moving;

	private int SwipesRemaining;

	public void Update()
	{
		if (MyNormTask.IsComplete)
		{
			return;
		}
		myController.Update();
		Vector3 localPosition = col.transform.localPosition;
		switch (myController.CheckDrag(col))
		{
		case DragState.TouchStart:
			dragTime = 0f;
			break;
		case DragState.Dragging:
		{
			if (State != TaskStages.Inserted)
			{
				break;
			}
			Vector2 vector = myController.DragPosition - (Vector2)base.transform.position;
			vector.x = XRange.Clamp(vector.x);
			if (vector.x - localPosition.x > 0.01f)
			{
				dragTime += Time.deltaTime;
				redLight.color = gray;
				greenLight.color = gray;
				if (!moving)
				{
					moving = true;
					if (Constants.ShouldPlaySfx())
					{
						SoundManager.Instance.PlaySound(CardMove.Random(), loop: false);
					}
				}
			}
			localPosition.x = vector.x;
			break;
		}
		case DragState.NoTouch:
			if (State == TaskStages.Inserted)
			{
				localPosition.x = Mathf.Lerp(localPosition.x, XRange.min, Time.deltaTime * 4f);
			}
			break;
		case DragState.Released:
			moving = false;
			if (State == TaskStages.Before)
			{
				State = TaskStages.Animating;
				StartCoroutine(InsertCard());
			}
			else
			{
				if (State != TaskStages.Inserted)
				{
					break;
				}
				if (XRange.max - localPosition.x < 0.05f)
				{
					if (AcceptedTime.Contains(dragTime) || PlayerControl.GameOptions.TaskDifficulty == 0)
					{
						if (Constants.ShouldPlaySfx())
						{
							SoundManager.Instance.PlaySound(AcceptSound, loop: false);
						}
						SwipesRemaining--;
						if (SwipesRemaining == 0)
						{
							State = TaskStages.After;
							StatusText.Text = "Accepted. Thank you.";
							StartCoroutine(PutCardBack());
							if ((bool)MyNormTask)
							{
								MyNormTask.NextStep();
							}
						}
						else
                        {
							StatusText.Text = "Accepted. Swipe " + SwipesRemaining + " more times.";
						}
						redLight.color = gray;
						greenLight.color = green;
					}
					else
					{
						if (AcceptedTime.max < dragTime)
						{
							StatusText.Text = "Too slow. Try again.";
						}
						else
						{
							StatusText.Text = "Too fast. Try again.";
						}
						if (PlayerControl.GameOptions.TaskDifficulty == 3)
						{
								SwipesRemaining = 4;
						}
						redLight.color = Color.red;
						greenLight.color = gray;
					}
				}
				else
				{
					if (PlayerControl.GameOptions.TaskDifficulty == 3)
					{
						SwipesRemaining = 4;
					}
					StatusText.Text = "Bad read. Try again.";
					redLight.color = Color.red;
					greenLight.color = gray;
				}
			}
			break;
		}
		col.transform.localPosition = localPosition;
	}

	private IEnumerator PutCardBack()
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(WalletOut, loop: false);
		}
		Vector3 pos = col.transform.localPosition;
		Vector3 targ = new Vector3(-1.11f, -1.9f, pos.z);
		float time = 0f;
		while (true)
		{
			float t = Mathf.Min(1f, time / 0.6f);
			col.transform.localPosition = Vector3.Lerp(pos, targ, t);
			col.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.75f, t);
			if (time > 0.6f)
			{
				break;
			}
			yield return null;
			time += Time.deltaTime;
		}
		StartCoroutine(CoStartClose());
	}

	private IEnumerator InsertCard()
	{
        if (Constants.ShouldPlaySfx())
        {
            SoundManager.Instance.PlaySound(WalletOut, loop: false);
        }
		if (PlayerControl.GameOptions.TaskDifficulty >= 2)
		{
			SwipesRemaining = 4;
		}
		else
        {
			SwipesRemaining = 1;
        }
		Vector3 pos = col.transform.localPosition;
		Vector3 targ = new Vector3(XRange.min, 0.75f, pos.z);
		float time = 0f;
		while (true)
		{
			float t = Mathf.Min(1f, time / 0.6f);
			col.transform.localPosition = Vector3.Lerp(pos, targ, t);
			col.transform.localScale = Vector3.Lerp(Vector3.one * 0.75f, Vector3.one, t);
			if (time > 0.6f)
			{
				break;
			}
			yield return null;
			time += Time.deltaTime;
		}
		StatusText.Text = "Please swipe card";
		greenLight.color = green;
		State = TaskStages.Inserted;
	}
}
