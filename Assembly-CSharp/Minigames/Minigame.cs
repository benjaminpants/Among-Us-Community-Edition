using System;
using System.Collections;
using UnityEngine;

public abstract class Minigame : MonoBehaviour
{
	protected enum CloseState
	{
		None,
		Waiting,
		Closing
	}

	public static Minigame Instance;

	public const float Depth = -50f;

	public TransitionType TransType;

	protected PlayerTask MyTask;

	protected NormalPlayerTask MyNormTask;

	protected CloseState amClosing;

	public AudioClip OpenSound;

	public AudioClip CloseSound;

	public Console Console
	{
		get;
		set;
	}

	protected int ConsoleId
	{
		get
		{
			if (!Console)
			{
				return 0;
			}
			return Console.ConsoleId;
		}
	}

	public virtual void Begin(PlayerTask task)
	{
		Instance = this;
		MyTask = task;
		MyNormTask = task as NormalPlayerTask;
		if ((bool)PlayerControl.LocalPlayer)
		{
			if ((bool)MapBehaviour.Instance)
			{
				MapBehaviour.Instance.Close();
			}
			PlayerControl.LocalPlayer.NetTransform.Halt();
		}
		StartCoroutine(CoAnimateOpen());
	}

	protected IEnumerator CoStartClose(float duration = 0.75f)
	{
		if (amClosing == CloseState.None)
		{
			amClosing = CloseState.Waiting;
			yield return Effects.Wait(duration);
			Close();
		}
	}

	[Obsolete("Don't use, I just don't want to reselect the close button event handlers", true)]
	public void Close(bool allowMovement)
	{
		Close();
	}

	public virtual void Close()
	{
		PlayerControl.LocalPlayer.moveable = true;
		if (amClosing != CloseState.Closing)
		{
			if ((bool)CloseSound && Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(CloseSound, loop: false);
			}
			amClosing = CloseState.Closing;
			StartCoroutine(CoDestroySelf());
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected virtual IEnumerator CoAnimateOpen()
	{
		if ((bool)OpenSound && Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(OpenSound, loop: false);
		}
		switch (TransType)
		{
		case TransitionType.SlideBottom:
		{
			for (float timer2 = 0f; timer2 < 0.25f; timer2 += Time.deltaTime)
			{
				float t2 = timer2 / 0.25f;
				base.transform.localPosition = new Vector3(0f, Mathf.SmoothStep(-8f, 0f, t2), -50f);
				yield return null;
			}
			base.transform.localPosition = new Vector3(0f, 0f, -50f);
			break;
		}
		case TransitionType.Alpha:
		{
			SpriteRenderer[] rends = GetComponentsInChildren<SpriteRenderer>();
			for (float timer2 = 0f; timer2 < 0.25f; timer2 += Time.deltaTime)
			{
				float t = timer2 / 0.25f;
				for (int i = 0; i < rends.Length; i++)
				{
					rends[i].color = Color.Lerp(Palette.ClearWhite, Color.white, t);
				}
				yield return null;
			}
			for (int j = 0; j < rends.Length; j++)
			{
				rends[j].color = Color.white;
			}
			break;
		}
		}
	}

	protected virtual IEnumerator CoDestroySelf()
	{
		switch (TransType)
		{
		case TransitionType.SlideBottom:
		{
			for (float timer2 = 0f; timer2 < 0.25f; timer2 += Time.deltaTime)
			{
				float t2 = timer2 / 0.25f;
				base.transform.localPosition = new Vector3(0f, Mathf.SmoothStep(0f, -8f, t2), -50f);
				yield return null;
			}
			break;
		}
		case TransitionType.Alpha:
		{
			SpriteRenderer[] rends = GetComponentsInChildren<SpriteRenderer>();
			for (float timer2 = 0f; timer2 < 0.25f; timer2 += Time.deltaTime)
			{
				float t = timer2 / 0.25f;
				for (int i = 0; i < rends.Length; i++)
				{
					rends[i].color = Color.Lerp(Color.white, Palette.ClearWhite, t);
				}
				yield return null;
			}
			break;
		}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
